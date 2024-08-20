;------------------------------------------------------------------------------
; GESTIONE EEPROM INTERNA AL PIC 16C877
;------------------------------------------------------------------------------


;------------------------------------------------ ROM_0 -----------------------
                VARIABLE        PC_ROM_0=$
;------------------------------------------------ EEPROM ----------------------

;------------------------------------------------------------------------------
; PROGRAMMAZIONE INIZIALE MEMORIA EEPROM
; Valori registrati in fase di programmazione del PIC
;------------------------------------------------------------------------------
       
;^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ RIGA 0 ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
EE_Flags_0              set EE_PREFERENCE + 0   ; EE_Flag_0 xxxxxx00
;--------------------  Definizione Flag_0  ------------------------------------
F_STR_INI       equ     0
F_FUN_MANTICE   equ     1
F_ACCORDATURA   equ     2
F_RADIO         equ     3
F_RUNSTATUS_IN  equ     4
F_RUNSTATUS_OUT equ     5
F_POT_OFF       equ     6
F_FUNZ_FISA     equ     7

;-------------------- Fine definizione Flag_0 ---------------------------------
F_TRS_DIA       equ     0


EE_Flags_1              set EE_PREFERENCE + 1   ; EE_Flag_1
;--------------------  Definizione Flag_1  ------------------------------------

;-------------------- Fine definizione Flag_1 ---------------------------------

EE_Ch_Treble_a          set EE_PREFERENCE + 2   ; Canale del TREBLE A
EE_Ch_Treble_b          set EE_PREFERENCE + 3   ; Canale del TREBLE B
EE_Ch_Chord_a           set EE_PREFERENCE + 4   ; Canale degli ACCORDI A
EE_Ch_Chord_b           set EE_PREFERENCE + 5   ; Canale degli ACCORDI B
EE_Ch_Bass_a            set EE_PREFERENCE + 6   ; Canale delle BASSI A
EE_Ch_Bass_b            set EE_PREFERENCE + 7   ; Canale delle BASSI B

;^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ RIGA 1 ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
EE_Ch_TreblePC_b        set EE_PREFERENCE + 8   ; Canale del TREBLE b (solo PC Direct)
EE_SogliaBellow         set EE_PREFERENCE + 9   ; SogliaMantice
EE_Cancel               set EE_PREFERENCE + 10  ; Cancel =    xxxx0000
EE_TrebleVelocity_a     set EE_PREFERENCE + 11  ; TrebleVelocity
EE_TrebleVelocity_b     set EE_PREFERENCE + 12  ; TrebleVelocity
EE_ChordVelocity        set EE_PREFERENCE + 13  ; ChordVelocity
EE_BassVelocity         set EE_PREFERENCE + 14  ; BassVelocity
EE_Traspose             set EE_PREFERENCE + 15  ; Traspose SemiToni

;^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ RIGA 2 ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
EE_PitchDelta           set EE_PREFERENCE + 16  ; Profondità di Pitch Bend
EE_NumPitch             set EE_PREFERENCE + 17  ; Durata di Pitch Bend
EE_T_Plungo             set EE_PREFERENCE + 18  ; Tempo per pressione prolungata
EE_Ch_Treble_ba         set EE_PREFERENCE + 19  ; Canale del TREBLE B su A
EE_Ch_TreblePC_ba       set EE_PREFERENCE + 20  ; Canale del TREBLE b (solo PC Direct) su a
EE_Righe_NC             set EE_PREFERENCE + 21  ; Righe con sensori Normalmente Chiusi


;******************************************************************************
; Banco EEPROM 0 padded
;******************************************************************************
                org 0xF00000
EE_CHORD_PRESET
         ;    Banco PrgCh   Flag
         ;       |   |      |
         ;---------------------------- BANCO 0 --------------------------------
        db      000,001,b'00000000',0  ;Preset 00
        db      000,002,b'00000000',0  ;Preset 01
        db      000,003,b'00000000',0  ;Preset 02
        db      000,004,b'00000000',0  ;Preset 03
         ;---------------------------- BANCO 1 --------------------------------
        db      001,001,b'00000000',0  ;Preset 04
        db      001,002,b'00000000',0  ;Preset 05
        db      001,003,b'00000000',0  ;Preset 06
        db      001,004,b'00000000',0  ;Preset 07
         ;---------------------------- BANCO 2 --------------------------------
        db      002,001,b'00000000',0  ;Preset 08
        db      002,002,b'00000000',0  ;Preset 09
        db      002,003,b'00000000',0  ;Preset 10
        db      002,004,b'00000000',0  ;Preset 11
         ;---------------------------- BANCO 3 --------------------------------
        db      003,001,b'00000000',0  ;Preset 12
        db      003,002,b'00000000',0  ;Preset 13
        db      003,003,b'00000000',0  ;Preset 14
        db      003,004,b'00000000',0  ;Preset 15

EE_BASS_PRESET
         ;    Banco PrgCh   Flag
         ;       |   |      |
         ;---------------------------- BANCO 0 --------------------------------
        db      004,001,b'00000000',0  ;Preset 00
        db      004,002,b'00000000',0  ;Preset 01
        db      004,003,b'00000000',0  ;Preset 02
        db      004,004,b'00000000',0  ;Preset 03
         ;---------------------------- BANCO 1 --------------------------------
        db      005,001,b'00000000',0  ;Preset 04
        db      005,002,b'00000000',0  ;Preset 05
        db      005,003,b'00000000',0  ;Preset 06
        db      005,004,b'00000000',0  ;Preset 07
         ;---------------------------- BANCO 2 --------------------------------
        db      006,001,b'00000000',0  ;Preset 08
        db      006,002,b'00000000',0  ;Preset 09
        db      006,003,b'00000000',0  ;Preset 10
        db      006,004,b'00000000',0  ;Preset 11
         ;---------------------------- BANCO 3 --------------------------------
        db      007,001,b'00000000',0  ;Preset 12
        db      007,002,b'00000000',0  ;Preset 13
        db      007,003,b'00000000',0  ;Preset 14
        db      007,004,b'00000000',0  ;Preset 15


EE_PREFERENCE
        db      0x10,0x00,0x00,0x01,0x02,0x03,0x04,0x05 ;RIGA 0
        db      0x06,0x04,0x00,0x7F,0x7F,0x7F,0x7F,0x00 ;RIGA 1
        db      0x10,0x10,0x08,0x07,0x08,0x00,0x00,0x00 ;RIGA 2
        db      0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00 ;RIGA 3
        

;******************************************************************************
; Banco EEPROM 1
;******************************************************************************
                org 0xF00100               
  
EE_TRB_PRESET_A
         ;---------------------------- BANCO 0 --------------------------------
         ;    Banco PrgCh   Flag
         ;       |   |      |
        db      000,000,b'00000000',0  ;Preset 00
        db      000,001,b'00000000',0  ;Preset 01
        db      000,002,b'00000000',0  ;Preset 02
        db      000,003,b'00000000',0  ;Preset 03
        db      000,004,b'00000000',0  ;Preset 04
        db      000,005,b'00000000',0  ;Preset 05
        db      000,006,b'00000000',0  ;Preset 06
        db      000,007,b'00000000',0  ;Preset 07
        db      000,008,b'00000000',0  ;Preset 08
        db      000,009,b'00000000',0  ;Preset 09
        db      000,010,b'00000000',0  ;Preset 10
        db      000,011,b'00000000',0  ;Preset 11
        db      000,012,b'00000000',0  ;Preset 12
        db      000,013,b'00000000',0  ;Preset 13
        db      000,014,b'00000000',0  ;Preset 14
        db      000,015,b'00000000',0  ;Preset 15

         ;---------------------------- BANCO 1 --------------------------------
         ;    Banco PrgCh   Flag
         ;       |   |      |
        db      001,000,b'00000000',0  ;Preset 00
        db      001,001,b'00000000',0  ;Preset 01
        db      001,002,b'00000000',0  ;Preset 02
        db      001,003,b'00000000',0  ;Preset 03
        db      001,004,b'00000000',0  ;Preset 04
        db      001,005,b'00000000',0  ;Preset 05
        db      001,006,b'00000000',0  ;Preset 06
        db      001,007,b'00000000',0  ;Preset 07
        db      001,008,b'00000000',0  ;Preset 08
        db      001,009,b'00000000',0  ;Preset 09
        db      001,010,b'00000000',0  ;Preset 10
        db      001,011,b'00000000',0  ;Preset 11
        db      001,012,b'00000000',0  ;Preset 12
        db      001,013,b'00000000',0  ;Preset 13
        db      001,014,b'00000000',0  ;Preset 14
        db      001,015,b'00000000',0  ;Preset 15

EE_TRB_PRESET_B
         ;---------------------------- BANCO 0 --------------------------------
         ;    Banco PrgCh   Flag
         ;       |   |      |
        db      002,000,b'00000000',0  ;Preset 00
        db      002,001,b'00000000',0  ;Preset 01
        db      002,002,b'00000000',0  ;Preset 02
        db      002,003,b'00000000',0  ;Preset 03
        db      002,004,b'00000000',0  ;Preset 04
        db      002,005,b'00000000',0  ;Preset 05
        db      002,006,b'00000000',0  ;Preset 06
        db      002,007,b'00000000',0  ;Preset 07
        db      002,008,b'00000000',0  ;Preset 08
        db      002,009,b'00000000',0  ;Preset 09
        db      002,010,b'00000000',0  ;Preset 10
        db      002,011,b'00000000',0  ;Preset 11
        db      002,012,b'00000000',0  ;Preset 12
        db      002,013,b'00000000',0  ;Preset 13
        db      002,014,b'00000000',0  ;Preset 14
        db      002,015,b'00000000',0  ;Preset 15

         ;---------------------------- BANCO 1 --------------------------------
         ;    Banco PrgCh   Flag
         ;       |   |      |
        db      003,000,b'00000000',0  ;Preset 00
        db      003,001,b'00000000',0  ;Preset 01
        db      003,002,b'00000000',0  ;Preset 02
        db      003,003,b'00000000',0  ;Preset 03
        db      003,004,b'00000000',0  ;Preset 04
        db      003,005,b'00000000',0  ;Preset 05
        db      003,006,b'00000000',0  ;Preset 06
        db      003,007,b'00000000',0  ;Preset 07
        db      003,008,b'00000000',0  ;Preset 08
        db      003,009,b'00000000',0  ;Preset 09
        db      003,010,b'00000000',0  ;Preset 10
        db      003,011,b'00000000',0  ;Preset 11
        db      003,012,b'00000000',0  ;Preset 12
        db      003,013,b'00000000',0  ;Preset 13
        db      003,014,b'00000000',0  ;Preset 14
        db      003,015,b'00000000',0  ;Preset 15
        
;------------------------------------------------------------------------------
; Banco EEPROM 2
;------------------------------------------------------------------------------
; REGISTRATION  0..7
                org 0xF00200

EE_REGISTRATION
        db      000,000,000,000,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      001,001,001,001,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      002,002,002,002,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      003,003,003,003,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      004,004,004,004,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      005,005,005,005,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      006,006,006,006,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      007,007,007,007,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000


  
;------------------------------------------------------------------------------
; Banco EEPROM 3
;------------------------------------------------------------------------------
; REGISTRATION  8..15

                org 0xF00300
    
        db      008,008,008,008,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      009,009,009,009,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      010,010,010,010,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      011,011,011,011,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      012,012,012,012,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      013,013,013,013,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      014,014,014,014,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

        db      015,015,015,015,100,100,100,100,080,080,080,080,063,063,063,063
        db      012,012,012,012,000,000,000,000,000,000,000,000,000,000,000,000

;------------------------------------------------ EEPROM ----------------------
                ORG             PC_ROM_0
;------------------------------------------------ ROM_0 -----------------------






;******************************************************************************
; SUBRUOTINE:   Salva parametri PitchBender
;
; Chiamata:     -set-   PitchLimit (H e L)
;               -set-   PitchDelta
;               call    EE_SavePitch
;
; Chiamata da:  ---     ---     Lavora in:      prom_1          Stack:   2/3
; Ritorna:      ---     ---     Usa Reg:        Reg0,1          FSR:    ---
;******************************************************************************
EE_SavePitch    movlw   low EE_PitchDelta
                movwf   Reg1
                movf    PitchDelta,w
                rcall   EE_Write
                bnz     EE_Save_err
                ;.......................................;
                movlw   low EE_NumPitch
                movwf   Reg1
                movf    NumPitch,w
                rcall   EE_Write
                bnz     EE_Save_err
                ;.......................................;
                return

;******************************************************************************
; SUBRUOTINE:   Set/Clr_EE_Flag         Modifica flag in EEPROM indicato da W
;
; Chiamata:     -set-   W               b'00001000'
;               rcall   Set_EE_Flag     setta a 1 il flag 3 in EEPROM
;
; Chiamata da:  ---     ---     Lavora in:      prom_1          Stack:   2
; Ritorna:      ---     ---     Usa Reg:        Reg0,1          FSR:    ---
;******************************************************************************
Set_EE_Flag_0   movwf   Reg0                    ;salva su reg0
                movlw   low EE_Flags_0          ;\
                movwf   Reg1                    ;|Leggi flag
                rcall   EE_Read                 ;/
                iorwf   Reg0,w                  ;Setta flag indicato da Reg0
                rcall   EE_Write                ;Salva
                bnz     EE_Save_err             ;
                return                          ;
                ;...............................;
Clr_EE_Flag_0   movwf   Reg0                    ;salva su reg0
                comf    Reg0                    ;
                movlw   low EE_Flags_0          ;\
                movwf   Reg1                    ;|Leggi flag
                rcall   EE_Read                 ;/
                andwf   Reg0,w                  ;Azzera flag indicato da Reg0
                rcall   EE_Write                ;Salva
                bnz     EE_Save_err             ;
                return                          ;

;******************************************************************************
; SUBRUOTINE:   EE_Save_err        Errore di scrittura in EEPROM
;
; Chiamata:     rcall   EE_Save_err
;
; Chiamata da:  ---     ---     Lavora in:      prom_1          Stack:   2
; Ritorna:      ---     ---     Usa Reg:        Reg0,1,2,3      FSR:    ---
;******************************************************************************
EE_Save_err     ; gestione errori di scrittura in EEPROM
                return
                
;******************************************************************************
; SUBRUOTINE:   EE_Write
;
;               EE_WRITE:       scrive e verifica dalla memoria EEPROM puntata
;                               da Reg1
;
; Chiamata:     -set-   Reg1            Address in EEPROM
;               -set-   W               Byte da scrivere
;               rcall   EE_Write
; Restituisce   Z=1                     Se la verifica va a buon fine
;               Reg1    puntatore in eeprom sul byte scritto
;               W       Byte (ri)letto per verifica
;
; Chiamata da:  ---     ---     Lavora in:      prom_1          Stack:   1
; Ritorna:      ram_0   ---     Usa Reg:        Reg0,1          FSR:    ---
;******************************************************************************
EE_Write        movwf   Reg0            ;\Salva byte da scrivere
                movwf   EEDATA          ;/
                movf    Reg1,w          ;\
                movwf   EEADR           ;/Carica indirizzo
                bra     EWN_1           ;

;******************************************************************************
; SUBRUOTINE:   EE_Write_Next:  scrive e verifica dalla memoria EEPROM sul byte
;                               successivo alla precedente scrittura EE_WRITE
;
; Chiamata:     -set-   W               Byte da scrivere
;               rcall   EE_Write_Next
; Restituisce   Z=1                     Se la verifica va a buon fine
;               W       Byte (ri)letto per verifica
;
; Chiamata da:  ---     ---     Lavora in:      prom_1          Stack:   1
; Ritorna:      ram_0   ---     Usa Reg:        Reg0            FSR:    ---
;******************************************************************************
EE_Write_Next   movwf   Reg0            ;\Salva byte da scrivere
                movwf   EEDATA          ;/

;******************************************************************************
EWN_1           bcf     EECON1,EEPGD    ;Punta a Memoria EEPROM (oppure alla FLASH)
                bsf     EECON1,WREN     ;Abilita scrittura in EEPROM
                ;.......................;.......................SEZIONE CRITICA
                bcf     INTCON,GIE      ;Disabilita interrupt
                movlw   h'55'           ;\
                movwf   EECON2          ;|Sequenza di protezione
                movlw   h'AA'           ;|
                movwf   EECON2          ;/
                bsf     EECON1,WR       ;Start scrittura
                bsf     INTCON,GIE      ;Abilita interrupt
                ;.......................;......................................
                btfsc   EECON1,WR       ;\Attendi fine scrittura     
                bra     $-2             ;/
                bcf     EECON1,WREN     ;Disabilita scrittura in EEPROM
                ;.......................;
                bsf     EECON1,RD       ;Operazione di lettura
                movf    EEDATA,w        ; ------------------------+
                incf    EEADR           ;\                        |
                skpnz                   ;| Incremento puntatore   |
                incf    EEADRH          ;/                        |
                xorwf   Reg0            ;Verifica scrittura  <----+
                return                  ;
                
;******************************************************************************
; SUBRUOTINE:   EE_Read
;               
;               Legge dalla memoria EEPROM
;
; Chiamata:     -set-   W               Address in EEPROM
;               rcall   EE_Read
; Restituisce   W                       Byte letto
;
; Chiamata da:  ---     ---     Lavora in:      prom_1          Stack:   1
; Ritorna:      ram_0   ---     Usa Reg:        ---            
;******************************************************************************
EE_Read         movwf   EEADR           ;
EE_Read_Next    bcf     EECON1,EEPGD    ;Punta a Memoria EEPROM
                bsf     EECON1,RD       ;Operazione di lettura
                movf    EEDATA,w        ;Leggi byte
                incf    EEADR           ;\
                skpnz                   ;| Incremento puntatore
                incf    EEADRH          ;/
                return                  ;

;******************************************************************************
; SUBRUOTINE:   Recall
;
;               Ripristina il contenuto dell'EEPROM con le Entry di Factory
;
; Chiamata:     rcall   Recall_xxxxx
;******************************************************************************
Recall_Chord_Bass_Preset
                movlw   high ROM_CHORD_PRESET
                movwf   TBLPTRH 
                movlw   low  ROM_CHORD_PRESET
                movwf   TBLPTRL
                movlw   high (EE_CHORD_PRESET - 0xF00000)
                movwf   EEADRH
                movlw   low  (EE_CHORD_PRESET - 0xF00000)
                movwf   EEADR
                movlw   (END_ROM_BASS_PRESET - ROM_CHORD_PRESET)
                movwf   Reg1
RP_1            tblrd*+
                movf    TABLAT,w
                rcall   EE_Write_Next
                decfsz  Reg1
                bra     RP_1 
                return
 
Recall_Treble_Preset
                movlw   high ROM_TRB_PRESET_A
                movwf   TBLPTRH 
                movlw   low  ROM_TRB_PRESET_A
                movwf   TBLPTRL
                movlw   high (EE_TRB_PRESET_A - 0xF00000)
                movwf   EEADRH
                movlw   low  (EE_TRB_PRESET_A - 0xF00000)
                movwf   EEADR
                movlw   low (END_ROM_TRB_PRESET_B - ROM_TRB_PRESET_A)
                movwf   Reg1
RP_2            tblrd*+
                movf    TABLAT,w
                rcall   EE_Write_Next
                decfsz  Reg1
                bra     RP_2 
                return               
   
Recall_Registration
                movlw   high ROM_REGISTRATION
                movwf   TBLPTRH 
                movlw   low  ROM_REGISTRATION
                movwf   TBLPTRL
                movlw   high (EE_REGISTRATION - 0xF00000)
                movwf   EEADRH
                movlw   low  (EE_REGISTRATION - 0xF00000)
                movwf   EEADR
                movlw   high (END_ROM_REGISTRATION - ROM_REGISTRATION)
                movwf   Reg2
                movlw   low (END_ROM_REGISTRATION - ROM_REGISTRATION)
                movwf   Reg1
RP_3            tblrd*+
                movf    TABLAT,w
                rcall   EE_Write_Next
                decfsz  Reg1
                bra     RP_3 
                decfsz  Reg2
                bra     RP_3 
                return                 

Recall_Preference
                movlw   high ROM_PREFERENCE
                movwf   TBLPTRH 
                movlw   low  ROM_PREFERENCE
                movwf   TBLPTRL
                movlw   high (EE_PREFERENCE - 0xF00000)
                movwf   EEADRH
                movlw   low  (EE_PREFERENCE - 0xF00000)
                movwf   EEADR
                movlw   END_ROM_PREFERENCE - ROM_PREFERENCE
                movwf   Reg1
RP_4            tblrd*+
                movf    TABLAT,w
                rcall   EE_Write_Next
                decfsz  Reg1
                bra     RP_4 
                return     


;******************************************************************************
; SUBRUOTINE:   Ini_eeprom
;
;               Legge dalla EEPROM i valori da assegnare alle variabili in RAM
;
; Chiamata:     rcall   Ini_eeprom
;
; Chiamata da:  ---     Lavora in:      prom_1          Stack:   1
; Ritorna:      ram_0   Usa Reg:        ---            
;******************************************************************************
Ini_eeprom
        bcf     EECON1,CFGS     ;Punta alla memoria EEPROM (oppure reg. di configurazione)
        clrf    EEADRH          ;
        ;......................................................................
        ;  FLAG 0
        ;......................................................................
        movlw   low EE_Flags_0                  ;\
        rcall   EE_Read                         ;|Leggi FLAG su Reg1
        movwf   Reg1                            ;/
        ;......................................................................
        ;.......................................;
        bcf     F_IniStr_2                      ;\
        btfsc   Reg1,F_STR_INI                  ;|Assegna Stringa iniziale
        bsf     F_IniStr_2                      ;/
        ;.......................................;
        bcf     F_FunzMantice1                  ;\
        btfsc   Reg1,F_FUN_MANTICE              ;|Assegna Funzione mantice
        bsf     F_FunzMantice1                  ;/
        ;.......................................;
        bcf     F_Tune442                       ;\
        btfsc   Reg1,F_ACCORDATURA              ;|Assegna accordatura
        bsf     F_Tune442                       ;/        
        ;.......................................;
        bcf     F_Radio                         ;\
        btfsc   Reg1,F_RADIO                    ;|1 = Uscite identiche per Midi Radio
        bsf     F_Radio                         ;/
        ;.......................................;
        bcf     F_RunStatusIn                   ;\
        btfsc   Reg1,F_RUNSTATUS_IN             ;|abilita Running Status in ingresso
        bsf     F_RunStatusIn                   ;/
        ;.......................................;
        bcf     F_RunStatusOut                  ;\
        btfsc   Reg1,F_RUNSTATUS_OUT            ;|abilita Running Status in uscita
        bsf     F_RunStatusOut                  ;/
        ;.......................................;
        bcf     F_Pot_Off                       ;\
        btfsc   Reg1,F_POT_OFF                  ;|1 = No Potenziometri
        bsf     F_Pot_Off                       ;/
        ;.......................................;
        bcf     F_FunzFisa                      ;\
        btfsc   Reg1,F_FUNZ_FISA                ;|Solo Chiudere (Aprire è come Chiudere)
        bsf     F_FunzFisa                      ;/


        ;......................................................................
        ;  FLAG 1
        ;......................................................................
        movlw   low EE_Flags_1                  ;\
        rcall   EE_Read                         ;|Leggi FLAG su Reg1
        movwf   Reg1                            ;/
        ;......................................................................
        ;.......................................;
        bcf     F_TrsDiaMat                     ;\
        btfsc   Reg1,F_TRS_DIA                  ;|Traspose diatonico 1 riga matrice
        bsf     F_TrsDiaMat                     ;/
        ;.......................................;
        ;.......................................;
        ;.......................................;
        ;.......................................;

        ;......................................................................
        movlw   low EE_Ch_Treble_a              ;Assegna canale dei Treble A
        rcall   EE_Read                         ;
        movwf   Channel_Treble_a                ;
        ;.......................................;
        movlw   low EE_Ch_Treble_b              ;Assegna canale dei Treble B
        rcall   EE_Read                         ;
        movwf   Channel_Treble_b                ;
        ;.......................................;
        movlw   low EE_Ch_Chord_a               ;Assegna canale degli accordi A
        rcall   EE_Read                         ;
        movwf   Channel_Chord_a                 ;
        ;.......................................;
        movlw   low EE_Ch_Chord_b               ;Assegna canale degli accordi B
        rcall   EE_Read                         ;
        movwf   Channel_Chord_b                 ;
        ;.......................................;
        movlw   low EE_Ch_Bass_a                ;Assegna canale dei bassi A
        rcall   EE_Read                         ;
        movwf   Channel_Bass_a                  ;
        ;.......................................;
        movlw   low EE_Ch_Bass_b                ;Assegna canale dei bassi B
        rcall   EE_Read                         ;
        movwf   Channel_Bass_b                  ;
        ;.......................................;
        movlw   low EE_Ch_TreblePC_b            ;Assegna canale dei Treble B
        rcall   EE_Read                         ;
        movwf   Channel_TreblePC_b              ;
        ;.......................................;
        movlw   low EE_SogliaBellow             ;Assegna soglia mantice
        rcall   EE_Read                         ;
        movwf   SogliaBellow                    ;
        ;.......................................;
        movlw   low EE_Cancel                   ;Assegna default Cancel
        rcall   EE_Read                         ;
        movwf   Cancel                          ;
        ;.......................................;
        movlw   low EE_TrebleVelocity_a         ;Assegna default Dinamica Treble
        rcall   EE_Read                         ;
        movwf   TrebleVelocity_a                ;        
        ;.......................................;
        movlw   low EE_TrebleVelocity_b         ;Assegna default Dinamica Treble
        rcall   EE_Read                         ;
        movwf   TrebleVelocity_b                ;        
        ;.......................................;
        movlw   low EE_ChordVelocity            ;Assegna default Dinamica Chord
        rcall   EE_Read                         ;
        movwf   ChordVelocity                   ;        
        ;.......................................;
        movlw   low EE_BassVelocity             ;Assegna default Dinamica Bass
        rcall   EE_Read                         ;
        movwf   BassVelocity                    ;        
        ;.......................................;
        movlw   low EE_Traspose                 ;Assegna default Traspose
        rcall   EE_Read                         ;
        movwf   Traspose                        ;        
        ;.......................................;
        movlw   low EE_PitchDelta               ;\
        rcall   EE_Read                         ;|Assegna variazione Pitch
        movwf   PitchDelta                      ;/
        ;.......................................;
        movlw   low EE_NumPitch                 ;\
        rcall   EE_Read                         ;|Assegna variazione Pitch
        movwf   NumPitch                        ;/
        ;.......................................;
        movlw   low EE_T_Plungo                 ;\
        rcall   EE_Read                         ;|Assegna tempo P_Lungo
        movwf   T_Plungo                        ;/
        ;.......................................;
        movlw   low EE_Ch_Treble_ba             ;Assegna canale dei Treble B su A
        rcall   EE_Read                         ;
        movwf   Channel_Treble_ba               ;
        ;.......................................;
        movlw   low EE_Ch_TreblePC_ba           ;Assegna canale dei Treble PC B su A
        rcall   EE_Read                         ;
        movwf   Channel_TreblePC_ba             ;
        ;.......................................;
        movlw   low EE_Righe_NC                 ;Assegna default Righe sensori NC
        rcall   EE_Read                         ;
        movwf   Righe_NC                        ;        
        ;.......................................;
        return                                  ;

;******************************************************************************
; Copia da EEPROM a FLASH ROM
;******************************************************************************

        #define P_Low           Reg0            ;\Puntatore in flash dove scrivere
        #define P_High          Reg1            ;/
        #define NByteL          Reg2            ;\Numero byte da scrivere
        #define NByteH          Reg3            ;/
        #define Cnt_fByte       Reg4            ;Contatore byte
        #define Cnt_fPage       Reg5            ;Contatore pagine

        
                ; Puntatore in EEPROM
                movlw   high (EE_REGISTRATION - 0xF00000)
                movwf   EEADRH
                movlw   low  (EE_REGISTRATION - 0xF00000)
                movwf   EEADR
                ; Puntatore a flash rom
                movlw   High    ROM_REGISTRATION
                movwf   P_High
                movlw   Low     ROM_REGISTRATION
                movwf   P_Low
                ; Numero byte da scrivere
                movlw   high    (END_ROM_REGISTRATION - ROM_REGISTRATION)
                movwf   NByteH 
                movlw   low     (END_ROM_REGISTRATION - ROM_REGISTRATION)
                movlw   NByteL
                ;...............................;
                rcall   Copy_EEtoFlash
                return
                
;******************************************************************************
; SUBRUOTINE:   Copy_EEtoFlash
;               Scrive buffer sulla Flash Rom
;
; Chiamata:     rcall   Ini_eeprom
;
; Chiamata da:  ---     Lavora in:      prom_1          Stack:   1
; Ritorna:      ram_0   Usa Reg:        0,1,2,3,4,5            
;******************************************************************************                
Copy_EEtoFlash  ;...............................;Punta a pagina da copiare 
                movff   P_High,TBLPTRH
                movf    P_Low,w
                andlw   ~MASK_ERASE_PAGE
                movwf   TBLPTRL
                ;...............................;
                ; Copia pagina flash sul buffer
                ;...............................;
                tblrd*-                         ;
                lfsr    1,BuffPagina            ;
                movlw   DIM_ERASE_PAGE          ;
                movwf   Cnt_fByte               ;
CWI_1           tblrd+*                         ;
                movff   TABLAT,POSTINC1         ;
                decfsz  Cnt_fByte               ;
                bra     CWI_1                   ;
                ;...............................;
                lfsr    1,BuffPagina            ;\
                movf    P_Low,w                 ;|Punta sul buffer al byte
                andlw   MASK_ERASE_PAGE         ;|da scrivere
                addwf   FSR1L                   ;/
                ;...............................;
CWI_2           rcall   EE_Read_Next            ;\Carica buffer con nuovi dati
                movwf   POSTINC1                ;/
                decfsz  NByteL                  ;
                bra     CWI_6                   ;
                decfsz  NByteH                  ;
                bra     CWI_6                   ;
                rcall   Page_Flash_Write        ;
                BZ      Errore_Flash_Write      ;
                return                          ;
                
                ;...............................;
                ; Controlla fine pagina
                ;...............................;
CWI_6           BSFLTL  FSR1L,low (BuffPagina + DIM_ERASE_PAGE),CWI_2
                rcall   Page_Flash_Write        ;
                BZ      Errore_Flash_Write      ;
                ;...............................;
                lfsr    1,BuffPagina            ;Copia pagina nel buffer
                movlw   DIM_ERASE_PAGE          ;
                movwf   Cnt_fByte               ;
CWI_4           tblrd+*                         ;
                movff   TABLAT,POSTINC1         ;
                decfsz  Cnt_fByte               ;
                bra     CWI_4                   ;
                ;...............................;
                lfsr    1,BuffPagina            ;
                bra     CWI_2                   ;
  
;------------------------------------------------------------------------------
; Errore di scrittura la verifica non è andata a buon fine
;------------------------------------------------------------------------------
Errore_Flash_Write  
                ; gestione errore di scrittura su flash rom
                return              



;******************************************************************************
; SUBRUOTINE:   Flash_Write
;               Scrive buffer sulla Flash Rom
;
; Chiamata:     rcall   Ini_eeprom
;
; Chiamata da:  ---     Lavora in:      prom_1          Stack:   1
; Ritorna:      ram_0   Usa Reg:        0,1,2,3,4,5            
;******************************************************************************
Page_Flash_Write
                movlw   ~MASK_ERASE_PAGE
                andwf   TBLPTRL
                ;...............................; Cancella ERASE_PAGE
                bsf     EECON1,EEPGD            ;
                bcf     EECON1,CFGS             ;
                bsf     EECON1,WREN             ;
                bsf     EECON1,FREE             ;
                bcf     INTCON,GIE              ;
                movlw   0x55                    ;
                movwf   EECON2                  ;
                movlw   0xAA                    ;
                movwf   EECON2                  ;
                bsf     EECON1,WR               ;Start erasing (CPU stall)
                bsf     INTCON,GIE              ;
                bcf     EECON1,WREN             ;
                ;...............................;
                lfsr    1,BuffPagina            ; Carica Holding Register
                tblrd*-
                movlw   DIM_ERASE_PAGE/DIM_WRITE_PAGE
                movwf   Cnt_fPage
CW_8            movlw   DIM_WRITE_PAGE
                movwf   Cnt_fByte
CW_4            movff   POSTINC1,TABLAT
                tblwt+*
                decfsz  Cnt_fByte
                bra     CW_4                
                ;...............................; Scrivi WRITE_PAGE
                bsf     EECON1,EEPGD            ;
                bcf     EECON1,CFGS             ;
                bsf     EECON1,WREN             ;
                bcf     EECON1,FREE             ;
                bcf     INTCON,GIE              ;
                movlw   0x55                    ;
                movwf   EECON2                  ;
                movlw   0xAA                    ;
                movwf   EECON2                  ;
                bsf     EECON1,WR               ;Start programming (CPU stall)
                bsf     INTCON,GIE              ;
                bcf     EECON1,WREN             ;
                ;...............................;
                decfsz  Cnt_fPage
                bra     CW_8
                ;...............................;
                ; Verifica                      ;
                ;...............................;
                movlw   ~MASK_ERASE_PAGE
                andwf   TBLPTRL
                ;...............................;
                lfsr    1,BuffPagina
                tblrd*-
                movlw   DIM_ERASE_PAGE
                movwf   Cnt_fByte
CW_5            tblrd+*
                movf    TABLAT,w
                cpfseq  POSTINC1
                bra     CW_6
                decfsz  Cnt_fByte
                bra     CW_5
                ;...............................;
                ; Scrittura OK
                ;...............................;
                clrz
                return
                ;...............................;
                ; ERROR di scrittura            ;
                ;...............................;
CW_6            setz
                return
