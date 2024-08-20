;--------------------------------------------------------------------------------------------------
; BOOT LOADER per PIC18 con formato di frame fino a 256 byte
;
; Il Boot loader è compreso fra 0x000 e 0x7FF
;
; Frame UART <Comando> <Add_L> <Add_H> <nByte> <b0> <b1> .. <bn> <CheckSum>;  (nByte 1-255, 0=256)
; 
; CheckSum = -{ <Comando> + <Add_L> + <Add_H> + <nByte> + <b0> + <b1> + .. + <bn> }
;
; Versione Boot del 14/06/2024
;--------------------------------------------------------------------------------------------------
     
        LIST            n=0,w=0,st=Off,r=dec
        PROCESSOR       18F14K22
        RADIX           dec        
        
        #include        ConfigBIT_14K22.inc
        #include        Costanti.inc
        #include        Macro.inc
 
        ;--------------------- Risposte o errori --------------------------------------        
        CONSTANT        WRITE_OK = 0
        CONSTANT        WRITE_ERROR = 1
        CONSTANT        WRITE_FLASH_NO_NEED = 2
        CONSTANT        WRITE_FLASH_OVERDATA = 3
        CONSTANT        WRITE_ADDRESS_ERROR = 4 

        ;--------------------- Comandi ------------------------------------------------
        CONSTANT        COM_LOOP = 0
        CONSTANT        COM_BOOT = 1
        CONSTANT        COM_START = 2
        CONSTANT        COM_READ_FLASH = 3
        CONSTANT        COM_WRITE_FLASH = 4
        CONSTANT        COM_READ_EE = 5
        CONSTANT        COM_WRITE_EE = 6
        CONSTANT        COM_READ_CONFIG = 7

        ;--------------------- Eventi ------------------------------------------------
        CONSTANT        EVENT           = 0xFF

        CONSTANT        EVN_RESET = 0
        CONSTANT        EVN_ERREP_FRAMING = 0x10
        CONSTANT        EVN_ERREP_OVERRUN = 0x11
        CONSTANT        EVN_ERREP_TIMEOUT = 0x12
        CONSTANT        EVN_ERREP_BUFFULL = 0x13
        CONSTANT        EVN_ERREP_CHKSUM  = 0x14


;------------------------------------------------------------------------------
;  Definizioni Flag0                                                 SUBRUOTINE
;------------------------------------------------------------------------------
        #define F_00            Flag0,0 ;
        #define F_01            Flag0,1 ;
        #define F_02            Flag0,2 ;
        #define F_03            Flag0,3 ;
        #define F_04            Flag0,4 ;
        #define F_05            Flag0,5 ;
        #define F_06            Flag0,6 ;
        #define F_07            Flag0,7 ;


        #define S_00            StatoBuff,0 ;
        #define S_01            StatoBuff,1 ;
        #define S_02            StatoBuff,2 ;
        #define S_03            StatoBuff,3 ;
        #define S_04            StatoBuff,4 ;
        #define S_05            StatoBuff,5 ;
        #define S_06            StatoBuff,6 ;
        #define S_07            StatoBuff,7 ;




;--------------------------------------------------------------------------------------------------
;  Variabili verso gli altri moduli
;--------------------------------------------------------------------------------------------------
    extern      int_accesspoint,main_accesspoint
    
;--------------------------------------------------------------------------------------------------
;  Definizioni  ACCESS Ram       GPR Banco 0  0x000 - 0x07F     FSRxH = 0x00
;  Registi di uso generale e Variabili Flag
;--------------------------------------------------------------------------------------------------
RegAndFlag      udata_acs   0
;...........................;
Reg0            res 1
Reg1            res 1
Reg2            res 1
Reg3            res 1
Reg4            res 1
Reg5            res 1
Reg6            res 1
Reg7            res 1
Flag0           res 1
Flag1           res 1
Flag2           res 1
Flag3           res 1
Flag4           res 1
Flag5           res 1
Flag6           res 1
Flag7           res 1


;--------------------------------------------------------------------------------------------------
;  Queste locazioni sono overlay perchè vengono riutilizzate nel main
;--------------------------------------------------------------------------------------------------
Ram_acs_ovr     access_ovr
;...........................;
StatoBuff       res 1       ;FSM buffer
p_rx            res 1       ;puntatore al buffer di upload
RxChkSum        res 1       ;CheckSum di Rx
TxChkSum        res 1       ;CheckSum per Tx
Rx_Command      res 1
Add_L           res 1
Add_H           res 1
Rx_nByte        res 1
CntLoadByte     res 1       ;
p_Low           res 1       ;
Frame_TimeOut   res 2       ;  Frame da 256 byte @ 2Mbit durata 1,3 mSec
Tmr_AttBoot     res 3       ;


    ifndef  EEADRH
EEADRH          res 1       ;
    endif
    
;--------------------------------------------------------------------------------------------------
;  Definizioni Ram GPR Banco 1
;  Queste locazioni sono overlay perchè vengono riutilizzate nel main
;--------------------------------------------------------------------------------------------------
Banco100        udata_ovr   0x100
;...........................;
BuffPagina      res DIM_ERASE_PAGE      ;Obbligatoria all'indirizzo 0x100 !
BuffRx          res DIM_ERASE_PAGE      ;                      
                
;--------------------------------------------------------------------------------------------------
; Vettore di interrupt 0x0008
;--------------------------------------------------------------------------------------------------
reset_code      code    0
                bcf     INTCON,GIE              ;Disabilita interrupt
                movlb   1                       ;Seleziona banco 1 per Ram
                clrf    PCLATU                  ;Max Flash 0xFFFF
                bra     BootLoader              ;Salto al segmento di BOOTLOADER

;--------------------------------------------------------------------------------------------------
; Vettore di interrupt 0x0008
;--------------------------------------------------------------------------------------------------
int_vector      code    0x008
                goto    int_accesspoint 


boot_code       code    0x02a
;--------------------------------------------------------------------------------------------------
; BOOTLOADER
;--------------------------------------------------------------------------------------------------
BootLoader      clrf    Flag0                   ;
                ;----------------------------------------------------------------------------------
                ; Inizializzazioni Porte e Periferiche
                ;----------------------------------------------------------------------------------
                ;...........................;Attiva PLL su oscillatore interno a 8Mhz x4 = 32Mhz
                movlw   b'01110000'
                ;         ||||||++-> SCS1:SCS0: System Clock Select bits
                ;         |||||+-> HFIOFS 1 = INTOSC frequency is stable
                ;         ||||+-> OSTS 1 = primary oscillator is running
                ;         |+++-> IRCF2:IRCF0: Internal Oscillator Frequency Select bits 111=16Mhz 
                ;         +-> IDLEN 1 = Device enters Idle mode on SLEEP instruction
                movwf   OSCCON
        
                bsf     OSCTUNE,PLLEN       ;PLL x4 = 32Mhz abilitato (solo dopo set OSCCON)
                ;...........................;
OS_1            btfss   OSCCON,HFIOFS       ;\Attendi stabilità INTOSC con PLL
                bra     OS_1                ;/
                ;...........................;
            if  TEST == 0
OS_2            btfss   OSCCON2,HFIOFL      ;\Attendi aggancio PLL
                bra     OS_2                ;/
            else
                nop
                nop
            endif
                ;...........................;
                call    Rit_1m              ;Necessario per attendere avvio INTOSC e PLL
                ;...........................;
                rcall   IniSerBoot
                ;...........................;
                movlw   EVN_RESET           ;\Invia evento RESET a PC
                call    Evnt_aPC            ;/
                ;...........................;
                clrf    Tmr_AttBoot         ;\
                clrf    Tmr_AttBoot+1       ;| Set timer per attesa ricezione COM_BOOT
                movlw   5                   ;| 327 mSec@64Mhz
                movwf   Tmr_AttBoot+2       ;/
                ;...........................;
Att_CodBoot     clrf    StatoBuff           ;Resetta la FSM del buffer
ACB_1           decfsz  Tmr_AttBoot         ;\
                bra     ACB_2               ;|
                decfsz  Tmr_AttBoot+1       ;|Controlla scadenza timer attesa COM_BOOT
                bra     ACB_2               ;|
                dcfsnz  Tmr_AttBoot+2       ;|
                bra     StartFirm           ;/Al timeout salta apunto di accesso
                ;...........................;
ACB_2           rcall   GetBufferFDTI       ;\
                tstfsz  WREG                ;|Attendi ricezione Comando di entrare in BOOT
                bra     ACB_1               ;/
                ;..................................................................................
                ;Controlla che il frame ricevuto corretto abbia il Comando COM_BOOT (0xD8)
                ;..................................................................................
                BSFNEL  Rx_Command,COM_BOOT,Att_CodBoot
                BSFNEL  Rx_nByte,1,Att_CodBoot
                rcall   Pop_RxFDTI              ;Codice 0xD8
                BSWNEL  0xD8,Att_CodBoot
                ;..................................................................................
                ;Rispondi al PC che e' stato ricevuto il comando ed il link_PC e' attivo
                ;..................................................................................
                movlw   COM_BOOT                ;\Rispondi a PC con comando: COM_BOOT (0xD8)
                rcall   WrCmd_aPC               ;/
                movlw   0                       ;\Add_L
                rcall   WrData_aPC              ;/
                movlw   0                       ;\Add_H
                rcall   WrData_aPC              ;/
                movlw   1                       ;\nByte=1
                rcall   WrData_aPC              ;/
                movlw   0xD8                    ;\Codice 0xD8
                rcall   WrData_aPC              ;/
                rcall   WrCheckSum              ;
                

                ;----------------------------------------------------------------------------------
                ; Loop Ric Seriale
                ;----------------------------------------------------------------------------------
Loop_RxDaPC     clrf    StatoBuff               ;Libera Buffer Rx da PC in attesa nuovo frame
                ;...............................;
LRX_2           rcall   GetBufferFDTI
                tstfsz  WREG
                bra     LRX_2
                ;...............................; 
                ; Buffer FDTI Caricato          ;
                ;...............................;
                LBSFEL   Rx_Command,COM_LOOP,C_Loop
                LBSFEL   Rx_Command,COM_START,C_Start
                LBSFEL   Rx_Command,COM_READ_FLASH,C_ReadFlash
                LBSFEL   Rx_Command,COM_WRITE_FLASH,C_WriteFlash
                LBSFEL   Rx_Command,COM_READ_EE,C_ReadEE
                LBSFEL   Rx_Command,COM_WRITE_EE,C_WriteEE
                LBSFEL   Rx_Command,COM_READ_CONFIG,C_ReadCnf
                ;...............................;
                bra     Loop_RxDaPC

                ;----------------------------------------------------------------------------------
                ; Salta al punto di accesso zona riprogrammabile
                ;----------------------------------------------------------------------------------
C_Start         BSFNEL  Rx_nByte,1,Loop_RxDaPC
                rcall   Pop_RxFDTI              ;Codice 0xD8
                BSWNEL  0xD8,Loop_RxDaPC
                ;...............................;
                movlw   COM_START
                call    WrCmd_aPC
                movf    Add_L,w
                call    WrData_aPC
                movf    Add_H,w
                call    WrData_aPC
                movlw   1
                call    WrData_aPC
                movlw   0xD8                
                call    WrData_aPC
                call    WrCheckSum
                ;...............................;
CS_1            btfss   TXSTA,TRMT              ;\Attendi svuotamento buffer di
                bra     CS_1                    ;/trasmissione UART
                ;...............................;		
StartFirm       bcf     TXSTA,TXEN              ;Disabilita trasmissione
                bcf     RCSTA,CREN              ;Disabilita ricezione
                bcf     RCSTA,SPEN              ;Disattiva seriale
                ;...............................;
                goto    main_accesspoint        ;


                ;----------------------------------------------------------------------------------
                ; Test Loop
                ;----------------------------------------------------------------------------------
C_Loop          movlw   COM_LOOP
                call    WrCmd_aPC
                movf    Add_L,w
                call    WrData_aPC
                movf    Add_H,w
                call    WrData_aPC
                movf    Rx_nByte,w
                call    WrData_aPC
CTL_1           rcall   Pop_RxFDTI              ;
                rcall   WrData_aPC              ;
                decfsz  Rx_nByte                ;
                bra     CTL_1                   ;
                rcall   WrCheckSum              ;
                ;...............................;
                bra     Loop_RxDaPC

                ;----------------------------------------------------------------------------------
                ; Lettura di NumByte Byte FLASH
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando READ_FLASH            Comando READ_FLASH
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <1>                           <nByte>
                ; nByte                         Byte_1
                ;                               ... 
                ;                               Byte_n
                ;
                ;......................................................................
C_ReadFlash     clrf    TBLPTRU                 ;
                movf    Add_H,w                 ;
                movwf   TBLPTRH                 ;
                movf    Add_L,w                 ;
                movwf   TBLPTRL                 ;
                ;...............................;
                rcall   Pop_RxFDTI              ;Byte da Leggere
                movwf   Rx_nByte                ;(riutilizza la Rx_nByte che era a 1)
                ;...............................;
                movlw   COM_READ_FLASH          ;
                call    WrCmd_aPC               ;
                movf    Add_L,w                 ;
                rcall   WrData_aPC              ;
                movf    Add_H,w                 ;
                rcall   WrData_aPC              ;
                movf    Rx_nByte,w              ;
                rcall   WrData_aPC              ;
                ;...............................; Trasmetti DIM_WRITE_PAGE byte
RD_3            tblrd*+                         ;
                movf    TABLAT,w                ;
                rcall   WrData_aPC              ;
                decfsz  Rx_nByte                ;
                bra     RD_3                    ;
                ;...............................;
                rcall   WrCheckSum              ;
                bra     Loop_RxDaPC             ;Loop


                ;----------------------------------------------------------------------------------
                ; Scrittura Flash
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando WRITE_FLASH           Comando WRITE_FLASH
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <nByte>  0=256                <1>
                ; <b0>                          Byte_1 (write report)
                ; <b1> 
                ; ...
                ; <bn>                       
                ;......................................................................
C_WriteFlash    clrf    TBLPTRU                 ;
                movf    Add_H,w                 ;
                movwf   TBLPTRH                 ;
                movf    Add_L,w                 ;
                movwf   p_Low                   ;
                ;..............................................................
                ; Controllo se è esterno alla zona riprogrammabile
                ;..............................................................
                movlw   (INI_MEMRIP>>16)&0xFF
                subwf   TBLPTRU,w        
                skpz
                bra     Cp_2
                movlw   (INI_MEMRIP>>8)&0xFF
                subwf   TBLPTRH,w
                skpz
                bra     Cp_2
                movlw   INI_MEMRIP&0xFF
                subwf   p_Low,w        
Cp_2            skpnc
                bra     Cp_3                    ;salta se P>=INI_MEMRIP
                ;...............................;continua se P<INI_MEMRIP
                ; Risposta a mittente ERROR     ;
                ;...............................;
                movlw   WRITE_ADDRESS_ERROR     ;
                bra     WriteReport             ;
                
                ;..............................................................
                ; Copia pagina Flash nel buffer
                ;..............................................................
Cp_3            movf    p_Low,w                 ;\Posiziona puntatore flash 
                andlw   ~MASK_ERASE_PAGE        ;|all'inizio della pagina
                movwf   TBLPTRL                 ;/
                lfsr    1,BuffPagina            ;Puntatore all'inizio del buffer
                tblrd*-                         ;Decremento puntatore, dummy read                
                movlw   DIM_ERASE_PAGE
                movwf   Reg0
Cp_4            tblrd+*
                movff   TABLAT,POSTINC1
                decfsz  Reg0
                bra     Cp_4
                ;...............................;
                ; Modifica i dati nel buffer
                ;...............................; 
                clrf    CntLoadByte             ;Azzera contatore byte
                lfsr    1,BuffPagina            ;\
                movf    p_Low,w                 ;|Posiziona il puntatore buffer
                andlw   MASK_ERASE_PAGE         ;|sul primo byte da modificare
                addwf   FSR1L                   ;/
Cp_5            rcall   Pop_RxFDTI              ;\
                movwf   POSTINC1                ;|Modifica i Byte del buffer
                incf    CntLoadByte             ;/
                ;...............................;
                ; Controllo fine buffer
                ;...............................;
                movf    CntLoadByte,w
                addwf   p_Low,w
                andlw   MASK_ERASE_PAGE
                bz      C_Wr
                ;...............................;
                ; Ciclo su tutti i byte in arrivo
                ;...............................;
                decfsz  Rx_nByte                ;
                bra     Cp_5                    ;
                bra     C_Write                 ;

                ;----------------------------------------------------------------------------------
                ; Errore: i dati inviati sono oltre la erase_page
                ;----------------------------------------------------------------------------------
C_Wr            decf    Rx_nByte                ;\Salta se non ci sono più byte
                bz      C_Write                 ;/
                ;...............................;
                movlw   WRITE_FLASH_OVERDATA    ;\Errore al PC
                bra     WriteReport             ;/

                ;----------------------------------------------------------------------------------
                ; Scrittura:  Scrittura buffer su pagina Flash
                ;----------------------------------------------------------------------------------
C_Write         ;......................................................................
                ; Controllo se la scrittura della FLASH è necessaria, cioè se il
                ; Buffer è diverso dalla pagina della Flash
                ;......................................................................
                movlw   ~MASK_ERASE_PAGE        ;\Posiziona puntatore ad inizio
                andwf   TBLPTRL                 ;/della pagina
                lfsr    1,BuffPagina            ;Puntatore all'inizio del buffer
                tblrd*-                         ;Decremento puntatore, dummy read
                movlw   DIM_ERASE_PAGE          ;\Dimensione della pagina
                movwf   Reg0                    ;/
CWr_2           tblrd+*                         ;
                movf    TABLAT,w                ;\
                cpfseq  POSTINC1                ;|Salta se è diverso
                bra     CWr_3                   ;/
                decfsz  Reg0                    ;\Ciclo su tutta la pagina
                bra     CWr_2                   ;/
                ;..............................................................
                ; Non è necessaria la scrittura i byte caricati sul
                ; buffer corrispondono al contenuto attuale della flash
                ;..............................................................
                movlw   WRITE_FLASH_NO_NEED
                bra     WriteReport

                ;..............................................................
                ; *************  Processo di scrittura *********************    
                ;..............................................................
CWr_3           movlw   ~MASK_ERASE_PAGE        ;\Posiziona puntatore ad inizio
                andwf   TBLPTRL                 ;/pagina
                ;...............................; Cancella ERASE_PAGE
                bsf     EECON1,EEPGD            ;
                bcf     EECON1,CFGS             ;
                bsf     EECON1,WREN             ;
                bsf     EECON1,FREE             ;
                movlw   0x55                    ;
                movwf   EECON2                  ;
                movlw   0xAA                    ;
                movwf   EECON2                  ;
                bsf     EECON1,WR               ;Start erasing (CPU stall)
                bcf     EECON1,WREN             ;
                ;...............................;
                lfsr    1,BuffPagina            ;Puntatore all'inizio del buffer
                tblrd*-                         ;Decremento puntatore, dummy read
                ;..............................................................
                ; Ciclo di scrittura per tutte le Write_Page all'interno
                ; dell' Erase_Page
                ;..............................................................
                movlw   DIM_ERASE_PAGE/DIM_WRITE_PAGE
                movwf   Reg1
CWr_1           movlw   DIM_WRITE_PAGE
                movwf   Reg0
CWr_4           movff   POSTINC1,TABLAT
                tblwt+*
                decfsz  Reg0
                bra     CWr_4                
                ;...............................; Scrivi WRITE_PAGE
                bsf     EECON1,EEPGD            ;
                bcf     EECON1,CFGS             ;
                bsf     EECON1,WREN             ;
                bcf     EECON1,FREE             ;
                movlw   0x55                    ;
                movwf   EECON2                  ;
                movlw   0xAA                    ;
                movwf   EECON2                  ;
                bsf     EECON1,WR               ;Start programming (CPU stall)
                bcf     EECON1,WREN             ;
                ;...............................;
                decfsz  Reg1                    ;
                bra     CWr_1                   ;
                ;...............................;
                ; Verifica                      ;
                ;...............................;
                movlw   ~MASK_ERASE_PAGE        ;\Posiziona puntatore Flash
                andwf   TBLPTRL                 ;/all'inizio della pagina
                lfsr    1,BuffPagina            ;Puntatore all'inizio del buffer
                tblrd*-                         ;Decremento puntatore, dummy read
                movlw   DIM_ERASE_PAGE          ;\Dimensione della pagina
                movwf   Reg0                    ;/
CWr_5           tblrd+*                         ;
                movf    TABLAT,w                ;\
                cpfseq  POSTINC1                ;|Salta se è diverso
                bra     CWr_6                   ;/
                decfsz  Reg0                    ;\Ciclo su tutti i byte
                bra     CWr_5                   ;/
                ;...............................;
                ; Risposta a mittente OK        ;
                ;...............................;
CWr_7           movlw   WRITE_OK
                bra     WriteReport               
                ;...............................;
                ; Risposta a mittente ERROR     ;
                ;...............................;
CWr_6           movlw   WRITE_ERROR
                ;..................................................................................
                ; Risposta al PC comando WRITE FLASH
                ;..................................................................................
WriteReport     movwf   Reg0                    ;Salva TypeEvn in Reg0
                movlw   COM_WRITE_FLASH         ;\Rispondi a PC con comando: WRITE_FLASH
                rcall   WrCmd_aPC               ;/
                movf    Add_L,w                 ;\Add_L
                rcall   WrData_aPC              ;/
                movf    Add_H,w                 ;\Add_H
                rcall   WrData_aPC              ;/
                movlw   1                       ;\nByte=1
                rcall   WrData_aPC              ;/
                movf    Reg0,w                  ;\Report byte
                rcall   WrData_aPC              ;/
                rcall   WrCheckSum              ;
                bra     Loop_RxDaPC             ;Loop 



                ;----------------------------------------------------------------------------------
                ; Lettura EEPROM
                ;      -- DA CAN --                  -- A CAN --
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando READ_EE               Comando READ_EE
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <1>                           <nByte>
                ; nByte                         Byte_1
                ;                               ... 
                ;                               Byte_n
                ;   
                ;......................................................................
C_ReadEE        movf    Add_L,w                 ;
                movwf   EEADR                   ;
                movf    Add_H,w                 ;
                movwf   EEADRH                  ;
                rcall   Pop_RxFDTI
                movwf   Rx_nByte                ;Riutilizzo Rx_nByte che era 1                
                ;...............................;
                ; Invia Lettura EEPROM
                ;...............................;
                movlw   COM_READ_EE             ;\Rispondi a PC con comando: READ_EE
                rcall   WrCmd_aPC               ;/
                movf    Add_L,w                 ;\Add_L
                rcall   WrData_aPC              ;/
                movf    Add_H,w                 ;\Add_H
                rcall   WrData_aPC              ;/
                movf    Rx_nByte,w              ;\Rx_nByte
                rcall   WrData_aPC              ;/
                ;...............................;
                bcf     EECON1,EEPGD            ;\punta a memoria EEPROM
                bcf     EECON1,CFGS             ;/
                ;...............................;
CRDEE_2         bsf     EECON1,RD               ;Leggi da EEPROM
                movf    EEDATA,w                ;
                rcall   WrData_aPC              ;
                infsnz  EEADR                   ;
                incf    EEADRH                  ;
                decfsz  Rx_nByte                ;
                bra     CRDEE_2                 ;
                ;...............................;
                rcall   WrCheckSum              ;                
                ;...............................;
                bra     Loop_RxDaPC             ;Loop



                ;----------------------------------------------------------------------------------
                ; Scrittura in memoria EEPROM
                ;      -- DA CAN --                  -- A CAN --
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando WRITE_EE              Comando WRITE_EE
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <nByte>  0=256                <1>
                ; <b0>                          Byte_1 (write report)
                ; <b1> 
                ; ...
                ; <bn>   
                ;......................................................................
C_WriteEE       movf    Add_L,w                 ;
                movwf   EEADR                   ;
                movf    Add_H,w                 ;
                movwf   EEADRH                  ;
                ;...............................;
CWEE_1          rcall   Pop_RxFDTI              ;
                movwf   EEDATA                  ;
                movwf   Reg0                    ;
                bcf     EECON1,EEPGD            ;
                bcf     EECON1,CFGS             ;
                ;...............................;
                bsf     EECON1,WREN             ;Abilita scrittura in EEPROM
                movlw   0x55                    ;
                movwf   EECON2                  ;
                movlw   0xAA                    ;
                movwf   EECON2                  ;
                bsf     EECON1,WR               ;Start scrittura
                ;...............................;......................................
                btfsc   EECON1,WR               ;\Attendi fine scrittura     
                bra     $-2                     ;/
                bcf     EECON1,WREN             ;Disabilita scrittura in EEPROM
                ;...............................;
                bsf     EECON1,RD               ;Operazione di lettura
                movf    EEDATA,w                ;\Verifica scrittura 
                BSWNEF  Reg0,CWEE_3             ;/
                infsnz  EEADR                   ;Incrementa puntatore EEPROM
                incf    EEADRH                  ;
                ;...............................;
                decfsz  Rx_nByte                ;Ciclo su tutti i byte da scrivere
                bra     CWEE_1                  ;
                ;...............................;
                ; Risposta a mittente           ;
                ;...............................;
CWEE_2          movlw   WRITE_OK
                bra     WriteReportEE

                ;...............................;
                ; Risposta a mittente           ;
                ; ERRORE DI VERIFICA            ;
                ;...............................;
CWEE_3          movlw   WRITE_ERROR             ;
                ;..................................................................................
                ; Risposta al PC comando WRITE EEPROM
                ;..................................................................................
WriteReportEE   movwf   Reg0                    ;Salva TypeEvn in Reg0
                movlw   COM_WRITE_EE            ;\Rispondi a PC con comando: WRITE_EE
                rcall   WrCmd_aPC               ;/
                movf    Add_L,w                 ;\Add_L
                rcall   WrData_aPC              ;/
                movf    Add_H,w                 ;\Add_H
                rcall   WrData_aPC              ;/
                movlw   1                       ;\nByte=1
                rcall   WrData_aPC              ;/
                movf    Reg0,w                  ;\Report byte
                rcall   WrData_aPC              ;/
                rcall   WrCheckSum              ;
                bra     Loop_RxDaPC             ;Loop 

    CONSTANT    DIM_CONFIG_FRAME = 0x32
    if DIM_ERASE_PAGE > DIM_CONFIG_FRAME
                ;----------------------------------------------------------------------------------
                ; Lettura configurazione
                ;      -- DA CAN --                  -- A CAN --
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando COM_READ_CONFIG       Comando COM_READ_CONFIG
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <1>                           <nbyte>
                ; <cnf>                         cnf_b0
                ;                               cnt_b1
                ;                               ...
                ;                               cnf_bn
                ;......................................................................
C_ReadCnf       movlw   COM_READ_CONFIG         ;
                call    WrCmd_aPC               ;
                movf    Add_L,w                 ;
                rcall   WrData_aPC              ;
                movf    Add_H,w                 ;
                rcall   WrData_aPC              ;
                movlw   DIM_CONFIG_FRAME        ; Totale 0x32 = 50 byte
                rcall   WrData_aPC              ;
                ;...............................; Invia Intestazione Firmware 0x800-0x81F (32 byte)
                clrf    TBLPTRU                 ;
                movlw   0x08                    ;
                movwf   TBLPTRH                 ;
                movlw   0x00                    ;
                movwf   TBLPTRL                 ;
                movlw   0x20                    ;
                movwf   Reg0                    ;
CRC_1           tblrd*+                         ;
                movf    TABLAT,w                ;
                rcall   WrData_aPC              ;
                decfsz  Reg0                    ;
                bra     CRC_1                   ;
                ;...............................; Invia configuration Bit 0x300000 (16 byte)
                movlw   0x30
                movwf   TBLPTRU                 ;
                movlw   0x00                    ;
                movwf   TBLPTRH                 ;
                movlw   0x00                    ;
                movwf   TBLPTRL                 ;
                movlw   0x10                    ;
                movwf   Reg0                    ;
CRC_2           tblrd*+                         ;
                movf    TABLAT,w                ;
                rcall   WrData_aPC              ;
                decfsz  Reg0                    ;
                bra     CRC_2                   ;
                ;...............................; Invia Device ID  0x3FFFFE (2 byte)
                movlw   0x3F                    ;
                movwf   TBLPTRU                 ;
                movlw   0xFF                    ;
                movwf   TBLPTRH                 ;
                movlw   0xFE                    ;
                movwf   TBLPTRL                 ;
                tblrd*+                         ;
                movf    TABLAT,w                ;
                rcall   WrData_aPC              ;
                tblrd*                          ;
                movf    TABLAT,w                ;
                rcall   WrData_aPC              ;
                ;...............................;
                rcall   WrCheckSum              ;
                bra     Loop_RxDaPC             ;Loop
    endif

;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Invia evento al PC
;
; Frame UART <Evnt> <Add_L> <Add_H> <nByte=1> <TypeEvn> <CheckSum>;  (nByte 1-255, 0=256)
; 
; CheckSum = -{ <Evnt> + <Add_L> + <Add_H> + <nByte> + <TypeEvn>}
;
; Usa Reg:      Reg0    Stack:   2      FSR:    ---
;--------------------------------------------------------------------------------------------------
Evnt_aPC        movwf   Reg0                    ;Salva TypeEvn in Reg0
                movlw   EVENT                   ;\Rispondi a PC con comando: EVENT
                rcall   WrCmd_aPC               ;/
                movf    Add_L,w                 ;\Add_L
                rcall   WrData_aPC              ;/
                movf    Add_H,w                 ;\Add_H
                rcall   WrData_aPC              ;/
                movlw   1                       ;\nByte=1
                rcall   WrData_aPC              ;/
                movf    Reg0,w                  ;\TypeEvn = W
                rcall   WrData_aPC              ;/
                rcall   WrCheckSum              ;
                return


;--------------------------------------------------------------------------------------------------
; Deve essere richiamata in polling per ricevere byte da PC
;
;               call    GetBufferFDTI
; Ritorna       -W-     0:  Ricezione OK messaggio nel buffer
;                       1:  Ricezione in corso
;                       2:  Errore di Framing
;                       3:  Errore di overrun
;                       4:  Errore di TimeOut
;                       5:  Errore buffer non letto
;                       6:  Errore di CheckSum
;
;  L_1          call    GetBufferFDTI
;               tstfsz  WREG
;               bra     L_1
;               
;
; Usa Reg:      Reg0    Stack:   2
;--------------------------------------------------------------------------------------------------

GetBufferFDTI   ;...............................;
UL_1            BSC     S_00,UL_4               ; Salta se è in Stato 0
                ;...............................;
                ; Decrementa countdown          ;
                ;...............................;
                decfsz  Frame_TimeOut
                bra     UL_4
                decfsz  Frame_TimeOut+1
                bra     UL_4
                ;...............................;
                movf    RCREG,w                 ;Reset errore framing
                bcf     RCSTA,CREN              ;Reset errore overrun
                bsf     RCSTA,CREN              ;Riabilita seriale
                movlw   EVN_ERREP_TIMEOUT       ;-> Errore di TimeOut
                bra     UL_err                  ;
                ;...............................;
UL_4            btfss   PIR1,RCIF               ;\Controlla se è arrivato un byte
                bra     UL_7                    ;/
                BSS     RCSTA,FERR,UL_2         ;Errore di Framing (resettato leggendo RCREG)
                BSS     RCSTA,OERR,UL_3         ;Errore di Overrun (resettato con CREN=0)
                BSS     S_05,UL_5               ;Esci se il buffer è full (non è stato letto)
                movf    RCREG,w                 ;Leggi byte dalla UART
                rcall   Push_RxBuff             ;inserisci il byte nel buffer
                btfss   S_05                    ;verifica se il messaggio è completo
UL_7            retlw   1                       ;Return (1)-> Ricezione in corso
                tstfsz  RxChkSum                ;
                bra     UL_6                    ;
                retlw   0                       ;Return (0)-> Ricezione OK
UL_2            ;...............................; -> Gestione errore di framing (no stop bit detected)
                movf    RCREG,w                 ;Reset errore
                movlw   EVN_ERREP_FRAMING       ;-> Errore di Framing
                bra     UL_err                  ;
                ;...............................; -> Gestione errore di overrun (byte ricevuto con RCREG non letto)
UL_3            bcf     RCSTA,CREN              ;Reset errore
                bsf     RCSTA,CREN              ;Riabilita seriale
                movlw   EVN_ERREP_OVERRUN       ;-> Errore di overrun
                bra     UL_err                  ;
                ;...............................; -> Gestione errore di buffer non letto
UL_5            movlw   EVN_ERREP_BUFFULL       ;-> Errore buffer non letto
                bra     UL_err                  ;
                ;...............................;
UL_6            movlw   EVN_ERREP_CHKSUM        ;-> Errore di CheckSum               
UL_err          rcall   Evnt_aPC                ;
                clrf    StatoBuff               ;
                retlw   1                       ;Return (1)-> Ricezione in corso


;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Inserisce un byte nel buffer di ricezione da PC
;               Il buffer è lineare da DIM_BUFF_FDTI byte massimi
;               La struttura dati che il buffer si aspetta è coì formata:
;
; Struttura dati ricevuti da PC
;
; Frame UART <Comando> <Add_L> <Add_H> <nByte> <b0> <b1> .. <bn> <CheckSum>;  (nByte 1-255, 0=256)
; 
; CheckSum = -{ <Comando> + <Add_L> + <Add_H> + <nByte> + <b0> + <b1> + .. + <bn> }
;
;               Il byte di checksum viene memorizzato nel buffer dopo l'ultimo byte
;
; Chiamata:     -set-   W
;               call    Push_RxFDTI
;               Il flag F_RxMsgCmpl viene settato quando il messaggio è stato completato
;               e viene azzerato quando è stato letto e interpretato
;
; Usa Reg:      Reg0    Stack:   1
;--------------------------------------------------------------------------------------------------
        CONSTANT    SET_TIMEOUT     = 21        ;Time out frame 6mSec (un frame da 256 1,3mSec

Push_RxBuff     BSS     S_00,PRFT_1             ;
                movwf   Rx_Command              ;Leggi <Comando> -> Rx_Command
                movwf   RxChkSum                ;
                bsf     S_00                    ;
                clrf    p_rx                    ;Predispone alla scrittura del buffer
                ;...............................;Start contatore timeout
                clrf    Frame_TimeOut           ;
                movlw   SET_TIMEOUT             ;
                movwf   Frame_TimeOut+1         ;
                bra     PRFT_end                ;
                ;...............................;
PRFT_1          BSS     S_01,PRFT_2             ;
                movwf   Add_L                   ;Leggi <Add_L> -> Add_L
                addwf   RxChkSum                ;
                bsf     S_01                    ;
                bra     PRFT_end                ;
                ;...............................;
PRFT_2          BSS     S_02,PRFT_3             ;
                movwf   Add_H                   ;Leggi <Add_H> -> Add_H
                addwf   RxChkSum                ;
                bsf     S_02                    ;
                bra     PRFT_end                ;
                ;...............................;
PRFT_3          BSS     S_03,PRFT_4             ;
                movwf   Rx_nByte                ;Leggi <nByte> -> Rx_nByte   (0 = 256 byte)
                addwf   RxChkSum                ;
                bsf     S_03                    ;
                bra     PRFT_end                ;
                ;...............................;
PRFT_4          BSS     S_04,PRFT_5             ;
                movwf   Reg0                    ;Leggi <b0> + <b1> + .. + <bn>
                lfsr    1,BuffRx                ;
                movf    p_rx,w                  ;puntatore scrittura
                addwf   FSR1L                   ;indirizzo di scrittura dati
                movf    Reg0,w                  ;
                addwf   RxChkSum                ;
                movwf   INDF1                   ;scrive il dato sul buffer
                incf    p_rx                    ;incrementa puntatore
                SSFEF   p_rx,Rx_nByte           ;Per tutti i byte contenuti nel messaggio
                bra     PRFT_end                ;
                bsf     S_04                    ;
                bra     PRFT_end                ;
                ;...............................;
                ; Messaggio completato          ;
                ;...............................;
PRFT_5          addwf   RxChkSum                ;
                clrf    p_rx                    ;Predispone alla lettura del buffer
                bsf     S_05                    ;
                ;...............................;
PRFT_end        return


;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Estrae un byte nel buffer di ricezione da FTDI
;               Il buffer è lineare e occupa al massimo 67 byte
;
; Chiamata:     call    Pop_RxFDTI
;
; Usa Reg:      ---     Stack:   1      FSR:    2
;--------------------------------------------------------------------------------------------------
Pop_RxFDTI      BSC     S_05,PopRS1_end         ;Esci se il buffer NON è caricato
                lfsr    2,BuffRx                ;
                movf    p_rx,w                  ;puntatore scrittura
                addwf   FSR2L                   ;indirizzo di scrittura dati
                incf    p_rx                    ;incrementa puntatore
                movf    INDF2,w                 ;legge il dato dalla coda
                ;...............................;
PopRS1_end      return


;--------------------------------------------------------------------------------------------------
; Invia dati al PC
; - Attende che si liberi il buffer di trasmissione UART
; - Calcola il CheckSum
;
; Usa Reg:      ---     Stack:   1      FSR:    ---
;--------------------------------------------------------------------------------------------------
WrCmd_aPC       clrf    TxChkSum        ;\Azzera CheckSum
                bra     WrData_aPC      ;/
                ;.......................;
WrCheckSum      movf    TxChkSum,w      ;\Calcola CheckSum
                sublw   0               ;/
                ;.......................;
WrData_aPC      addwf   TxChkSum        ;Calcola CheckSum (azzerato quando esegue WrCheckSum)
Wait_TXaPC      btfss   PIR1,TXIF       ;\Attendi Trasmettitore UART libero
                bra     Wait_TXaPC      ;/
                movwf   TXREG           ;Trasmetti byte
                return


;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Inizializzazione delle variabili della seriale
;               Questa SubRuotine viene richiamata in modalità UpGrade
;
; Chiamata:     rcall   Ini_Seriale
;
; Usa:          Reg:    ---     
;--------------------------------------------------------------------------------------------------
IniSerBoot      bcf     PIE1,RCIE               ;\Disabilita Interruzioni UART
                bcf     PIE1,TXIE               ;/
                bcf     ANSELH,ANS11            ;Ingresso RX digitale
                ;...............................;
                bcf     BAUDCON,BRG16           ; 0 = 8-bit Baud Rate Generator
                bsf     TXSTA,BRGH              ; 1 = high speed
                movlw   d'1'                    ; 2Mbaud con fc=64Mhz
                movwf   SPBRG                   ; 
                bsf     RCSTA,SPEN              ;Abilita pin Tx e Rx
                bsf     TXSTA,TXEN              ;Abilita trasmissione
                bsf     RCSTA,CREN              ;Abilita ricezione
                ;...............................;
                bcf     PIR1,RCIF               ;\reset flag d'interrupt
                bcf     PIR1,TXIF               ;/
                ;...............................;
                clrf    TxChkSum                ;
                return                          ;
 


;==============================================================================
;
;       RUOTINE DI RITARDO
;
;==============================================================================

;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Rit_d
;
; Operazione:   RUOTINE DI RITARDO in decimi di secondo a 20MHz
;
; Chiamata:     movlw   d'x'            ;x = decimi di secondo da 1 a 255
;               call    Rit_d           ;
;
; Chiamata da:  ram_0   Lavora in:      prom_0          Stack:   2
; Ritorna:      ram_0   Usa Reg:        0,1,2,3            
;--------------------------------------------------------------------------------------------------
Rit_d1  movlw   d'1'
Rit_d   movwf   Reg3
Rit_2   movlw   d'100'
        rcall   Rit_m
        decfsz  Reg3
        bra     Rit_2
        return
 

;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Rit_m,Rit_1m
;
; Operazione:   RUOTINE DI RITARDO in millisecondi a 20MHz
;
; Chiamata:     movlw   d'x'            ;x = millisecondi da 2 a 255
;               call    Rit_m           ;
;
;               call    Rit_1m          ;1 millisecondo
;
; Chiamata da:  ram_0   Lavora in:      prom_0          Stack:   1
; Ritorna:      ram_0   Usa Reg:        0,1,2            
;--------------------------------------------------------------------------------------------------
Rit_m   movwf   Reg2                    ;\Imposta loop Esterno
        bra     Rit_1                   ;/
        ;...............................;
Rit_1m  movlw   d'1'                    ;
        movwf   Reg2                    ;
        ;...............................;
Rit_1   movlw   d'21'                   ;\
        movwf   Reg1                    ;|
        movlw   d'196'                  ;|
        movwf   Reg0                    ;|Ritardo di 1.040 mSec a 64Mhz
Rit_0   decfsz  Reg0                    ;|
        bra     Rit_0                   ;|
        decfsz  Reg1                    ;|
        bra     Rit_0                   ;/
        ;...............................;
        decfsz  Reg2                    ;\EndFor Loop Esterno
        bra     Rit_1                   ;/
        return                          ;



    end