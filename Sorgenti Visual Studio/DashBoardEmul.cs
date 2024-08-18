using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Timers;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms.DataVisualization.Charting;

namespace Andrea_NameSpace
{

    public enum EventoDaRemoto : byte
    {
        EVN_RESET = 0,             //Device Remoto si e' resettato
        EVN_ERREP_FRAMING = 0x10,
        EVN_ERREP_OVERRUN = 0x11,
        EVN_ERREP_TIMEOUT = 0x12,
        EVN_ERREP_BUFFULL = 0x13,
        EVN_ERREP_CHKSUM = 0x14

    }

    public enum SizeEprom
    {
        EP_2708_1K,
        EP_2716_2K,
        EP_2732_4K,
        EP_2764_8K,
        EP_27128_16K,
        EP_27256_32K,
        EP_27512_64K
    }

    public partial class DashBoardEmulEP : Form
    {


        private enum WriteReport : byte
        {
            WRITE_OK = 0,
            WRITE_ERROR = 1,
            WRITE_FLASH_NO_NEED = 2,
            WRITE_FLASH_OVERDATA = 3,
            WRITE_ADDRESS_ERROR = 4
        }

        public Preference preference = new Preference();
        XmlSerializer serializer = new XmlSerializer(typeof(Preference));

        InterfacciaComm InterfEmulEP;
        uCMem Pic18F14K22;
        uCMem Z80;
        //string DescrittoreFTDI;
        bool ImmagineOnPC = false;
        bool AttesaBoot = false;
        bool EmulInBootLoader = false;
        bool stop = false;
        byte[] ConfigDevice;

        System.Timers.Timer tmr1; //Timer per File Watcher

        private VisualizzaHexRAM vhex;

        private int fireCount = 0;
        FileSystemWatcher fswAsm;
        FileSystemWatcher fswHex;
        FileSystemWatcher fswBin;

        private static readonly uint[] OffSet = new uint[] { 0xF400, 0xF800, 0xF000, 0xE000, 0xC000, 0x8000, 0x0000 };

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Costruttore della classe </summary>
        //-------------------------------------------------------------------------------------------------------------
        public DashBoardEmulEP()
        {
            InitializeComponent();

            cbSizeEP.DataSource = Enum.GetValues(typeof(SizeEprom));
            cbSizeEP.SelectedIndex = cbSizeEP.Items.IndexOf(SizeEprom.EP_27512_64K);

            InterfEmulEP = new InterfacciaComm(monitor);
            InterfEmulEP.InterfaceEvent += EventDaInt;
            InterfEmulEP.TimeOutRx = 100;  //Tempo massimo ricezione Frame
            Pic18F14K22 = new uCMem(uCMem.uCType.PIC18F14K22, 0x800);
            Z80 = new uCMem(uCMem.uCType.Z80, 0x0000);

            tabControl.TabPages.Clear();
            
            tabControl.TabPages.Add(tabEmulEP);
            tabControl.TabPages.Add(tabFirmware);
            
            tmrInterfaccia.Start();
            monitor.ClearMonitor();

            //Instanzia Timer per File Watcher
            tmr1 = new System.Timers.Timer(500);
            tmr1.Elapsed += new System.Timers.ElapsedEventHandler(tmr1_Tick);
            tmr1.AutoReset = false;

            LoadPreference();
        }

        //-----------------------------------------------------------------------------------------
        // Evento generato quando una tab è selezionata
        //-----------------------------------------------------------------------------------------
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage != null)
            {

                switch (e.TabPage.Name)
                {
                    case "tabBootLD":
                        {

                        }
                        break;
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        // Evento generato quando una tab è deselezionata
        //-----------------------------------------------------------------------------------------
        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage != null)
            {
                switch (e.TabPage.Name)
                {
                    case "tabBootLD":
                        {

                        }
                        break;
                }
            }
        }

        #region InterfacciaFTDI #######################################################################################

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Disconnetti da Z80 </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void Disconnetti()
        {
            if (InterfEmulEP.DisConnetti())
            {
                monitor.ScriviSuMonitor("Disconnessione effettuata");
                toolStripStatusLabel1.Text = "Scheda Disconnessa";
                //tabControl1.TabPages.Clear();
            }
            else monitor.ScriviSuMonitor("Errore disconnessione");
        }

        //-----------------------------------------------------------------------------------------
        // Timer per attesa dell'interfaccia USB
        //-----------------------------------------------------------------------------------------
        bool lamp = true;
        private void tmrInterfaccia_Tick(object sender, EventArgs e)
        {
            bool statocon = false;
            statocon = InterfEmulEP.Connetti(InterfacciaComm.TypeCon.FTDI, toolStripTxtBoxDescrittore.Text, 2000000 );
            if (statocon)
            {
                toolStripStatusLabel3.Text = "Baud: " + InterfEmulEP.BaudRate.ToString();
                monitor.ScriviSuMonitor("Connesso con interfaccia");
                toolStripStatusLabel1.Text = "Connesso con interfaccia";


                FrameInterfaccia frm = new FrameInterfaccia
                {
                    Comando = ComFrame.COM_START,
                    Add = 0,
                    Msg = new byte[] { 0xD8 }
                };
                InterfEmulEP.InviaFrame(frm, true);
                monitor.ScriviSuMonitor("Emulatore in RUN");
            }
            else
            {
                if (InterfEmulEP.Connessa)
                {
                    // Controllo se viene disconnesso cavo USB
                    if (!InterfEmulEP.TestFDTI(toolStripTxtBoxDescrittore.Text))
                    {
                        //Z80_Ram.LinkZ80(false);
                        Disconnetti();
                    }
                }
                else
                {
                    if (lamp)
                    {
                        tmrInterfaccia.Enabled = true;
                        toolStripStatusLabel1.Text = "In attesa di: " + toolStripTxtBoxDescrittore.Text;
                    }
                    else toolStripStatusLabel1.Text = "";
                    lamp = !lamp;
                }
            }
        }


        private void FTDIToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (FTDIToolStripMenuItem.Checked == true)
            {
                InterfEmulEP.DisConnetti();
                FTDIToolStripMenuItem.CheckOnClick = false;
            }
        }

        private void toolStripTxtBoxDescrittore_Validated(object sender, EventArgs e)
        {
            InterfEmulEP.DisConnetti();
        }

        int nloop = 0;

        Random RandomByte = new Random();


        private void btnLoop_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) timer1.Stop();
            else
            {
                monitor.ScriviSuMonitor("\n---> Inizio Test link PC-Emulatore");
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            FrameInterfaccia frint = new FrameInterfaccia();
            frint.Comando = ComFrame.COM_LOOP;
            frint.Add = 0;
            if (EmulInBootLoader) frint.Msg = new byte[Pic18F14K22.DimErasePage];
            else frint.Msg = new byte[256];
            RandomByte.NextBytes(frint.Msg);

            FrameInterfaccia ris = InterfEmulEP.InviaFrame(frint, true);
            if (ris != null)
            {
                // monitor.ScriviSuMonitor("", ris.Msg);

                for (int i = 0; i < frint.Msg.Length; i++)
                {
                    if (frint.Msg[i] != ris.Msg[i])
                    {
                        monitor.ScriviSuMonitor("Errore nel byte: {0}", i);
                        MessageBox.Show("Errore di comunicazione a 2Mbit", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        timer1.Stop();
                    }
                }
                monitor.ScriviSuMonitorSR("Loop {0} O.K: {1}", frint.Msg.Length, nloop++);
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        // Evento in arrivo da interfaccia USB (sincronizzato con UI)
        //-------------------------------------------------------------------------------------------------------------
        private void EventDaInt(EventInterfaccia evn, FrameInterfaccia fic)
        {
            switch (evn)
            {
                case EventInterfaccia.ERR_FTDI:
                    monitor.ScriviSuMonitor("Errore di accesso FTDI in ricezione");
                    //if(fic != null) monitor.ScriviSuMonitor("", fic.Msg);
                    break;
                case EventInterfaccia.ERR_LINE_RX:
                    monitor.ScriviSuMonitor("Errore di (Overrun-Framing-Parity) FDTI in ricezione da uC");
                    //if (fic != null) monitor.ScriviSuMonitor("", fic.Msg);
                    break;
                case EventInterfaccia.ERR_CHKSUM_RX:
                    monitor.ScriviSuMonitor("Errore di check sum FDTI in ricezione da uC");
                    monitor.ScriviSuMonitor("",fic.Msg);
                    break;
                case EventInterfaccia.ERR_TIMEOUT_RX:
                    monitor.ScriviSuMonitor("Errore di TimeOut FDTI in ricezione da uC");
                    break;
                case EventInterfaccia.FRAME_DA_REMOTO:
                    EventoDaRemoto(fic);
                    break;
            }
        }

 


        //-------------------------------------------------------------------------------------------------------------
        // Evento in arrivo da device remoto sincronizzato con interfaccia utente
        //-------------------------------------------------------------------------------------------------------------
        private void EventoDaRemoto(FrameInterfaccia fic)
        {
            switch ((EventoDaRemoto)fic.Msg[0])
            {
                case Andrea_NameSpace.EventoDaRemoto.EVN_ERREP_TIMEOUT:
                    monitor.ScriviSuMonitor("Errore di TimeOut, frame incompleto ricevuto da device remoto. Address: 0x{0:X4}", fic.Add);
                    break;
                case Andrea_NameSpace.EventoDaRemoto.EVN_ERREP_CHKSUM:
                    monitor.ScriviSuMonitor("Errore di chksum ricevuto da device remoto");
                    break;
                case Andrea_NameSpace.EventoDaRemoto.EVN_ERREP_FRAMING:
                    monitor.ScriviSuMonitor("Errore di Framing ricevuto da device remoto");
                    break;
                case Andrea_NameSpace.EventoDaRemoto.EVN_ERREP_BUFFULL:
                    monitor.ScriviSuMonitor("Errore di buffer full ricevuto da device remoto");
                    break;
                case Andrea_NameSpace.EventoDaRemoto.EVN_ERREP_OVERRUN:
                    monitor.ScriviSuMonitor("Errore di overrun ricevuto da device remoto");
                    break;
                case Andrea_NameSpace.EventoDaRemoto.EVN_RESET:
                    monitor.ScriviSuMonitor("Il device remoto si è resettato");
                    if (AttesaBoot)
                    {
                        AttesaBoot = false;
                        if (InterfEmulEP.EntraInBoot())
                        {
                            monitor.ScriviSuMonitor("Device remoto in bootloader");
                            EmulInBootLoader = true;
                            ReadConfig();
                            btnLeggiFirmware.Enabled = true;
                        }
                        else monitor.ScriviSuMonitor("Errore accesso al bootloader");
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Upload Firmware  ###################################################################################################

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Legge Flash  Indirizzo di Inizio e array di destinazione (in RUN e in BOOTLOADER) </summary>
        //-------------------------------------------------------------------------------------------------------------
        public ErrorNodo ReadFlash(uint StartAdd, ref byte[] Flash)
        {
            FrameInterfaccia ris = null;
            uint icorr = 0;

            stop = false;
            while (!stop)
            {
                uint nByte = (uint)Flash.Length - icorr;
                if (nByte == 0) break;
                if (nByte > 256) nByte = 256;

                FrameInterfaccia frm = new FrameInterfaccia
                {
                    Comando = ComFrame.COM_READ_FLASH,
                    Add = StartAdd,
                    Msg = new byte[] { (byte)(nByte) }   //Num Byte
                };
                ris = InterfEmulEP.InviaFrame(frm, true);
                if (ris == null || ris.Comando != ComFrame.COM_READ_FLASH) break;

                Array.Copy(ris.Msg, 0, Flash, icorr, nByte);
                icorr += nByte;
                StartAdd += nByte;
                monitor.ScriviSuMonitorSR("Letto fino a: 0x{0:X5}", StartAdd);
            }
            if (stop) return ErrorNodo.STOP_UTENTE;
            if (ris == null) return ErrorNodo.NODO_NON_RISPONDE;
            return ErrorNodo.NO_ERROR;
        }


        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive Flash Indirizzo di Inizio e array di origine (in RUN e in BOOTLOADER) </summary>
        //-------------------------------------------------------------------------------------------------------------
        public ErrorNodo WriteFlash(uint StartAdd, byte[] Flash)
        {
            FrameInterfaccia ris = null;
            uint icorr = 0;
            uint ByteScritti = 0;
            stop = false;
            while (!stop)
            {
                
                uint nByte = (uint)Flash.Length - icorr;
                uint residuo = Pic18F14K22.DimErasePage - (StartAdd % Pic18F14K22.DimErasePage);  //Quanti byte mancano per arrivare alla fine della pagina di scrittura (in questo caso 256 byte))
                if (nByte > residuo) nByte = residuo;
                if (nByte == 0) break;

                FrameInterfaccia frm = new FrameInterfaccia
                {
                    Comando = ComFrame.COM_WRITE_FLASH,
                    Add = StartAdd,
                    Msg = new byte[nByte]
                };
                Array.Copy(Flash, icorr, frm.Msg, 0, nByte);

                ris = InterfEmulEP.InviaFrame(frm, true);

                if (ris == null) return ErrorNodo.NODO_NON_RISPONDE;
                else if (ris.Comando != ComFrame.COM_WRITE_FLASH) return ErrorNodo.RISP_COM_ERRATO;
                else if ((WriteReport)ris.Msg[0] == WriteReport.WRITE_ADDRESS_ERROR)
                {
                    monitor.ScriviSuMonitorSR("Scrittura in zona protetta! Add: 0x{0:X4}", StartAdd);
                    MessageBox.Show("Scrittura in zona protetta!", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ErrorNodo.ERRORE_SCRITTURA;
                }
                else if ((WriteReport)ris.Msg[0] == WriteReport.WRITE_ERROR)
                {
                    monitor.ScriviSuMonitorSR("Verifica non superata: Add: 0x{0:X4}", StartAdd);
                    MessageBox.Show("Verifica non superata!", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ErrorNodo.ERRORE_SCRITTURA;
                }
                else if ((WriteReport)ris.Msg[0] == WriteReport.WRITE_FLASH_OVERDATA)
                {
                    monitor.ScriviSuMonitorSR("Dati al di là della pagina: Add: 0x{0:X4}", StartAdd);
                    MessageBox.Show("Dati al di là della pagina!", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ErrorNodo.ERRORE_SCRITTURA;
                }

                ByteScritti += nByte;
                icorr += nByte;
                StartAdd += nByte;
                monitor.ScriviSuMonitorSR("Scritto fino a: 0x{0:X5}", StartAdd);
            }
            monitor.ScriviSuMonitorSR("Scritti {0} byte.", ByteScritti);
            if (stop) return ErrorNodo.STOP_UTENTE;
            else return ErrorNodo.NO_ERROR;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Legge EEprom  Indirizzo di Inizio e array di destinazione (in RUN e in BOOTLOADER) </summary>
        //-------------------------------------------------------------------------------------------------------------
        public ErrorNodo ReadEE(uint StartAdd, ref byte[] EEprom)
        {
            FrameInterfaccia ris = null;
            uint icorr = 0;

            stop = false;
            while (!stop)
            {
                uint nByte = (uint)EEprom.Length - icorr;
                if (nByte == 0) break;
                if (nByte > 256) nByte = 256;

                FrameInterfaccia frm = new FrameInterfaccia
                {
                    Comando = ComFrame.COM_READ_EE,
                    Add = StartAdd,
                    Msg = new byte[] { (byte)(nByte) }   //Num Byte
                };
                ris = InterfEmulEP.InviaFrame(frm, true);
                if (ris == null || ris.Comando != ComFrame.COM_READ_EE) break;

                Array.Copy(ris.Msg, 0, EEprom, icorr, nByte);
                icorr += nByte;
                StartAdd += nByte;
                monitor.ScriviSuMonitorSR("Letto fino a: 0x{0:X4}", StartAdd);
            }
            if (stop) return ErrorNodo.STOP_UTENTE;
            if (ris == null) return ErrorNodo.NODO_NON_RISPONDE;
            return ErrorNodo.NO_ERROR;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive EEprom  Indirizzo di Inizio e array di destinazione (in RUN e in BOOTLOADER) </summary>
        //-------------------------------------------------------------------------------------------------------------
        public ErrorNodo WriteEE(uint StartAdd, byte[] EEprom)
        {
            FrameInterfaccia ris = null;
            uint icorr = 0;

            stop = false;
            while (!stop)
            {
                uint nByte = (uint)EEprom.Length - icorr;
                if (nByte == 0) break;
                if (nByte > Pic18F14K22.DimErasePage) nByte = Pic18F14K22.DimErasePage;

                FrameInterfaccia frm = new FrameInterfaccia
                {
                    Comando = ComFrame.COM_WRITE_EE,
                    Add = StartAdd,
                    Msg = new byte[nByte]
                };
                Array.Copy(EEprom, icorr, frm.Msg, 0, nByte);

                ris = InterfEmulEP.InviaFrame(frm, true);

                if (ris == null) return ErrorNodo.NODO_NON_RISPONDE;
                else if (ris.Comando != ComFrame.COM_WRITE_EE) return ErrorNodo.RISP_COM_ERRATO;
                else if ((WriteReport)ris.Msg[0] == WriteReport.WRITE_ERROR)
                {
                    monitor.ScriviSuMonitorSR("Verifica non superata: Add: 0x{0:X4}", StartAdd);
                    MessageBox.Show("Verifica non superata!", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ErrorNodo.ERRORE_SCRITTURA;
                }
                icorr += nByte;
                StartAdd += nByte;
                monitor.ScriviSuMonitorSR("Scritto fino a: 0x{0:X4}", StartAdd);
            }
            monitor.ScriviSuMonitorSR("Scritt1: {0} byte.", EEprom.Length);
            if (stop) return ErrorNodo.STOP_UTENTE;
            if (ris == null) return ErrorNodo.NODO_NON_RISPONDE;
            return ErrorNodo.NO_ERROR;
        }

        

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Legge configurazione del device remoto (in bootLoader) </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void ReadConfig()
        {
            FrameInterfaccia frm = new FrameInterfaccia
            {
                Comando = ComFrame.COM_READ_CONFIG,
                Add = 0,
                Msg = new byte[] { 1 }
            };
            FrameInterfaccia ris = InterfEmulEP.InviaFrame(frm, true);
            if (ris != null)
            {
                lblVerDevice.Text = String.Format("Versione: {0}.{1}",ris.Msg[0],ris.Msg[2]);
                lblPicTypeDevice.Text = "PicType: " + ((uCMem.uCType)(ris.Msg[4] + ris.Msg[5] * 256)).ToString();
                int pos = Array.IndexOf(ris.Msg, (byte)0, 6,26);
                lblNomeFirmware.Text = "Nome: " + Encoding.ASCII.GetString(ris.Msg, 6, pos-6);

                ConfigDevice = new byte[16];
                Array.Copy(ris.Msg, 0x20, ConfigDevice, 0, ConfigDevice.Length);
                //monitor.ScriviSuMonitor("Configurazione Device: ", ConfigDevice);

                int DevID = (ris.Msg[49] << 3) | ((ris.Msg[48] >> 5) & 0x07);   //Non legge correttamente il DEVID ???
                monitor.ScriviSuMonitor("Device ID: 0x{0:X4}", DevID);
            }
        }

    

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Load Firmware on PC  </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void LoadFirm()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\" ;
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
            openFileDialog1.Filter = "Hex file (*.hex)|*.hex|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //monitor.ClearMonitor();

                uCMem.ReadHexResult res = Pic18F14K22.Read_uPMem(openFileDialog1.FileName);
                if (res != uCMem.ReadHexResult.OK) monitor.ScriviSuMonitor("Errore lettura file Hex: " + res);
                else
                {
                    lblVerDeviceOnPC.Text = String.Format("Versione: {0}.{1}", Pic18F14K22.PrgMem[0x800], Pic18F14K22.PrgMem[0x802]);
                    lblPicTypeDeviceOnPC.Text = "PicType: " + ((uCMem.uCType)(Pic18F14K22.PrgMem[0x804] + Pic18F14K22.PrgMem[0x805] * 256)).ToString();
                    int pos = Array.IndexOf(Pic18F14K22.PrgMem, (byte)0, 0x806, 26);
                    lblNomeFirmwareOnPC.Text = "Nome: " + Encoding.ASCII.GetString(Pic18F14K22.PrgMem, 0x806, pos - 0x806);

                    if (!Pic18F14K22.CnfBit.ValEqual(ConfigDevice))
                    {
                        DialogResult result = MessageBox.Show("La configurazione del Device è diversa. Non è possibile procedere", "Configuration Bit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        monitor.ScriviSuMonitor("Configuration bit on PC    : ", Pic18F14K22.CnfBit);
                        monitor.ScriviSuMonitor("Configuration bit on Device: ", ConfigDevice);
                        return;
                    }

                    if (lblNomeFirmwareOnPC.Text != lblNomeFirmware.Text)
                    {
                        DialogResult result = MessageBox.Show("Il nome del Firmware non corrisponde, vuoi procedere comunque?", "Nome Firmware", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                            btnUploadFirmware.Enabled = true;
                        else 
                            btnUploadFirmware.Enabled = false;
                    }
                    else
                    {
                        btnUploadFirmware.Enabled = true;
                        btnVerifyFirmware.Enabled = true;
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>Invia comando per entrare in bootloader </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void btnEnterBootLoader_Click(object sender, EventArgs e)
        {
            AttesaBoot = true;
            FrameInterfaccia frm = new FrameInterfaccia
            {
                Comando = ComFrame.COM_RESET,
                Add = 0,
                Msg = new byte[1]
            };
            InterfEmulEP.InviaFrame(frm, false);
            monitor.ScriviSuMonitor("Resettare Emulatore");
        }

        private void btnUploadFirm_Click(object sender, EventArgs e)
        {
            LoadFirm();
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> UpLoad Firmware </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void btnUploadFirmware_Click(object sender, EventArgs e)
        {
            byte[] DevFlash = new byte[Pic18F14K22.SupDiv - Pic18F14K22.IniPrg + 1];
            Array.Copy(Pic18F14K22.PrgMem, Pic18F14K22.IniPrg, DevFlash, 0, DevFlash.Length);
            DialogResult result = MessageBox.Show("Vuoi sovrascrivere il firmware?", "Scrittura Firmware", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                monitor.ScriviSuMonitor("Scrittura Firmware da indirizzo 0x{0:X4} a 0x{1:X4}", Pic18F14K22.IniPrg, Pic18F14K22.SupDiv);
                WriteFlash(Pic18F14K22.IniPrg, DevFlash);
            };
            result = MessageBox.Show("Vuoi sovrascrivere la EEprom?", "Scrittura EEprom", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                monitor.ScriviSuMonitor("Scrittura EEprom da indirizzo 0x{0:X4} a 0x{1:X4}", 0, Pic18F14K22.EEprom.Length);
                WriteEE(0, Pic18F14K22.EEprom);
            }
            btnStartFirmware.Enabled = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Verify Firmware </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void btnVerifyFirmware_Click(object sender, EventArgs e)
        {
            byte[] DevFlash = new byte[Pic18F14K22.SupDiv - Pic18F14K22.IniPrg + 1];
            ReadFlash(Pic18F14K22.IniPrg, ref DevFlash);
            if (DevFlash.ValEqual(Pic18F14K22.PrgMem, Pic18F14K22.IniPrg))
            {
                monitor.ScriviSuMonitor("Verifica superata con successo");
            }
            else
            {
                monitor.ScriviSuMonitor("Errore di Verifica");
                MessageBox.Show("Errore di verifica", "Errore caricamento Firmware", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Start Firmware </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void btnStartFirmware_Click(object sender, EventArgs e)
        {
            FrameInterfaccia frm = new FrameInterfaccia
            {
                Comando = ComFrame.COM_START,
                Add = 0,
                Msg = new byte[] { 0xD8 }
            };
            FrameInterfaccia ris = InterfEmulEP.InviaFrame(frm, true);
            if (ris != null && ris.Msg[0] == 0xD8)
            {
                EmulInBootLoader = false;
                monitor.ScriviSuMonitor("Emulatore in RUN");
                btnStartFirmware.Enabled = false;
                btnVerifyFirmware.Enabled = false;
                btnStartFirmware.Enabled = false;
                btnLeggiFirmware.Enabled = false;
                btnUploadFirmware.Enabled = false;
            }
            else monitor.ScriviSuMonitor("ERRORE avvio firmware");
        }

        #endregion Upload Firmware

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Timeout per file watcher </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void tmr1_Tick(object sender, ElapsedEventArgs e)
        {
            fireCount = 0;
            tmr1.Stop();
        }

        #region File ASM ##############################################################################################

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Sceglie file assembler </summary>
        //-------------------------------------------------------------------------------------------------------------
        String AsmFilePath;
        private void txbAsmFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\" ;
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
            openFileDialog2.Filter = "asm file (*.asm)|*.asm|All Files (*.*)|*.*";
            openFileDialog2.FilterIndex = 1;
            openFileDialog2.RestoreDirectory = true;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                AsmFilePath = openFileDialog2.FileName;
                txbAsmFile.Text = Path.GetFileName(openFileDialog2.FileName);
                
                txbBinFile.Text = "";
                WatcherFileBin(null);

                txbHexFile.Text = "";
                WatcherFileHex(null);

                WatcherFileAsm(AsmFilePath);
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Attiva il FileSystemWatcher per il file Assembly </summary>
        //-------------------------------------------------------------------------------------------------------------
        
        private void WatcherFileAsm(string FileAsm)
        {
            if (FileAsm == null)
            {
                if (fswAsm != null) fswAsm.Dispose();
            }
            else
            {
                fswAsm = new FileSystemWatcher();
                fswAsm.Path = Path.GetDirectoryName(FileAsm);
                fswAsm.SynchronizingObject = this; //Sincronizza il watcher con questo thread (oggetto)
                fswAsm.NotifyFilter = NotifyFilters.LastWrite;
                fswAsm.Filter = Path.GetFileName(FileAsm);
                fswAsm.Changed += new FileSystemEventHandler(AsmChange);
                fireCount = 0;
                fswAsm.EnableRaisingEvents = true; //Abilita il controllo LastWrite 
            }
        }

       

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Evento generato alla modifica del file Assembly </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void AsmChange(object source, FileSystemEventArgs e)
        {
            fireCount++;
            if (fireCount == 1)
            {
                monitor.ClearMonitor();
                if (File.Exists(AsmFilePath))
                {
                    monitor.ScriviSuMonitor("File {0} Modificato ultimo accesso {1}", AsmFilePath, File.GetLastAccessTime(AsmFilePath).ToString("HH:mm:ss:FFF"));
                    if (chbAutoAssembler.Checked)
                    {
                        btnCompilaZasm.PerformClick();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(this, "File " + txbAsmFile.Text + " modificato vuoi Assemblarlo?", "File Change", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            btnCompilaZasm.PerformClick();
                        }
                    }

                }
                else monitor.ScriviSuMonitor("Il file {0} non esiste più", AsmFilePath);
                tmr1.Start();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Esegui Zasm per compilazione file Assembly </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void btnCompilaZasm_Click(object sender, EventArgs e)
        {
            monitor.ScriviSuMonitor("\n---> Compilazione sorgente con Zasm");
            try
            {
                using (Process AsmProc = new Process())
                {
                    AsmProc.StartInfo.FileName = @"zasm.exe";


                    AsmProc.StartInfo.UseShellExecute = false;
                    AsmProc.StartInfo.CreateNoWindow = false;
                    // Indirizza l'output sul monitor
                    AsmProc.StartInfo.RedirectStandardOutput = true;
                    AsmProc.StartInfo.RedirectStandardError = true;
                    // Imposta directory di lavoro
                    AsmProc.StartInfo.WorkingDirectory = Path.GetDirectoryName(AsmFilePath);

                    //string HexFile = Path.GetFileNameWithoutExtension(txbAsmFile.Text) + ".hex";
                    AsmProc.StartInfo.Arguments = txtbArgomentiZasm.Text + " " + txbAsmFile.Text;
                    AsmProc.Start();

                    StreamReader outreader = AsmProc.StandardOutput;
                    string output = outreader.ReadToEnd();

                    StreamReader errreader = AsmProc.StandardError;
                    string erroutput = errreader.ReadToEnd();

                    AsmProc.WaitForExit(10000);
                    if (AsmProc.ExitCode == 0)
                    {
                        monitor.ScriviSuMonitor(output);
                        monitor.ScriviSuMonitor(erroutput);
                        monitor.ScriviSuMonitor("Compilazione terminata con successo");
                        txbHexFile.Text = Path.GetFileNameWithoutExtension(AsmFilePath) + ".hex";
                        string PathFileHex = Path.Combine(Path.GetDirectoryName(AsmFilePath), txbHexFile.Text);
                        if (LoadFileHex(PathFileHex))
                            if (chbAutoLoad.Checked) btnLoadEmul_Click(null,null);
                        txbBinFile.Text = "";
                    }
                    else
                    {
                        monitor.ScriviSuMonitor(output);
                        monitor.ScriviSuMonitor(erroutput);
                        monitor.ScriviSuMonitor("Errore di compilazione, ExitCode {0}", AsmProc.ExitCode);
                        MessageBox.Show(this, "Errore di compilazione", "Errore di compilazione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception exc)
            {
                monitor.ScriviSuMonitor(exc.Message.ToString());
                MessageBox.Show(this, exc.Message.ToString(), "Errore di compilazione", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion File Assembler

        #region File HEX  #############################################################################################

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Carica in memoria il file Hex </summary>
        //-------------------------------------------------------------------------------------------------------------
        private bool LoadFileHex(string NomeFile)
        {
            if (ckbResetOnLoad.Checked) btnResetMem.PerformClick();
            monitor.ScriviSuMonitor("\n---> Lettura File *.hex");
            uCMem.ReadHexResult res = Z80.Read_uPMem(NomeFile);
            if (res != uCMem.ReadHexResult.OK)
            {
                monitor.ScriviSuMonitor("Errore lettura file Hex: " + res);
                MessageBox.Show(this, "Errore lettura file Hex: " + res, "Errore lettura file Hex", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                txbHexFile.Text = Path.GetFileName(NomeFile);

                monitor.ScriviSuMonitor("Lettura file Hex:  {0}", txbHexFile.Text);
                if (Z80.InfDiv <= Z80.SupDiv)
                {
                    monitor.ScriviSuMonitor("Intervallo Codici Macchina Caricati: 0x{0:X5} - 0x{1:X5}", Z80.InfDiv, Z80.SupDiv);
                    Z80.SetRangeAllMem((uint)nudResetValue.Value);
                    monitor.ScriviSuMonitor("Intervallo Codici Macchina in memoria PC: 0x{0:X5} - 0x{1:X5} diversi da 0x{2:X2}", Z80.InfMem, Z80.SupMem, (uint)nudResetValue.Value);
                    nudStartBin.Value = Z80.InfMem;
                    nudStopBin.Value = Z80.SupMem;
                }
                else monitor.ScriviSuMonitor("File Identico alla memoria PC!");
                return true;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Carica File Intel Hex </summary>
        //-------------------------------------------------------------------------------------------------------------
        String File_Path_Hex;
        private void txbHexFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\" ;
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
            openFileDialog1.Filter = "Hex file (*.hex)|*.hex|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //monitor.ClearMonitor();
                File_Path_Hex = openFileDialog1.FileName;
                if (LoadFileHex(File_Path_Hex))
                {
                    txbBinFile.Text = "";
                    WatcherFileBin(null);

                    txbAsmFile.Text = "";
                    WatcherFileAsm(null);

                    WatcherFileHex(File_Path_Hex);

                    if (chbAutoLoad.Checked) btnLoadEmul_Click(null,null);
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Attiva il FileSystemWatcher per il file Intel HEX</summary>
        //-------------------------------------------------------------------------------------------------------------
        private void WatcherFileHex(string FileHex)
        {
            if (FileHex == null)
            {
                if (fswHex != null) fswHex.Dispose();
            }
            else
            {
                fswHex = new FileSystemWatcher();
                fswHex.Path = Path.GetDirectoryName(FileHex);
                fswHex.SynchronizingObject = this; //Sincronizza il watcher con questo thread (oggetto)
                fswHex.NotifyFilter = NotifyFilters.LastWrite;
                fswHex.Filter = Path.GetFileName(FileHex);
                fswHex.Changed += new FileSystemEventHandler(HexChange);
                fireCount = 0;
                fswHex.EnableRaisingEvents = true; //Abilita il controllo LastWrite 
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Evento generato alla modifica del file Intel Hex </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void HexChange(object source, FileSystemEventArgs e)
        {
            fireCount++;
            if (fireCount == 1)
            {
                SystemSounds.Beep.Play();
                if (File.Exists(File_Path_Hex))
                {
                    monitor.ScriviSuMonitor("File {0} Modificato ultimo accesso {1}", File_Path_Hex, File.GetLastAccessTime(File_Path_Hex).ToString("HH:mm:ss:FFF"));

                    if (chbAutoLoad.Checked)
                    {
                        if (!LoadFileHex(File_Path_Hex))
                        {
                            tmr1.Start();
                            return;
                        }
                        btnLoadEmul_Click(null, null);
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(this, "File " + txbHexFile.Text + " modificato vuoi ricaricarlo?", "File Change", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (!LoadFileHex(File_Path_Hex))
                            {
                                tmr1.Start();
                                return;
                            }
                        }
                    }
                }
                else monitor.ScriviSuMonitor("Il file {0} non esiste più", File_Path_Hex);
                tmr1.Start();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        // Scrivi file Hex
        //-------------------------------------------------------------------------------------------------------------
        private void btnSalvaHex_Click(object sender, EventArgs e)
        {
            monitor.ScriviSuMonitor("\n---> Scrittura File *.hex");
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Hex file (*.hex)|*.hex";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] MemHex = new byte[(int)(nudStopBin.Value - nudStartBin.Value + 1)];
                Array.Copy(Z80.PrgMem, (int)nudStartBin.Value, MemHex, 0, MemHex.Length);
                int res = Z80.WriteHex(saveFileDialog1.FileName, MemHex, (uint)nudStartBin.Value);
                if (res == 1) monitor.ScriviSuMonitor("File scritto con successo: " + saveFileDialog1.FileName);
                else monitor.ScriviSuMonitor("Errore scrittura file: " + saveFileDialog1.FileName);
            }
        }

        #endregion File hex

        #region File Binario #########################################################################################

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Carica in memoria il file Bin </summary>
        //-------------------------------------------------------------------------------------------------------------
        private bool LoadFileBin(string NomeFile)
        {
            if (ckbResetOnLoad.Checked) btnResetMem.PerformClick();
            try
            {
                monitor.ScriviSuMonitor("\n---> Lettura File *.bin");
                long length = new System.IO.FileInfo(NomeFile).Length;
                byte[] ArrayBin = new byte[length];
                ArrayBin = File.ReadAllBytes(NomeFile);
                Array.Copy(ArrayBin, 0, Z80.PrgMem, (int)nudBaseBin.Value, ArrayBin.Length);
                txbBinFile.Text = Path.GetFileName(NomeFile);
                monitor.ScriviSuMonitor("Lettura file Bin: {0}", txbBinFile.Text);
                monitor.ScriviSuMonitor("Intervallo Codici Macchina caricati: 0x{0:X4} - 0x{1:X4}", (int)nudBaseBin.Value, (int)nudBaseBin.Value + ArrayBin.Length - 1);
                Z80.SetRangeAllMem((uint)nudResetValue.Value);
                monitor.ScriviSuMonitor("Intervallo Codici Macchina in memoria PC: 0x{0:X5} - 0x{1:X5} diversi da 0x{2:X2}", Z80.InfMem, Z80.SupMem, (uint)nudResetValue.Value);
                nudStartBin.Value = Z80.InfMem;
                nudStopBin.Value = Z80.SupMem;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message);
                return false;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Carica File Binario *.bin</summary>
        //-------------------------------------------------------------------------------------------------------------
        String File_Path_Bin;
        private void txbBinFile_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.Filter = "tab files (*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1; //determina il filtro iniziale
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File_Path_Bin = openFileDialog1.FileName;
                if (LoadFileBin(File_Path_Bin))
                {
                    txbHexFile.Text = "";
                    WatcherFileHex(null);

                    txbAsmFile.Text = "";
                    WatcherFileAsm(null);

                    WatcherFileBin(File_Path_Bin);

                    if (chbAutoLoad.Checked) btnLoadEmul_Click(null, null);
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Attiva il FileSystemWatcher per il file Intel BIN</summary>
        //-------------------------------------------------------------------------------------------------------------
        private void WatcherFileBin(string FileBin)
        {
            if (FileBin == null)
            {
                if (fswBin != null) fswBin.Dispose();
            }
            else
            {
                fswBin = new FileSystemWatcher();
                fswBin.Path = Path.GetDirectoryName(FileBin);
                fswBin.SynchronizingObject = this; //Sincronizza il watcher con questo thread (oggetto)
                fswBin.NotifyFilter = NotifyFilters.LastWrite;
                fswBin.Filter = Path.GetFileName(FileBin);
                fswBin.Changed += new FileSystemEventHandler(BinChange);
                fireCount = 0;
                fswBin.EnableRaisingEvents = true; //Abilita il controllo LastWrite 
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Evento generato alla modifica del file Intel Bin </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void BinChange(object source, FileSystemEventArgs e)
        {
            fireCount++;
            if (fireCount == 1)
            {
                SystemSounds.Beep.Play();
                if (File.Exists(File_Path_Bin))
                {
                    monitor.ScriviSuMonitor("File {0} Modificato ultimo accesso {1}", File_Path_Bin, File.GetLastAccessTime(File_Path_Bin).ToString("HH:mm:ss:FFF"));
                    if (chbAutoLoad.Checked)
                    {
                        if (!LoadFileBin(File_Path_Bin))
                        {
                            tmr1.Start();
                            return;
                        }
                        btnLoadEmul_Click(null, null);
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(this, "File " + txbBinFile.Text + " modificato vuoi ricaricarlo?", "File Change", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            if (!LoadFileBin(File_Path_Bin))
                            {
                                tmr1.Start();
                                return;
                            }
                        }
                    }
                }
                else monitor.ScriviSuMonitor("Il file {0} non esiste più", File_Path_Bin);
                tmr1.Start();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Salva file codici macchina binari </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void btnSalvaBin_Click(object sender, EventArgs e)
        {
            if (nudStopBin.Value != nudStartBin.Value)
            {
                monitor.ScriviSuMonitor("\n---> Scrittura File *.bin");
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                //saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
                saveFileDialog1.Filter = "File binari (*.bin)|*.bin";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    byte[] ArrayBin = new byte[(int)(nudStopBin.Value - nudStartBin.Value + 1)];
                    Array.Copy(Z80.PrgMem, (int)nudStartBin.Value, ArrayBin, 0, ArrayBin.Length);
                    try
                    {
                        File.WriteAllBytes(saveFileDialog1.FileName, ArrayBin);
                        monitor.ScriviSuMonitor("File BIN Salvato su PC: {0}", saveFileDialog1.FileName);
                    }
                    catch (Exception ex)
                    {
                        monitor.ScriviSuMonitor("Errore salvataggio File BIN: {0}", saveFileDialog1.FileName);
                        monitor.ScriviSuMonitor("Error: " + ex.Message);
                        MessageBox.Show(this, "Error: " + ex.Message);
                    }
                }
            }
            else MessageBox.Show(this, "Intervallo creazione bin nullo ");
        }
        private void nudStartBin_ValueChanged(object sender, EventArgs e)
        {
            if (nudStartBin.Value > nudStopBin.Value) nudStopBin.Value = nudStartBin.Value;
        }

        private void nudStopBin_ValueChanged(object sender, EventArgs e)
        {
            if (nudStartBin.Value > nudStopBin.Value) nudStartBin.Value = nudStopBin.Value;
        }


        #endregion File Binario

        #region RAM Emulatore ##################################################################################

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Legge Ram Emulatore </summary>
        //-------------------------------------------------------------------------------------------------------------
        public ErrorNodo ReadRam(uint StartAdd, ref byte[] Ram)
        {
            FrameInterfaccia ris = null;
            uint icorr = 0;

            monitor.ScriviSuMonitor("Letto da: 0x{0:X4}", StartAdd);
            stop = false;
            while (!stop)
            {
                uint nByte = (uint)Ram.Length - icorr;
                if (nByte == 0) break;
                if (nByte > 256) nByte = 256;

                FrameInterfaccia frm = new FrameInterfaccia
                {
                    Comando = ComFrame.COM_READ_RAM,
                    Add = StartAdd,
                    Msg = new byte[] { (byte)(nByte) }   //Num Byte
                };
                ris = InterfEmulEP.InviaFrame(frm, true);
                if (ris == null) return ErrorNodo.NODO_NON_RISPONDE;
                else if (ris.Comando != ComFrame.COM_READ_RAM)
                {
                    monitor.ScriviSuMonitorSR("Errore lettura Ram del blocco: 0x{0:X4} - 0x{0:X4}", StartAdd, StartAdd + nByte - 1);
                    MessageBox.Show("Errore di trasferimento su Ram Emulatore", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ErrorNodo.RISP_COM_ERRATO;
                }
                Array.Copy(ris.Msg, 0, Ram, icorr, nByte);
                icorr += nByte;
                StartAdd += nByte;
                monitor.ScriviSuMonitorSR("Letto fino a: 0x{0:X4}", StartAdd-1);
            }
            if (stop) return ErrorNodo.STOP_UTENTE;
            return ErrorNodo.NO_ERROR;
        }


        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive RAM Emulatore </summary>
        //-------------------------------------------------------------------------------------------------------------
        public ErrorNodo WriteRam(uint StartAdd, byte[] Ram)
        {
            FrameInterfaccia ris = null;
            uint icorr = 0;

            monitor.ScriviSuMonitor("Scritto da: 0x{0:X4}", StartAdd);
            stop = false;
            while (!stop)
            {
                uint nByte = (uint)Ram.Length - icorr;
                if (nByte == 0) break;
                if (nByte > 256) nByte = 256;

                FrameInterfaccia frm = new FrameInterfaccia
                {
                    Comando = ComFrame.COM_WRITE_RAM,
                    Add = StartAdd,
                    Msg = new byte[nByte]
                };
                Array.Copy(Ram, icorr, frm.Msg, 0, nByte);

                ris = InterfEmulEP.InviaFrame(frm, true);

                if (ris == null) return ErrorNodo.NODO_NON_RISPONDE;
                else if (ris.Comando != ComFrame.COM_WRITE_RAM) return ErrorNodo.RISP_COM_ERRATO;
                else if ((WriteReport)ris.Msg[0] != WriteReport.WRITE_OK)
                {
                    monitor.ScriviSuMonitorSR("Errore scrittura Ram nel blocco: 0x{0:X4} - 0x{0:X4}", StartAdd, StartAdd + nByte - 1);
                    MessageBox.Show("Errore di trasferimento su Ram Emulatore", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ErrorNodo.ERRORE_SCRITTURA;
                }
                icorr += nByte;
                StartAdd += nByte;
                monitor.ScriviSuMonitorSR("Scritto fino a: 0x{0:X4}", StartAdd-1);
            }
            monitor.ScriviSuMonitor("Scritti: {0} byte.", Ram.Length);
            if (stop) return ErrorNodo.STOP_UTENTE;
            return ErrorNodo.NO_ERROR;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Reset CPU - Abilita trasferimento dati sull'emulatore </summary>
        //-------------------------------------------------------------------------------------------------------------
        private ErrorNodo ResetCPU(bool rst)
        {
            FrameInterfaccia frm = new FrameInterfaccia
            {
                Comando = ComFrame.COM_SET_COMMAND,
                Add = 0,
                Msg = new byte[2]
            };
            frm.Msg[0] = 0;
            if (rst) frm.Msg[1] = 0;
            else frm.Msg[1] = 1;
            FrameInterfaccia ris = InterfEmulEP.InviaFrame(frm, true);
            if (ris == null || ris.Comando != ComFrame.COM_SET_COMMAND || ris.Msg[0] != 0) return ErrorNodo.NODO_NON_RISPONDE;
            else return ErrorNodo.NO_ERROR;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Setta banco RAM Emulatore </summary>
        //-------------------------------------------------------------------------------------------------------------
        private ErrorNodo BancoRamHigh(bool bnkhigh)
        {
            FrameInterfaccia frm = new FrameInterfaccia
            {
                Comando = ComFrame.COM_SET_COMMAND,
                Add = 0,
                Msg = new byte[2]
            };
            frm.Msg[0] = 1;
            if (bnkhigh) frm.Msg[1] = 1;
            else frm.Msg[1] = 0;
            FrameInterfaccia ris = InterfEmulEP.InviaFrame(frm, true);
            if (ris == null || ris.Comando != ComFrame.COM_SET_COMMAND || ris.Msg[0] != 1) return ErrorNodo.NODO_NON_RISPONDE;
            else return ErrorNodo.NO_ERROR;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Carica codice su emulatore </summary>
        //-------------------------------------------------------------------------------------------------------------

        private ErrorNodo LoadOnEmul()
        {
            string msg;
            byte[] Ram;
            uint initrasf, fintrasf;
            monitor.ScriviSuMonitor("Invio comando di Reset CPU");
            if (ResetCPU(true) != ErrorNodo.NO_ERROR)
            {
                msg = "Non è stato possibile resettare la CPU UpLoad interrotto!";
                monitor.ScriviSuMonitor(msg);
                MessageBox.Show(msg, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ErrorNodo.ERRORE_RESET_CPU;
            }

            //Carica i codici dal PC a partire da nudBaseEP e dimensione cbSizeEP nella Ram all'offsetRam
            uint dimram = (uint)(1024 * (1 << (cbSizeEP.SelectedIndex)));
            initrasf = (uint)nudBaseEP.Value;
            fintrasf = dimram + (uint)nudBaseEP.Value - 1;
            if (nudStartBin.Value > fintrasf || nudStopBin.Value < initrasf)
            {
                monitor.ScriviSuMonitor("Intervallo codici da trasferire: 0x{0:X4} - 0x{1:X4}", (int)nudStartBin.Value, (int)nudStopBin.Value);
                monitor.ScriviSuMonitor("Intervallo codici esterno all'area della Eprom: 0x{0:X4} - 0x{1:X4}", initrasf, fintrasf);
                MessageBox.Show("Intervallo codici esterno all'area della Eprom", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ErrorNodo.ERRORE_SCRITTURA;
            }
            if (ckbLoadAllRam.Checked)
                Ram = new byte[dimram];
            else
            {
                if (nudStartBin.Value > initrasf)
                {
                    initrasf = (uint)nudStartBin.Value;
                }
                if (nudStopBin.Value < fintrasf)
                {
                    fintrasf = (uint)nudStopBin.Value;
                }
                Ram = new byte[fintrasf - initrasf + 1];
            }
            Array.Copy(Z80.PrgMem, initrasf, Ram, 0, Ram.Length);

            uint offset = OffSet[cbSizeEP.SelectedIndex] + initrasf - (uint)nudBaseEP.Value;

            monitor.ScriviSuMonitor("Inizio caricamento RAM Emulatore");
            ErrorNodo ris = WriteRam(offset, Ram);
            if (ris != ErrorNodo.NO_ERROR)
            {
                msg = "Non è stato possibile caricare nella RAM: ";
                monitor.ScriviSuMonitor(msg + ris.ToString());
                MessageBox.Show(msg + ris.ToString(), "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ris;
            }
            monitor.ScriviSuMonitor("Caricati {0} byte dalla memoria PC (0x{1:X4} - 0x{2:X4}) nella ram emulatore all'Offset 0x{3:X4}", Ram.Length, initrasf,fintrasf, offset);

            if (ckbVerifyRam.Checked)
            {
                monitor.ScriviSuMonitor("Inizio lettura RAM Emulatore");
                ris = ReadRam(offset, ref Ram);
                if (ris != ErrorNodo.NO_ERROR)
                {
                    msg = "Non è stato possibile leggere dalla RAM: ";
                    monitor.ScriviSuMonitor(msg + ris.ToString());
                    MessageBox.Show(msg + ris.ToString(), "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ris;
                }
                monitor.ScriviSuMonitor("Letti {0} byte dalla ram emulatore all'Offset 0x{1:X4} fino a 0x{2:X4}", Ram.Length, offset, offset+ Ram.Length-1);

                // Inizio Verifica
                monitor.ScriviSuMonitor("Inizio verifica codice riletto");
                int i;
                for (i = 0; i < Ram.Length; i++)
                {
                    if (Ram[i] != Z80.PrgMem[initrasf + i])
                    {
                        break;
                    }
                }
                if (i != Ram.Length)
                {
                    msg = String.Format("Errore nella verifica all'indirizzo: 0x{0:X4) Byte sul PC: 0x{0:X2) Byte in Ram Emul. 0x{0:X2)",i, Z80.PrgMem[(int)nudBaseEP.Value + i], Ram[i]);
                    msg += "\n Upload interrotto";
                    monitor.ScriviSuMonitor(msg + ris.ToString());
                    MessageBox.Show(msg + ris.ToString(), "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ErrorNodo.ERRORE_VERIFICA;
                }
                else monitor.ScriviSuMonitor("Caricamento verificato mediante Rilettura.");
            }
            monitor.ScriviSuMonitor("Invio comando di Run CPU");
            if (ResetCPU(false) != ErrorNodo.NO_ERROR)
            {
                msg = "Non è stato possibile Far ripartire la CPU";
                monitor.ScriviSuMonitor(msg);
                MessageBox.Show(msg, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ErrorNodo.ERRORE_RUN_CPU;
            }
            monitor.ScriviSuMonitor("Ram emulatore caricata e CPU in RUN");
            return ErrorNodo.NO_ERROR;
        }


        private void rbBancoRamLow_CheckedChanged(object sender, EventArgs e)
        {
            monitor.ScriviSuMonitor("\n---> Commutazione Banco RAM"); 
            ErrorNodo ris = BancoRamHigh(rbBancoRamHigh.Checked);
            if (ris != ErrorNodo.NO_ERROR)
            {
                MessageBox.Show("Errore di commutazione banco Ram Emulatore", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                monitor.ScriviSuMonitor("Errore di commutazione banco Ram Emulatore: {0}", ris.ToString());
            }
            else
            {
                if (rbBancoRamLow.Checked) monitor.ScriviSuMonitor("Banco Ram LOW Attivo");
                else monitor.ScriviSuMonitor("Banco Ram HIGH Attivo");
            }
        }

        #endregion  Ram Emulatore


        //-------------------------------------------------------------------------------------------------------------
        // Resetta la memoria
        //-------------------------------------------------------------------------------------------------------------
        public void btnResetMem_Click(object sender, EventArgs e)
        {
            monitor.ScriviSuMonitor("\n---> Inizializzazione memoria immagine sul PC (64K)");
            Z80.ResetMem((byte)nudResetValue.Value);
            monitor.ScriviSuMonitor("Tutta la RAM (0000-FFFF) resettata al valore 0x{0:X2}", (byte)nudResetValue.Value);
        }

        //-------------------------------------------------------------------------------------------------------------
        // Visualizza memoria caricata
        //-------------------------------------------------------------------------------------------------------------
        private void btnEditMem_Click(object sender, EventArgs e)
        {
            vhex = new VisualizzaHexRAM();
            vhex.Text = "Edit Immagine Memoria RAM";
            vhex.ArrayHex = Z80.PrgMem;
            vhex.Start = 0;
            vhex.Lunghezza = 0x10000;
            vhex.Show();
        }

        private void nudBaseEP_Validated(object sender, EventArgs e)
        {
            nudBaseEP.Value = (int)(nudBaseEP.Value / 1024)*1024;
        }

        private void nudBaseEP_ValueChanged(object sender, EventArgs e)
        {
            int dimRam = 1024 * (1 << (cbSizeEP.SelectedIndex));
            if (nudBaseEP.Value + dimRam > 65536) cbSizeEP.SelectedIndex -= 1;
            nudBaseBin.Value = nudBaseEP.Value;
            lblRangeEprom.Text = String.Format("Range Eprom: 0x{0:X4} - 0x{1:X4}", (int)nudBaseEP.Value, (int)nudBaseEP.Value + dimRam - 1); 
        }
        
        private void cbSizeEP_SelectedIndexChanged(object sender, EventArgs e)
        {
            int dimRam = 1024 * (1 << (cbSizeEP.SelectedIndex));
            if (nudBaseEP.Value + dimRam > 65536) cbSizeEP.SelectedIndex -= 1;
            lblRangeEprom.Text = String.Format("Range Eprom: 0x{0:X4} - 0x{1:X4}", (int)nudBaseEP.Value, (int)nudBaseEP.Value + dimRam - 1);
        }

        private void btnLoadEmul_Click(object sender, EventArgs e)
        {
            
            monitor.ScriviSuMonitor("\n---> Trasferimento dati su emulatore");
            ErrorNodo ris = LoadOnEmul();
            if (ris != ErrorNodo.NO_ERROR)
            {
                MessageBox.Show("Errore di trasferimento dati su Emulatore", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                monitor.ScriviSuMonitor("Errore trasferimento dati su emulatore: {0}", ris.ToString());
            }
            else monitor.ScriviSuMonitor("Trasferimento dati su emulatore avvenuto con successo.");
            
        }


        #region Preference ######################################################################################

        private void LoadPreference()
        {
            try
            {
                using (Stream reader = new FileStream(@".\Preference.xml", FileMode.Open))
                {
                    preference = (Preference)serializer.Deserialize(reader);
                }
                txtbArgomentiZasm.Text = preference.ArgomentiZasm;
                nudBaseBin.Value = preference.BaseBin;
                nudBaseEP.Value = preference.BaseEP;
                nudResetValue.Value = preference.ResetValue;
                cbSizeEP.SelectedIndex = preference.SizeEP;
                txtbArgomentiZasm.Text = preference.ArgomentiZasm;
                ckbVerifyRam.Checked = preference.VerifyRam;
                ckbResetOnLoad.Checked = preference.ResetOnLoad;
                chbAutoAssembler.Checked = preference.AutoAssembler;
                chbAutoLoad.Checked = preference.AutoLoad;
                ckbLoadAllRam.Checked = preference.LoadAllRam; 
                salvaPreferenceOnExitToolStripMenuItem.Checked = preference.salvaPreferenceOnExit;
            }
            catch 
            {
                monitor.ScriviSuMonitor("Non è stato possibile aprire il file delle preferenze.");
            }
        }

        private void SavePreference()
        {
            preference.ArgomentiZasm = txtbArgomentiZasm.Text;
            preference.BaseBin = (int)nudBaseBin.Value;
            preference.BaseEP = (int)nudBaseEP.Value;
            preference.ResetValue = (int)nudResetValue.Value;
            preference.SizeEP = cbSizeEP.SelectedIndex;
            preference.VerifyRam = ckbVerifyRam.Checked;
            preference.ResetOnLoad = ckbResetOnLoad.Checked;
            preference.AutoAssembler = chbAutoAssembler.Checked;
            preference.AutoLoad = chbAutoLoad.Checked;
            preference.LoadAllRam = ckbLoadAllRam.Checked;
            preference.salvaPreferenceOnExit = salvaPreferenceOnExitToolStripMenuItem.Checked;
            try
            {
                using (Stream fs = new FileStream(@".\Preference.xml", FileMode.Create))
                {
                    XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
                    serializer.Serialize(writer, preference); //Serializza l'oggetto del tipo di cui sopra
                }
                monitor.ScriviSuMonitor("Preferenze salvate su Preference.xml");
            }
            catch
            {
                monitor.ScriviSuMonitor("Non è stato possibile salvare il file delle preferenze.");
                MessageBox.Show("Non è stato possibile salvare il file delle preferenze.", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void setPreferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            preference.ArgomentiZasm = txtbArgomentiZasm.Text;
            SavePreference();
        }

        private void DashBoardEmulEP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (salvaPreferenceOnExitToolStripMenuItem.Checked) SavePreference();
        }

        private void loadPreferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadPreference();
        }

        #endregion Preference

        
    }
}
