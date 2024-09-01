                TITLE	"Emulatore EP"
;******************************************************************************
; Emulatore EPROM 
;
;
; ATTENZIONE non usare le istruzioni movff x,BSR  movff x,WREG  movff x,STATUS
;            con fast return attivato a causa di un bug sul PIC18Fxxxx
;
;******************************************************************************

    LIST            n=0,w=0,st=Off,r=dec
    PROCESSOR       18F14K22
    RADIX           dec

 if TEST == 1
    #DEFINE         MODELLO              "Test"
 else
    #DEFINE         MODELLO              "Emulatore_EP"
 endif
    
    CONSTANT        MAIN_RELEASE   = 2   ;Mainnumber release  (AddOn)
    CONSTANT        BUG_CORRECTION = 05  ;Subnumber release   (bug correction)
    CONSTANT        PIC_TYPE       = b'00100000011'    ;0x103 259d

    global          int_accesspoint,main_accesspoint

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
    CONSTANT        COM_RESET = 8
    CONSTANT        COM_READ_RAM = 9
    CONSTANT        COM_WRITE_RAM = 10
    CONSTANT        COM_SET_COMMAND = 11

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
    #define F_A16_H         Flag0,0 ;Attivo banco RAM High
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
    
    #define DataBusDIR  TRISC
    #define DataBusIN   PORTC
    #define DataBusOUT  LATC
    

    #define CPU_RESET   LATA,RA2
    #define CNT_CLR     LATA,RA4
    #define CNT_CLK_A16 LATA,RA5
    #define RAM_WR      LATB,RB4
    #define RAM_OE      LATB,RB6
        


;----------------------------------------------------------------------------
;  Definizioni  ACCESS Ram
;  Queste locazioni si sovrappongono a quelle del BootLoader
;----------------------------------------------------------------------------
Ram_acs_ovr     access_ovr
;..................................................................................................
; Registri di uso generico
;..................................................................................................
Reg0            res 1       ;
Reg1            res 1       ;
Reg2            res 1       ;
Reg3            res 1       ;
Reg4            res 1       ;
Reg5            res 1       ;
iReg0           res 1       ;
iReg1           res 1       ;
Flag0           res 1       ;
Flag1           res 1       ;
Flag2           res 1       ;
Flag3           res 1       ;
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
AddCnt          res 2       ; Indirizzo Contatori

;----------------------------------------------------------------------------
; Banco 0x100   accessibile perchè nel boot è stato settato BRG=1
;----------------------------------------------------------------------------
Banco100        udata_ovr   0x100
;...........................;
BuffRx          res  256    ; 


;==============================================================================
;                        Inizio zona riprogrammabile.
; >>>>>  NON MODIFICARE LA POSIZIONE ASSOLUTA DEGLI ACCESSPOINT <<<<<<<<<<<
;==============================================================================
version_code    code    0x800
                dw      MAIN_RELEASE
                dw      BUG_CORRECTION
                dw      PIC_TYPE
                data    MODELLO,0               ;Stringa terminata con 0
                ;...............................;
access_code     code    0x820
int_accesspoint 
                goto    Int_routine
main_accesspoint
                goto    Init
;==================================================================================================
;               >>>>>  NON MODIFICARE LA POSIZIONE ASSOLUTA DEGLI ACCESSPOINT <<<<<
;==================================================================================================





;**************************************************************************************************
; INTERRUPT:    RUOTINE ASSOCIATA ALL'INTERRUPT
;
;               1) Allo scadere del timer 0
;               2) All'arrivo di un byte dalla seriale (MIDI IN)
;               ATTENZIONE usa salvataggio STATUS,BSR,WREG automatico
;               NON!! usare istruzione movff con questi registri
;
; Livelli di Stack occupati:   3
;**************************************************************************************************

Int_routine     
                ;...............................;
Int_check       


                ;...............................;
                retfie  1                       ;


;**************************************************************************************************
; Main code
;**************************************************************************************************
Init    clrf    Flag0
        clrf    Flag1
        clrf    Flag2
        clrf    Flag3

;==================================================================================================
;               >>>>> INIZIALIZZAZIONI <<<<<
;==================================================================================================
                
        movlw   b'00000011'     ;\
        movwf   ANSEL           ;|Solo AN0 e AN1 attivi come ingressi analogici
        clrf    ANSELH          ;/
                        
        setf    DataBusDIR      ;Bus Dati IN
        
        bsf     CPU_RESET       ;\Reset CPU Attivo
        bcf     TRISA,RA2       ;/
        
        bcf     CNT_CLR         ;\Contatori in reset
        bcf     TRISA,RA4       ;/

        bcf     CNT_CLK_A16     ;\Clock basso A16=0
        bcf     TRISA,RA5       ;/                
        
        bsf     RAM_WR          ;\RAM in lettura
        bcf     TRISB,RB4       ;/
        
        bsf     RAM_OE          ;\RAM in HiZ
        bcf     TRISB,RB6       ;/

        rcall   ResetCnt


;        ;...............................;
;        ; Ini Timer 0                   ;
;        ;...............................;
;        ;          +--> 1=Timer0 8-Bit
;        ;          |   +++--> Prescaler  001=1:4
;        movlw   b'01000001'             ;1
;        movwf   T0CON
;        bcf     INTCON,TMR0IF
;        bsf     INTCON,TMR0IE           ;abilita int timer 0
;        bsf     T0CON,TMR0ON            ;Avvia timer0
;        clrf    TMR0L
        
        
;        bsf     INTCON,PEIE
;        bsf     INTCON,GIE


        rcall   IniSerMain

;==================================================================================================
; Main Loop    (dopo 330mSec)
;==================================================================================================
Main_Loop
                ;----------------------------------------------------------------------------------
                ; Loop Ric Seriale
                ;----------------------------------------------------------------------------------
Loop_RxDaPC     clrf    StatoBuff               ;Libera Buffer Rx da PC in attesa nuovo frame
                ;...............................;
LRX_2           rcall   GetBufferFDTI
                tstfsz  WREG
                bra     LRX_2
                ;..................................................................................
                ; Buffer FDTI Caricato: Parser Comandi
                ;..................................................................................
                LBSFEL   Rx_Command,COM_LOOP,C_Loop
                LBSFEL   Rx_Command,COM_RESET,C_Reset
                LBSFEL   Rx_Command,COM_SET_COMMAND,C_SetCommand
                BSC      CPU_RESET,PRS_end   ;------------------Comandi attivi solo se in Reset CPU
                LBSFEL   Rx_Command,COM_READ_RAM,C_ReadRam
                LBSFEL   Rx_Command,COM_WRITE_RAM,C_WriteRam
                ;..................................................................................
PRS_end         bra     Loop_RxDaPC

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
                ; Reset per entrare in Bootloader
                ;----------------------------------------------------------------------------------
C_Reset         reset                                     

                ;----------------------------------------------------------------------------------
                ; Set Command
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando SET_COMMAND           Comando SET_COMMAND
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <2>                           <1>
                ; <Command>                     <report>
                ; <parametro>
                ;..................................................................................
C_SetCommand    rcall   Pop_RxFDTI              ; --> Leggi <Command>
                movf    WREG,w                  ;\             
                bz      CSC_0                   ;| Parsing <Command>
                decf    WREG                    ;|
                bz      CSC_1                   ;/
                movlw   0xFF                    ;\Ritorna Errore comando sconosciuto
                bra     SetReport               ;/
                ;...............................; 0) Reset/Run CPU
CSC_0           rcall   Pop_RxFDTI              ; --> Leggi <Parametro>
                rcall   ResetRunCPU             ; 0)Reset CPU 1)Run CPU
                movlw   0                       ;\Ritorna <Command>
                bra     SetReport               ;/
                ;...............................; 1) Setta Banco RAM
CSC_1           rcall   Pop_RxFDTI              ; --> Leggi <Parametro>
                rcall   SetBancoRAM             ; 0)BancoBasso 1)Bancoalto
                movlw   1                       ;\Ritorna <Command>
                bra     SetReport               ;/
                ;..................................................................................
                ; Risposta al PC comando SET_COMMAND
                ;..................................................................................
SetReport       movwf   Reg0                    ;Salva TypeEvn in Reg0
                movlw   COM_SET_COMMAND         ;\Rispondi a PC con comando: WRITE_RAM
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
                ;...............................;
                bra     Loop_RxDaPC             ;Loop

                ;----------------------------------------------------------------------------------
                ; Lettura di NumByte Byte RAM
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando READ_RAM              Comando READ_RAM
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <1>                           <nByte>
                ; nByte                         Byte_1
                ;                               ... 
                ;                               Byte_n
                ;
                ;..................................................................................
C_ReadRam       call    GoToAddress             ;
                ;...............................;
                rcall   Pop_RxFDTI              ;Byte da Leggere
                movwf   Rx_nByte                ;(riutilizza la Rx_nByte che era a 1)
                ;...............................;
                movlw   COM_READ_RAM            ;
                call    WrCmd_aPC               ;
                movf    Add_L,w                 ;
                rcall   WrData_aPC              ;
                movf    Add_H,w                 ;
                rcall   WrData_aPC              ;
                movf    Rx_nByte,w              ;
                rcall   WrData_aPC              ;
                ;...............................; Trasmetti Rx_nByte Ram byte
                bcf     RAM_OE                  ;RAM in lettura
                nop                             ;
CRR_1           movf    DataBusIN,w             ;
                rcall   WrData_aPC              ;
                rcall   IncCntAdd               ;Incrementa contatore
                decfsz  Rx_nByte                ;
                bra     CRR_1                   ;
                bsf     RAM_OE                  ;RAM in HiZ
                ;...............................;
                rcall   WrCheckSum              ;
                bra     Loop_RxDaPC             ;Loop

                ;----------------------------------------------------------------------------------
                ; Scrittura di NumByte Byte RAM
                ;----------------------------------------------------------------------------------
                ; PC --> PIC                    PIC --> PC
                ;
                ; Comando WRITE_RAM             Comando WRITE_RAM
                ; Add_L                         Add_L 
                ; Add_H                         Add_H 
                ; <nByte>  0=256                <1>
                ; <b0>                          Byte_1 (write report)
                ; <b1> 
                ; ...
                ; <bn>                       
                ;..................................................................................
C_WriteRam      BSS     CPU_RESET,CWF_0         ;
                movlw   WRITE_ERROR             ;
                bra     WriteReport             ;
                ;...............................;
CWF_0           call    GoToAddress             ;
                bsf     RAM_OE                  ;\Direzione Bus OUT PIC
                clrf    DataBusDIR              ;/
CWF_1           rcall   Pop_RxFDTI              ;
                movwf   DataBusOUT              ;
                bcf     RAM_WR                  ;\
                nop                             ;|Impulso di scrittura
                bsf     RAM_WR                  ;/
                rcall   IncCntAdd               ;Incrementa contatore
                decfsz  Rx_nByte                ;
                bra     CWF_1                   ;
                setf    DataBusDIR              ; Bus in HiZ
                ;...............................;
                movlw   WRITE_OK                ;
                ;..................................................................................
                ; Risposta al PC comando WRITE RAM
                ;..................................................................................
WriteReport     movwf   Reg0                    ;Salva TypeEvn in Reg0
                movlw   COM_WRITE_RAM           ;\Rispondi a PC con comando: WRITE_RAM
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
                ;...............................;
                bra     Loop_RxDaPC             ;Loop
                
;==================================================================================================
; END Main Loop
;==================================================================================================
                bra     Main_Loop
                
                

;##################################################################################################
;
;                                     ZONA SUBRUOTINE
;
;##################################################################################################

;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Reset/Run CPU
;
; Chiamata:     -set-   W       W=0 Reset CPU  W=1 Run CPU
;               call    ResetRunCPU
;
; Usa Reg:      0,1,2   Stack:   1      FSR:    ---
;--------------------------------------------------------------------------------------------------
ResetRunCPU     setf    DataBusDIR      ; Bus in HiZ
                bsf     RAM_WR          ; Write line disabilitata
                BSC     WREG,0,RRC_1    ; if (Wreg,0)
                ;.......................; =1) --> Run CPU
                bcf     RAM_OE          ; Abilita uscite RAM
                bcf     CPU_RESET       ;
                return                  ;
RRC_1           ;.......................; =0) --> Reset CPU
                bsf     CPU_RESET       ;
                bsf     RAM_OE          ; DisAbilita uscite RAM
                movlw   100             ;
                rcall   Rit_m           ;
                return                  ;

;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Imposta Banco RAM di lavoro
;
; Chiamata:     -set-   W       W=0 BancoBasso W=1 BancoAlto
;               call    SetBancoRAM
;
; Usa Reg:      ---     Stack:   1      FSR:    ---
;--------------------------------------------------------------------------------------------------
SetBancoRAM     BSC     WREG,0,SBR_1    ; if (Wreg,0)
                ;.......................; =1) --> Banco alto A16=1
                bsf     F_A16_H         ;
                bsf     CNT_CLK_A16     ;
                bra     SBR_2           ;
                ;.......................; =0) --> Banco basso A16=0
SBR_1           bcf     F_A16_H         ;
                bcf     CNT_CLK_A16     ;
SBR_2           ;.......................;
;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Incrementa contatore
;
; Chiamata:     -set-   F_A16_H
;               call    IncCntAdd
;
; Usa Reg:      0,1     Stack:   1      FSR:    ---
;--------------------------------------------------------------------------------------------------
ResetCnt        bcf     CNT_CLR         ;Azzera Contatori
                setf    AddCnt          ;\AddCnt = -1
                setf    AddCnt+1        ;/
                bsf     CNT_CLR         ;Abilita contatori al conteggio
                ;.......................;
IncCntAdd       BSS     F_A16_H,IC_1    ; if (F_A16_H)     _
                ;.......................; =0)           __| |__  A16=0
                bsf     CNT_CLK_A16     ;\
                nop                     ;|Incrementa contatore
                bcf     CNT_CLK_A16     ;/
                bra     IC_2            ;               __   __
                ;.......................; =1)             |_|    A16=1       
IC_1            bcf     CNT_CLK_A16     ;\
                nop                     ;|Incrementa contatore
                bsf     CNT_CLK_A16     ;/
                ;.......................;
IC_2            incfsz  AddCnt          ;
                return                  ;
                incf    AddCnt+1        ;
                return

;--------------------------------------------------------------------------------------------------
; SUBRUOTINE:   Setta indirizzo Add sui contatori (max 86mSec@64Mhz)
;
; Chiamata:     -set-   Add_L,Add_H
;               call    SetAddress
;
; Usa Reg:      0,1     Stack:   1      FSR:    ---
;--------------------------------------------------------------------------------------------------
GoToAddress     movf    Add_H,w         ;\High
                subwf   AddCnt+1,w      ;/
                bnz     GTA_1           ;
                movf    Add_L,w         ;\Low
                subwf   AddCnt,w        ;/
GTA_1           ;.......................;
                skpnz                   ;
                return                  ;
                ;.......................;
                bc      GTA_2           ;Salta se AddCnt > Add
                rcall   IncCntAdd       ;
                bra     GoToAddress     ;
                ;.......................;
GTA_2           rcall   ResetCnt        ;Reset in uscita e '1' in cnt
                bra     GoToAddress     ;


        


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
IniSerMain      bcf     PIE1,RCIE               ;\Disabilita Interruzioni UART
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




