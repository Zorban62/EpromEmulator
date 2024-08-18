using System;
using System.Timers;
using System.Threading;
using System.ComponentModel;
using FTD2XX_NET;
using System.IO.Ports;
using System.Windows.Forms;




namespace Andrea_NameSpace
{

    //----------------------------- Comandi PC ------------------------
    public enum ComFrame : byte
    {
        COM_LOOP = 0,
        COM_BOOT = 1,
        COM_START = 2,
        COM_READ_FLASH = 3,
        COM_WRITE_FLASH = 4,
        COM_READ_EE = 5,
        COM_WRITE_EE = 6,
        COM_READ_CONFIG = 7,
        COM_RESET = 8,
        COM_READ_RAM = 9,
        COM_WRITE_RAM = 10,
        COM_SET_COMMAND = 11,
        // Evento generato verso PC
        EVENT = 0xFF,
    }



    public enum EventInterfaccia : byte
    {
        FRAME_DA_REMOTO,     // Evento in arrivo da Device Remoto
        ERR_CHKSUM_RX,       // Errore di Checksum sul frame in RX
        ERR_TIMEOUT_RX,      // Errore di Messaggio in TimeOut sul frame in RX
        ERR_LINE_RX,         // Errore Overrun-Framing-Parity sul frame in RX
        ERR_FTDI             // Errore FTDI non accessibile
    }


    public enum ErrorNodo : int
    {
        NO_ERROR,
        NODO_NON_RISPONDE,
        RISP_COM_ERRATO,
        ERRORE_TIME_OUT,
        ERRORE_CHECKSUM,
        ERRORE_SCRITTURA,
        STOP_UTENTE,
        ERRORE_VERIFICA,
        ERRORE_RESET_CPU,
        ERRORE_RUN_CPU
    }


    public class FrameInterfaccia
    {
        public ComFrame Comando;
        public uint Add;
        public byte[] Msg;
    }


    //-------------------------------------------------------------------------------------------------------------
    /// <summary> Classe che gestisce l'interfaccia USB-CAN (firmware versione 4.x) </summary>
    //-------------------------------------------------------------------------------------------------------------
    public class InterfacciaComm
    {
        FTDI myFtdiDevice;
        FTDI.FT_STATUS ftStatus;
        AutoResetEvent receivedDataEvent;
        BackgroundWorker dataReceivedHandler;
        private System.Timers.Timer Frame_Timer;

        public double TimeOutRx { get; set; } = 10;

        private Mutex mut_a = new Mutex(); //Crea mutex per sincronizzare i thread su Richieste Nodo
        private AutoResetEvent autoEventInterface = new AutoResetEvent(false); //Attende che il messaggio in arrivo venga elaborato

        private FrameInterfaccia frameDaInt;

        public uint BaudRate { get; private set; }
        public bool Connessa { get; private set; } = false;
        public bool stop { private get; set; } = false;

        private Monitor monitor;

        public enum TypeCon
        {
            NON_CONNESSO,
            RS232,
            FTDI
        }

        public TypeCon StatoCon { get; private set; } = TypeCon.NON_CONNESSO;

        private enum StatoFrame
        {
            COMANDO,
            ADDH,
            ADDL,
            NUM_BYTE,
            BYTE_MSG
        }
        StatoFrame statoFrame = StatoFrame.COMANDO;
        
        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Costruttore della classe </summary>
        //-------------------------------------------------------------------------------------------------------------
        public InterfacciaComm(Monitor mon)
        {
            monitor = mon;
            myFtdiDevice = new FTDI(); // Create new instance of the FTDI device class
        }
        

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Connetti con Interfaccia FTDI </summary>
        //-------------------------------------------------------------------------------------------------------------
        public bool Connetti(TypeCon Tc, string parametro, uint baudRate)
        {
            //if (StatoCon == TypeCon.RS232) CloseRS232();
            //if (StatoCon == TypeCon.FTDI) CloseFTDI();

            bool OpenCon=false;

            switch (Tc)
            {
                case TypeCon.NON_CONNESSO:
                    break;
                case TypeCon.RS232:
                    OpenCon = RIFcomRS232(parametro, baudRate);
                    break;
                case TypeCon.FTDI:
                    OpenCon = RIFcomFTDI(parametro, baudRate);
                    break;
            }
            return OpenCon;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Disconnetti da Interfaccia FTDI </summary>
        //-------------------------------------------------------------------------------------------------------------
        public bool DisConnetti()
        {
            switch (StatoCon) {
                case TypeCon.FTDI:
                    CloseFTDI();
                    break;
                case TypeCon.RS232:
                    CloseRS232();
                    break;
                default: return false;
            }
            return true;
        }

        // Definiamo il delegato per l'evento
        public delegate void ErrorInterfaceEventHandler(EventInterfaccia evn, FrameInterfaccia fic);
        // Definiamo l'evento (InterfaceEvent) tramite il delegato (ErrorInterfaceEventHandler )
        public event ErrorInterfaceEventHandler InterfaceEvent;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Evento o errore da Interfaccia sincronizzato con il controllo</summary>
        //-------------------------------------------------------------------------------------------------------------
        private void EventoInt(EventInterfaccia evn,FrameInterfaccia fdic)
        {
            if (InterfaceEvent != null)
            {
                if (InterfaceEvent.Target is Control && ((Control)InterfaceEvent.Target).InvokeRequired)
                {
                    //Se il Targhet è un oggetto Control e ha bisogno di essere sincronizzato
                    Control ControlloUI = (Control)InterfaceEvent.Target;
                    //Istanziamo il delegato per l'Evento
                    ErrorInterfaceEventHandler dlgEvnInt = new ErrorInterfaceEventHandler(InterfaceEvent);
                    //Eseguiamo l'evento sul thread del controllo in maniera asincrona (ritorna subito). 
                    ControlloUI.BeginInvoke(dlgEvnInt, new object[] { evn, fdic });
                }
                else InterfaceEvent(evn, fdic);
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Frame arrivato dall'interfaccia </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void RxFrame(FrameInterfaccia fint)
        {
            if (fint != null)
            {
                if (fint.Comando == ComFrame.EVENT)
                {
                    EventoInt(EventInterfaccia.FRAME_DA_REMOTO, fint);
                }
                else
                {
                    frameDaInt = fint;
                    autoEventInterface.Set();   //Evento ricezione messaggio da interfaccia segnalato
                }
            }
        }

        #region Connetti tramite FTDI 

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Connetti con Interfaccia FTDI isolata </summary>
        //-------------------------------------------------------------------------------------------------------------
        private bool RIFcomFTDI(string Descrittore, uint baudrate)
        {
            if (!myFtdiDevice.IsOpen)
            {
                ftStatus = OpenFTDI(Descrittore, baudrate);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    monitor.ScriviSuMonitor(String.Format("Interfaccia USB {0} connessa a: {1}", Descrittore, baudrate));
                    StatoCon = TypeCon.FTDI;
                    Connessa = true;
                    BaudRate = baudrate;
                    return true;
                }
                else return false;
            }
            else return false;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Controlla se FDTI è collegato a PC </summary>
        //-------------------------------------------------------------------------------------------------------------
        public bool TestFDTI(string Descrittore)
        {
            if (StatoCon == TypeCon.FTDI)
            {
                string Descrizione;
                ftStatus = myFtdiDevice.GetDescription(out Descrizione);
                if (ftStatus == FTDI.FT_STATUS.FT_OK && Descrizione == Descrittore) return true;
                else return false;
            }
            else return true;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Modifica baudrate </summary>
        //-------------------------------------------------------------------------------------------------------------
        public bool SetBaudRate(uint baudrate)
        {
            myFtdiDevice.SetBaudRate(baudrate);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                monitor.ScriviSuMonitor("Modificata velocità seriale a baud: {0}", baudrate);
                return true;
            }
            else return false;
        }


        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Apre il dispositivo FTDI corrispondente alla descrizione e lancia il thread di ascolto </summary>
        //-------------------------------------------------------------------------------------------------------------
        private FTDI.FT_STATUS OpenFTDI(string Descrizione, uint baudrate)
        {
            ftStatus = myFtdiDevice.OpenByDescription(Descrizione);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                // Set baud rate
                ftStatus = myFtdiDevice.SetBaudRate(baudrate);
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return ftStatus;
                // Set data characteristics - 8n1
                ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return ftStatus;
                // Set flow control - None
                ftStatus = myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0x11, 0x13);
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return ftStatus;
                // Set read timeout to 5 seconds, write timeout to infinite
                ftStatus = myFtdiDevice.SetTimeouts(5000, 0);
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return ftStatus;
                //................................................................................
                //  TIMER per sincronizzazione messaggio seriale
                //................................................................................
                Frame_Timer = new System.Timers.Timer(500);
                Frame_Timer.Elapsed += Frame_TimeOver;
                Frame_Timer.AutoReset = false;
                Frame_Timer.Enabled = false;
                //................................................................................
                // Start BackgroundWorker (thread di ascolto)
                //................................................................................
                receivedDataEvent = new AutoResetEvent(false);
                ftStatus = myFtdiDevice.SetEventNotification(FTDI.FT_EVENTS.FT_EVENT_RXCHAR, receivedDataEvent);
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return ftStatus;
                dataReceivedHandler = new BackgroundWorker();
                dataReceivedHandler.DoWork += ReadData;
                dataReceivedHandler.RunWorkerCompleted += EndReadData;
                if (!dataReceivedHandler.IsBusy) dataReceivedHandler.RunWorkerAsync();
                else return FTDI.FT_STATUS.FT_OTHER_ERROR;
            }
            return ftStatus;
        }

        /*-------------------------------------------------------------------------------------------------------------
        * Frame UART <Comando> <Add_L> <Add_H> <nByte> <b0> <b1> .. <bn> <CheckSum>;  (nByte 1-255, 0=256)
        * 
        * CheckSum = -{ <Comando> + <Start_L> + <Start_H> + <nByte> + <b0> + <b1> + .. + <bn> }
        */
        FrameInterfaccia msg_daZ80 = null;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Thread in ascolto dell'evento FT_EVENT_RXCHAR </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void ReadData(object pSender, DoWorkEventArgs pEventArgs)
        {
            int CheckSum = 0;
            int CntByte = 0;
            //int Lun=0;
            byte[] Msg = { };
            UInt32 nrOfBytesAvailable = 0;
            UInt32 nByteRead = 0;
            FTDI.FT_STATUS status;

            while (true)
            {
                receivedDataEvent.WaitOne();                                                //Attendi arrivo byte da FTDI
                byte err = 0;                                                               //
                status = myFtdiDevice.GetLineStatus(ref err);                               //Controlla errori di Overrun-Framing-Parity
                if (status != FTDI.FT_STATUS.FT_OK) return;                                 //Esci se non riesce a leggere da myFtdiDevice
                if ((err & 0x0E) != 0) EventoInt(EventInterfaccia.ERR_LINE_RX, msg_daZ80);        //
                status = myFtdiDevice.GetRxBytesAvailable(ref nrOfBytesAvailable);          //Legge il numero di byte disponibili
                if (status != FTDI.FT_STATUS.FT_OK) return;                                 //Esci se non riesce a leggere da myFtdiDevice
                byte[] readData = new byte[nrOfBytesAvailable];                             //Crea l'Array di lettura
                status = myFtdiDevice.Read(readData, nrOfBytesAvailable, ref nByteRead);    //Legge i byte dal buffer
                if (status != FTDI.FT_STATUS.FT_OK) return;                                 //Esci se non riesce a leggere da myFtdiDevice

                for (int i = 0; i < nByteRead; i++)                                         //Parsing dei byte in arrivo
                {
                    switch (statoFrame)
                    {
                        case StatoFrame.COMANDO:
                            statoFrame = StatoFrame.ADDL;
                            msg_daZ80 = new FrameInterfaccia();
                            CntByte = 0;
                            CheckSum = readData[i];
                            msg_daZ80.Comando = (ComFrame)readData[i];
                            Frame_Timer.Interval = TimeOutRx;                               //Imposta timeout     
                            Frame_Timer.Enabled = true;                                     //Start timer per timeout di lettura frame
                            //System.Threading.Thread.Sleep(20);                            //Test per generare errore time out
                            break;
                        case StatoFrame.ADDL:
                            statoFrame = StatoFrame.ADDH;
                            msg_daZ80.Add = readData[i];
                            CheckSum += readData[i];
                            break;
                        case StatoFrame.ADDH:
                            statoFrame = StatoFrame.NUM_BYTE;
                            msg_daZ80.Add += readData[i] * 256U;
                            CheckSum += readData[i];
                            break;
                        case StatoFrame.NUM_BYTE:
                            statoFrame = StatoFrame.BYTE_MSG;
                            if (readData[i] == 0) msg_daZ80.Msg = new byte[256];
                            else msg_daZ80.Msg = new byte[readData[i]];
                            CheckSum += readData[i];
                            break;
                        case StatoFrame.BYTE_MSG:
                            CntByte++;
                            CheckSum += readData[i];
                            if (CntByte == (msg_daZ80.Msg.Length + 1))
                            {
                                // Ricezione frame completata
                                Frame_Timer.Enabled = false;
                                statoFrame = StatoFrame.COMANDO;
                                if ((CheckSum & 0xFF) != 0) EventoInt(EventInterfaccia.ERR_CHKSUM_RX, msg_daZ80);
                                else RxFrame(msg_daZ80);
                            }
                            else msg_daZ80.Msg[CntByte - 1] = readData[i];
                            break;
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Termine processo in attesa dati da FTDI su errore</summary>
        //-------------------------------------------------------------------------------------------------------------
        private void EndReadData(object sender, RunWorkerCompletedEventArgs e)
        {
            //DisConnetti();
            EventoInt(EventInterfaccia.ERR_FTDI, msg_daZ80);
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Disconnetti interfaccia isolata FTDI </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void CloseFTDI()
        {
            if (myFtdiDevice.IsOpen)
            {
                myFtdiDevice.Close();
                StatoCon = TypeCon.NON_CONNESSO;
                monitor.ScriviSuMonitor("Interfaccia FTDI disconnessa");
                Connessa = false;
                //dataReceivedHandler.CancelAsync();
            }
        }

        #endregion

        #region Connetti tramite porta COM RS232

        private readonly SerialPort serialPort = new SerialPort();

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Connetti tramite RS232  </summary>
        //-------------------------------------------------------------------------------------------------------------
        private bool RIFcomRS232(string com,uint baudRate)
        {
            if (StatoCon == TypeCon.NON_CONNESSO)
            {
                //------------------------- APERTURA SERIALE COM1 --------------------------------------
                serialPort.PortName = com; // esempio "COM1"
                serialPort.BaudRate = (int)baudRate;
                serialPort.ReceivedBytesThreshold = 1;
                //  serialPort.ReadBufferSize = 4096; //Default value 
                serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DatiRicevuti);
                serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPort_Error);
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    monitor.ScriviSuMonitor("Porta " + serialPort.PortName.ToString() + " aperta");

                    //................................................................................
                    //  TIMER per sincronizzazione messaggio seriale
                    //................................................................................
                    Frame_Timer = new System.Timers.Timer(500);
                    Frame_Timer.Elapsed += Frame_TimeOver;
                    Frame_Timer.AutoReset = false;
                    Frame_Timer.Enabled = false;

                    StatoCon = TypeCon.RS232;
                    Connessa = true;
                    BaudRate = baudRate;
                    return true;
                }
                else return false;
            }
            else return false;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>  Errori di ricezione sulla seriale RS232  </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void SerialPort_Error(object sender, SerialErrorReceivedEventArgs SerErr)
        {
            monitor.ScriviSuMonitor("Errore seriale: " + SerErr.EventType);
        }


        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Chiude la connessione con RS232  </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void CloseRS232()
        {
            if (serialPort.IsOpen) serialPort.Close(); //Chiude la porta corrente se è aperta
            while (serialPort.IsOpen) ; //Attende la chiusura della COM1
            StatoCon = TypeCon.NON_CONNESSO;
            Frame_Timer.Dispose();
            serialPort.Dispose();
            monitor.ScriviSuMonitor("Porta COM1 chiusa");
            Connessa = false;
        }

        /*-------------------------------------------------------------------------------------------------------------
        * Frame UART <Comando> <Add_L> <Add_H> <nByte> <b0> <b1> .. <bn> <CheckSum>;  (nByte 1-255, 0=256)
        * 
        * CheckSum = -{ <Comando> + <Start_L> + <Start_H> + <nByte> + <b0> + <b1> + .. + <bn> }
        */
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>  CallBack dati ricevuti sulla porta Seriale  </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void SerialPort_DatiRicevuti(object sender, SerialDataReceivedEventArgs e)
        {
            int CheckSum = 0;
            int CntByte = 0;
            int nByteRead = 0;
            do
            {
                nByteRead = serialPort.BytesToRead;
                if (nByteRead > 0)
                {
                    byte[] readData = new byte[nByteRead];                                     //Attendi arrivo byte da FTDI
                    serialPort.Read(readData, 0, nByteRead);
                    for (int i = 0; i < nByteRead; i++)                                         //Parsing dei byte in arrivo
                    {
                        switch (statoFrame)
                        {
                            case StatoFrame.COMANDO:
                                statoFrame = StatoFrame.ADDL;
                                msg_daZ80 = new FrameInterfaccia();
                                CntByte = 0;
                                CheckSum = readData[i];
                                msg_daZ80.Comando = (ComFrame)readData[i];
                                Frame_Timer.Interval = TimeOutRx;                             //Imposta timeout     
                                Frame_Timer.Enabled = true;                                   //Start timer per timeout di lettura frame
                                //Thread.Sleep(500);                                          //Per test di funzionamento time out  
                                break;
                            case StatoFrame.ADDL:
                                statoFrame = StatoFrame.ADDH;
                                msg_daZ80.Add = readData[i];
                                CheckSum += readData[i];
                                break;
                            case StatoFrame.ADDH:
                                statoFrame = StatoFrame.NUM_BYTE;
                                msg_daZ80.Add += readData[i] * 256U;
                                CheckSum += readData[i];
                                break;
                            case StatoFrame.NUM_BYTE:
                                statoFrame = StatoFrame.BYTE_MSG;
                                if (readData[i] == 0) msg_daZ80.Msg = new byte[256];
                                else msg_daZ80.Msg = new byte[readData[i]];
                                CheckSum += readData[i];
                                break;
                            case StatoFrame.BYTE_MSG:
                                CntByte++;
                                CheckSum += readData[i];
                                if (CntByte == (msg_daZ80.Msg.Length + 1))
                                {
                                    // Ricezione frame completata
                                    Frame_Timer.Enabled = false;
                                    statoFrame = StatoFrame.COMANDO;
                                    if ((CheckSum & 0xFF) != 0) EventoInt(EventInterfaccia.ERR_CHKSUM_RX, msg_daZ80);
                                    else RxFrame(msg_daZ80);
                                }
                                else msg_daZ80.Msg[CntByte - 1] = readData[i];
                                break;
                        }
                    }
                }
            } while (Frame_Timer.Enabled);
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Invia frame seriale a Interfaccia </summary>
        //-------------------------------------------------------------------------------------------------------------
        public FrameInterfaccia InviaFrame(FrameInterfaccia frameInterface, bool att_ris)
        {
            return InviaFrame(frameInterface, att_ris, 1000);  //Attende risposta in mSec
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Invia frame seriale a Interfaccia specificando il tempo di attesa </summary>
        //-------------------------------------------------------------------------------------------------------------
        public FrameInterfaccia InviaFrame(FrameInterfaccia frameInterface, bool att_ris, int tAtt)
        {
            uint nByteScritti = 0;
            mut_a.WaitOne();   //Permette l'accesso di un thread alla volta
            FrameInterfaccia retMsg;
            if (Connessa)
            {
                byte[] fmar = FormatFrameInterface(frameInterface);

                if (StatoCon == TypeCon.RS232)
                    serialPort.Write(fmar, 0, fmar.Length);
                else if (StatoCon == TypeCon.FTDI)
                    myFtdiDevice.Write(fmar, fmar.Length, ref nByteScritti); //Invia messaggio sul device FTDI
                
                if (att_ris)
                {
                    // autoEventInterface.Reset();
                    if (autoEventInterface.WaitOne(tAtt)) retMsg = frameDaInt;
                    else retMsg = null;
                }
                else retMsg = null;
            }
            else retMsg = null;
            mut_a.ReleaseMutex(); // Rilascia il Mutex
            return retMsg;
        }



        /*-------------------------------------------------------------------------------------------------------------
        * Frame UART <Comando> <Add_L> <Add_H> <nByte> <b0> <b1> .. <bn> <CheckSum>;  (nByte 1-255, 0=256)
        * 
        * CheckSum = -{ <Comando> + <Start_L> + <Start_H> + <nByte> + <b0> + <b1> + .. + <bn> }
        */
        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Formatta il frame secondo il protocollo inserendo il checksum </summary>
        //-------------------------------------------------------------------------------------------------------------
        private byte[] FormatFrameInterface(FrameInterfaccia frame)
        {
            byte[] msg;
            int CheckSum;
            if (frame.Msg.Length > 256) throw new System.InvalidOperationException("Lunghezza Messaggio > 256 byte");
            if (frame.Msg.Length == 0) throw new System.InvalidOperationException("Lunghezza Messaggio = 0 byte");

            msg = new byte[frame.Msg.Length + 5];
            msg[0] = (byte)(frame.Comando);
            msg[1] = (byte)(frame.Add & 0xFF);
            msg[2] = (byte)((frame.Add >> 8) & 0xFF);
            msg[3] = (byte)frame.Msg.Length;
            CheckSum = msg[0] + msg[1] + msg[2] + msg[3];
            for (int i = 0; i < frame.Msg.Length; i++)
            {
                msg[i + 4] = frame.Msg[i];
                CheckSum += frame.Msg[i];
            }
            //CheckSum++; //test errore di checksum
            msg[msg.Length - 1] = (byte)(-CheckSum & 0xFF);  //Inserisci valore calcolato di Checksum

            return msg;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Il messaggio non si è concluso nel tempo previsto </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void Frame_TimeOver(Object source, ElapsedEventArgs e)
        {
            statoFrame = StatoFrame.COMANDO;   //Resetta buffer in attese del prossimo messaggio valido
            Frame_Timer.Enabled = false;
            EventoInt(EventInterfaccia.ERR_TIMEOUT_RX, msg_daZ80);
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Invia impulso negativo DTR di durata 82uSec  </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void Reset()
        {
            switch (StatoCon)
            {
                case TypeCon.FTDI:
                    myFtdiDevice.SetDTR(true);
                    myFtdiDevice.SetDTR(false);
                   break;
                case TypeCon.RS232:
                    serialPort.DtrEnable = true;
                    serialPort.DtrEnable = false;
                    break;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Invia impulso negativo RTS di durata 82uSec  </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void NMI()
        {
            switch (StatoCon)
            {
                case TypeCon.FTDI:
                    myFtdiDevice.SetRTS(true);
                    myFtdiDevice.SetRTS(false);
                    break;
                case TypeCon.RS232:
                    serialPort.RtsEnable = true;
                    serialPort.RtsEnable = false;
                    break;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Comando di BOOT da inviare entro 300mSec dalla ricezione di EVN_RESET </summary>
        //-------------------------------------------------------------------------------------------------------------
        public bool EntraInBoot()
        {
            FrameInterfaccia frm = new FrameInterfaccia()
            {
                Comando = ComFrame.COM_BOOT,
                Add = 0,
                Msg = new byte[1] { 0xD8 }
            };
            FrameInterfaccia ris = this.InviaFrame(frm, true);
            if (ris != null && ris.Comando == frm.Comando && ris.Msg[0] == 0xD8) return true;
            else return false;
        }


    }
    
}
