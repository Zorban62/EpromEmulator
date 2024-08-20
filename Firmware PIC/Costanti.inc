       
        CONSTANT   TEST             = 0     ;1 = Versione TEST per debugger


        ;----------------------------------------------------------------------
        ; 1.00
        ;----------------------------------------------------------------------



;******************************************************************************
;
;                                 DEFINIZIONI
;
;******************************************************************************
       
        CONSTANT        PIC_TYPE       = b'00100000011'    ;0x103 259d


    ifdef           __18F4525
        #include    p18f4525.inc
        #include    ConfigBIT_4525.inc
        ;...................................;
        CONSTANT    DIM_WRITE_PAGE  = 64    ;Dimensione pagina di scrittura in flash
        CONSTANT    DIM_ERASE_PAGE  = 64    ;Dimensione pagina di cancellazione
        CONSTANT    INI_MEMRIP      = 0x0800
        CONSTANT    FIN_MEMRIP      = 0xBFFF
        CONSTANT    DIM_EEPROM      = 1024   ;Dimensione EEPROM
    endif
    ifdef           __18F4550
        #include    p18f4550.inc
        #include    ConfigBIT_4550.inc
        ;...................................;
        CONSTANT    DIM_WRITE_PAGE  = 32    ;Dimensione pagina di scrittura in flash
        CONSTANT    DIM_ERASE_PAGE  = 64    ;Dimensione pagina di cancellazione
        CONSTANT    INI_MEMRIP      = 0x0800
        CONSTANT    FIN_MEMRIP      = 0x7FFF
        CONSTANT    DIM_EEPROM      = 256   ;Dimensione EEPROM
    endif
    ifdef           __18F4620
        #include    p18f4620.inc
        #include    ConfigBIT_4620.inc
        CONSTANT    DIM_WRITE_PAGE  = 64    ;Dimensione pagina di scrittura in flash
        CONSTANT    DIM_ERASE_PAGE  = 64    ;Dimensione pagina di cancellazione
        CONSTANT    INI_MEMRIP      = 0x0800
        CONSTANT    FIN_MEMRIP      = 0xFFFF
        CONSTANT    DIM_EEPROM      = 1024  ;Dimensione EEPROM
    endif
    ifdef           __18F2480
        CONSTANT    DIM_WRITE_PAGE  = 32    ;Dimensione pagina di scrittura in flash
        CONSTANT    DIM_ERASE_PAGE  = 64    ;Dimensione pagina di cancellazione
        CONSTANT    INI_MEMRIP      = 0x0800
        CONSTANT    FIN_MEMRIP      = 0x3FFF
    endif
    ifdef           __18F442
        CONSTANT    DIM_WRITE_PAGE  = 8     ;Dimensione pagina di scrittura in flash
        CONSTANT    DIM_ERASE_PAGE  = 64    ;Dimensione pagina di cancellazione
        CONSTANT    INI_MEMRIP      = 0x0800
        CONSTANT    FIN_MEMRIP      = 0x3FFF
    endif
    ifdef           __18F14K22
        #include    p18f14k22.inc
        CONSTANT    DIM_WRITE_PAGE  = 16    ;Dimensione pagina di scrittura in flash
        CONSTANT    DIM_ERASE_PAGE  = 64    ;Dimensione pagina di cancellazione
        CONSTANT    INI_MEMRIP      = 0x0800
        CONSTANT    FIN_MEMRIP      = 0x3FFF
    endif
    
        CONSTANT        MASK_ERASE_PAGE = DIM_ERASE_PAGE - 1  
