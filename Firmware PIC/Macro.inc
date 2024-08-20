        NOLIST
;----------------------------------------------------------------------------
;  Definizioni Base
;----------------------------------------------------------------------------
        #DEFINE clrw    clrf    WREG
        #DEFINE tstw    andlw   h'FF'           ;Test del registro W
        #DEFINE decw    addlw   -1              ;Decrementa registro W
        #DEFINE incw    addlw   +1              ;Incrementa registro W

        #DEFINE skpz    btfss   STATUS,Z
        #DEFINE skpnz   btfsc   STATUS,Z
        #DEFINE clrz    bcf     STATUS,Z
        #DEFINE setz    bsf     STATUS,Z


        #DEFINE skpc    btfss   STATUS,C
        #DEFINE skpnc   btfsc   STATUS,C
        #DEFINE clrc    bcf     STATUS,C
        #DEFINE setc    bsf     STATUS,C

        #DEFINE BANK_0  0
        #DEFINE BANK_1  1
        #DEFINE BANK_2  2
        #DEFINE BANK_3  3
        #DEFINE BANK_4  4
        #DEFINE BANK_5  5
        #DEFINE BANK_6  6
        #DEFINE BANK_7  7
        #DEFINE BANK_8  8
        #DEFINE BANK_9  9
        #DEFINE BANK_10 10
        #DEFINE BANK_11 11
        #DEFINE BANK_12 12
        #DEFINE BANK_13 13
        #DEFINE BANK_14 14
        #DEFINE BANK_15 15



SELECT_CASE     MACRO   x
                        movlw   high ($+16)
                        movwf   PCLATH
                        rlncf   x,w 
                        bcf     WREG,0
                        addlw   low ($+8)
                        skpnc
                        incf    PCLATH
                        movwf   PCL
                ENDM                
                
; ESEMPIO DI UTILIZZO DI SELECT_CASE              
;
;        SELECT_CASE     ReadBuff
;        bra     P_Piu
;        bra     P_Meno
;        bra     P_Section
;        bra     P_VolPiu
;        bra     P_VolMeno
;        bra     P_Bellow
;        
;        SELECT_CASE     ReadBuff
;        dt      0
;        dt      2
;        dt      3
;        dt      2
;        dt      1
;        dt      1
;
; ALTERNATIVA efficiente fino a 5-6 scelte
;        movf    ReadBuff,w              ;
;        BZ      Pul_ini_0               ;
;        addlw   -1                      ;
;        BZ      Pul_ini_1               ;
;        addlw   -1                      ;
;        BZ      Pul_ini_2               ;
;        addlw   -1                      ;
;        BZ      Pul_ini_3               ;
;        addlw   -1                      ;
;        BZ      Pul_ini_5               ;
;        ;...............................;
;        bra     Pul_ini_end             ;

SELECT_CASE_AUT MACRO   x,n,e             
        BSFGEL  x,n,e
        SELECT_CASE     x
        local a = 0
        while a < n
        bra      x#v(a)
a += 1
        endw
                ENDM
; ESEMPIO DI UTILIZZO DI SELECT_CASE_AUTomatico
;
;        SELECT_CASE_AUT    ReadBuff,10,Error
;
; crea un SELECT_CASE con salti progressivi a:
; ReadBuff0
; ReadBuff1
; ReadBuff2
; ...
; ReadBuff9
;
; Se ReadBuff è maggiore di 9 salta a Error




        ;.......................................;
        ; Leggi matrice t con rollover di pagina;
        ;.......................................;
LEGGI_MAT      MACRO    t                
                addlw   low t                   ;\Posiziona da W Pointer L --+
                movwf   TBLPTRL                 ;/                           |
                clrf    TBLPTRH                 ;\                        Carry
                movlw   high t                  ;|Posizione Pointer H        |
                addwfc  TBLPTRH                 ;/                     <-----+
                tblrd*                          ;\Leggi tabella  
                movf    TABLAT,w                ;/       
               ENDM



        ;......................................................................
        ; Leggi matrice t con rollover di pagina
        ; Versione a 16 bit
        ; Indice matrice 16 bit:
        ; W    = IndiceL                        
        ; Reg0 = IndiceH
        ;......................................................................
LEGGI_MAT_16   MACRO    t                
                addlw   low t                   ;\Posiziona da W Pointer L --+
                movwf   TBLPTRL                 ;/                           |
                movlw   high t                  ;\Posizione Pointer H      Carry
                addwfc  Reg0,w                  ;/                     <-----+
                movwf   TBLPTRH                 ;
                tblrd*                          ;\Leggi tabella  
                movf    TABLAT,w                ;/
               ENDM
               
;----------------------------------------------------------------------------
;  Definizioni MACRO
;----------------------------------------------------------------------------
        ;.......................; Carica timer t da variabile
LDFTMR  MACRO   t,x
        movf    x,w
        movwf   Cnt_Timer_#v(t)
        ENDM        
        ;.......................; Carica timer t da litteral
LDTMR   MACRO   t,x
        movlw   x
        movwf   Cnt_Timer_#v(t)
        ENDM        
        ;.......................; Salta se timeout timer t
BSTOUT  MACRO   t,b
        movf    Cnt_Timer_#v(t),f    
        bz      b
        ENDM
        ;.......................; Ferma Timer t
STOPTMR MACRO   t
        clrf    Cnt_Timer_#v(t)
        ENDM

LBC     MACRO   b
        skpnc
        goto    b
        ENDM
LBNC    MACRO   b
        skpc
        goto    b
        ENDM

LBZ     MACRO   b
        skpnz
        goto    b
        ENDM

LBNZ    MACRO   b
        skpz
        goto    b
        ENDM

        ;.......................; Muovi da l a f utilizzando w
MOVLWF  MACRO   x,y
        movlw   x
        movwf   y
        ENDM
;----------------------------------------------------------------------------
; OPERAZIONI SUI BIT      
;----------------------------------------------------------------------------
        ;............................ Setta il bit in W
bsfw    MACRO    b
        local    i=1
        variable i = i<<b	
        iorlw    i
        ENDM
        ;............................ Azzera il bit in W
bcfw    MACRO    b
        local    i=1
        variable i = ~(i<<b)	
        andlw    i
        ENDM
        ;............................ Branch se il bit e' settato
BSS     MACRO   x,y,z
        btfsc   x,y
        bra     z
        ENDM
        ;............................ Branch se il bit e' clear
BSC     MACRO   x,y,z
        btfss   x,y
        bra     z
        ENDM
        
        ;............................ Branch se il bit e' settato
LBSS    MACRO   x,y,z
        btfsc   x,y
        goto    z
        ENDM
        ;............................ Branch se il bit e' clear
LBSC    MACRO   x,y,z
        btfss   x,y
        goto    z
        ENDM
        
        
;----------------------------------------------------------------------------
; CONFRONTI DI UGUAGLIANZA
;----------------------------------------------------------------------------
         ;........................... Salta a 'b' se 'W' non e' uguale a 'y'
BSWNEF  MACRO   x,b	
        xorwf   x,w
        bnz     b
        ENDM
        ;........................... Salta a 'b' se 'F' non e' uguale a 'y'
BSFNEF  MACRO   x,y,b
        movf    x,w
        xorwf   y,w
        bnz     b
        ENDM
        ;........................... Salta a 'b' se 'W' e' uguale a 'y'
BSWEF   MACRO   x,b	
        xorwf   x,w
        bz      b
        ENDM
        ;........................... Salta a 'b' se 'W' e' uguale a 'y'
BSFEF   MACRO   x,y,b
        movf    x,w
        xorwf   y,w
        bz      b
        ENDM
        ;........................... Salta a 'b' se 'W' non e' uguale a litteral
BSWNEL  MACRO   x,b	
        xorlw   x
        bnz     b
        ENDM
        ;........................... Salta a 'b' se 'W' non e' uguale a litteral
BSFNEL  MACRO   x,y,b
        movf    x,W
        xorlw   y
        bnz     b
        ENDM
        ;........................... Salta a 'b' se 'W' e' uguale a litteral
BSWEL   MACRO   x,b	
        xorlw   x
        bz      b
        ENDM
        ;........................... Salta a 'b' se 'W' e' uguale a litteral
BSFEL   MACRO   x,y,b
        movf    x,w
        xorlw   y
        bz      b
        ENDM
        ;........................... Salta a 'b' se 'W' e' uguale a litteral
LBSFEL  MACRO   x,y,b
        movf    x,w
        xorlw   y
        skpnz
        goto    b
        ENDM

;----------------------------------------------------------------------------
; CONFRONTI ORDINALI
;       GT = Greater Then       --> Maggiore di ..
;       GE = Greater Equal      --> Maggiore o uguale di ..
;       LT = Less    Then       --> Minore di ..          
;       LE = Less    Equal      --> Minore o Uguale di ..
;----------------------------------------------------------------------------

;----------------------------------------------------------------------------
; Confronti tra locazioni in RAM (file)
;----------------------------------------------------------------------------
        ;........................... Salta a 'b' se 'F' e' maggiore di 'y'
BSFGTF  MACRO   x,y,b
        movf    x,W
        subwf   y,w
        bnc     b
        ENDM
        ;........................... Salta a 'b' se 'F' e' minore o uguale a 'y'
BSFGEF  MACRO   x,y,b
        movf    y,W
        subwf   x,w
        bc      b
        ENDM
        ;........................... Salta a 'b' se 'F' e' maggiore di 'y'
BSFLTF  MACRO   x,y,b
        movf    y,W
        subwf   x,w
        bnc     b
        ENDM
        ;........................... Salta a 'b' se 'F' e' minore o uguale a 'y'
BSFLEF  MACRO   x,y,b
        movf    x,W
        subwf   y,w
        bc      b
        ENDM

;----------------------------------------------------------------------------
; Confronti tra W e locazione in RAM (file)
;----------------------------------------------------------------------------
        ;........................... Salta a 'b' se 'W' e' maggiore di 'y'
BSWGTF  MACRO   x,b
        subwf   x,w
        bnc     b
        ENDM
        ;........................... Salta a 'b' se 'W' e' minore o uguale a 'y'
BSWLEF  MACRO   x,b
        subwf   x,w
        bc      b
        ENDM

        
;----------------------------------------------------------------------------
; Confronti tra locazioni in RAM (file) e LITTERAL
;----------------------------------------------------------------------------
        ;........................... Salta a 'b' se 'F' e' maggiore di litteral
BSFGTL  MACRO   x,y,b
        movf    x,W
        sublw   y
        bnc     b
        ENDM
        ;........................... Salta a 'b' se 'F' e' minore o uguale a litteral
BSFGEL  MACRO   x,y,b
        movlw   y
        subwf   x,w
        bc      b
        ENDM
LBSFGEL MACRO   x,y,b
        movlw   y
        cpfslt  x
        goto    b
        ENDM
        ;........................... Salta a 'b' se 'F' e' maggiore di litteral
BSFLTL  MACRO   x,y,b
        movlw   y
        subwf   x,w
        bnc     b
        ENDM
        ;........................... Salta a 'b' se 'F' e' minore o uguale a litteral
BSFLEL  MACRO   x,y,b
        movf    x,W
        sublw   y
        bc      b
        ENDM
        ;........................... Salta se 'F' e' minore o uguale a litteral
SSFLEL  MACRO   x,y
        movf    x,W
        sublw   y
        skpc
        ENDM


;----------------------------------------------------------------------------
; Confronti tra W e LITTERAL
;----------------------------------------------------------------------------
        ;...............Salta a 'b' se 'W' e' maggiore di litteral
BSWGTL  MACRO   x,b
        sublw   x
        bnc     b
        ENDM
        ;...............Salta a 'b' se 'W' e' maggiore o uguale di litteral
BSWGEL  MACRO   x,b
        addlw   -x
        bc      b
        ENDM
        ;...............Salta a 'b' se 'W' e' minore o uguale a litteral
BSWLEL  MACRO   x,b
        sublw   x
        bc      b
        ENDM
        ;...............Salta a 'b' se 'W' e' minore di litteral
BSWLTL  MACRO   x,b
        addlw   -x
        bnc     b
        ENDM


;----------------------------------------------------------------------------
; Salti dopo i confronti
;----------------------------------------------------------------------------
        ;........................... Salta se 'W' non e' uguale a 'y'
SSWNEF  MACRO   x	
        xorwf   x,w
        skpnz
        ENDM
        ;........................... Salta se 'W' non e' uguale a litteral
SSWNEL  MACRO   x	
        xorlw   x
        skpnz   
        ENDM
        ;........................... Salta se 'W' e' uguale a 'y'
SSWEF   MACRO   x	
        xorwf   x,w
        skpz
        ENDM
        ;........................... Salta se 'W' e' uguale a litteral
SSWEL   MACRO   x	
        xorlw   x
        skpz    
        ENDM
        ;........................... Salta se 'x' non e' uguale a 'y'
SSFNEF  MACRO   x,y
        movf    x,w
        xorwf   y,w
        skpnz
        ENDM
        ;........................... Salta se 'W' non e' uguale a litteral
SSFNEL  MACRO   x,y
        movf    x,w
        xorlw   y
        skpnz   
        ENDM
        ;........................... Salta se 'x' e' uguale a 'y'
SSFEF   MACRO   x,y
        movf    x,w
        xorwf   y,w
        skpz
        ENDM
        ;........................... Salta se 'W' e' uguale a litteral
SSFEL   MACRO   x,y
        movf    x,w
        xorlw   y
        skpz    
        ENDM

;----------------------------------------------------------------------------
; SALTI DOPO I CONFRONTI ORDINALI
;       GT = Greater Then       --> Maggiore di ..
;       GE = Greater Equal      --> Maggiore o uguale di ..
;       LT = Less    Then       --> Minore di ..          
;       LE = Less    Equal      --> Minore o Uguale di ..
;----------------------------------------------------------------------------
        ;........................... Salta se 'W' e' minore o uguale a 'y'
SSWLEF  MACRO   x
        subwf   x,w
        skpc
        ENDM
        ;........................... Salta se 'W' e' maggiore di 'y'
SSWGTF  MACRO   x
        subwf   x,w
        skpnc
        ENDM

;----------------------------------------------------------------------------
; CONFRONTI CON 0
;----------------------------------------------------------------------------
        ;........................... Salta a 'b' se 'f' == '0'
BSFE0   MACRO   x,b
        movf    x,f
        bz      b
        ENDM
        ;........................... Salta a 'b' se 'f' != '0'
BSFNE0  MACRO   x,b
        movf    x,f
        bnz     b
        ENDM
        ;........................... Salta a 'b' se 'w' == '0'
BSwE0   MACRO   b
        tstw    
        bz      b
        ENDM
        ;........................... Salta a 'b' se 'w' != '0'
BSwNE0  MACRO   b
        tstw    
        bnz     b
        ENDM
        ;........................... Salta se 'f' == '0'
SSFE0   MACRO   x
        movf    x,f
        skpz    
        ENDM
        ;........................... Salta se 'f' != '0'
SSFNE0  MACRO   x
        movf    x,f
        skpnz   
        ENDM
        ;........................... Salta se 'w' == '0'
SSwE0   MACRO   
        tstw    
        skpz    
        ENDM
        ;........................... Salta se 'w' != '0'
SSwNE0  MACRO   
        tstw    
        skpnz   
        ENDM


;******************************************************************************
; MACRO:        RITD   
;
; Operazione:   Versione macro che ritarda x decimi di secondo a 20 Mhz 
;
; Chiamata:     RITD    d'x'
;
; Richiama Sub: Rit_m/Rit_1m
;
; Chiamata da:  ram_0   Lavora in:      prom_0          Stack:   2
; Ritorna:      ram_0   Usa Reg:        0,1,2,3
;******************************************************************************
RITD    MACRO   x
        IF x <= d'255'
          movlw  x
          call   Rit_d
        ELSE
          ERROR "Ritardo troppo lungo per macro RITD"
        ENDIF
        ENDM


 
;******************************************************************************
; MACRO:        RITM   
;
; Operazione:   Versione macro che ritarda x millisecondi a 20 Mhz 
;
; Chiamata:     RITM    d'x'
;
; Richiama Sub: Rit_m/Rit_1m
;
; Chiamata da:  ram_0   Lavora in:      prom_0          Stack:   1
; Ritorna:      ram_0   Usa Reg:        0,1,2            
;******************************************************************************
RITM    MACRO   x                       ;Ritardo in millisecondi > di x
        IF x == 1 
          call    Rit_1m                ;richiama ruotine di 1 mSec
        ELSE
          IF x <= d'255'
            movlw  x
            call   Rit_m
          ELSE
                ERROR "Ritardo troppo lungo per macro RITM"
          ENDIF
        ENDIF
        ENDM

        LIST
