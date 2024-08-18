using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;


namespace Andrea_NameSpace
{
	/// <summary>
	/// Classe che gestisce una struttura rappresentante la memoria del uP
	/// </summary>
	public class uCMem
	{
		public enum uCType {           // Device ID
            PIC18F2480   = 0x00D7,
            PIC18F4480   = 0x00D5,
            PIC18F4620   = 0x0060,
            PIC18F4525   = 0x0062,
            PIC18F66K80  = 0x0307,
            PIC18F65K80  = 0x0302,
            PIC18F46K80  = 0x0300,
            PIC18F45K80  = 0x0303,
            PIC18F26K80  = 0x0301,
            PIC18F25K80  = 0x030C,
            PIC18F47Q43  = 0x74A0,
			PIC18F14K22  = 0x0103,
			Z80
        }

		public enum MjrRevision
		{           
			A = 0,
			B = 1
		}

		public enum ReadHexResult {
			OK,
			MancanoDuePunti,
			MancaRecordFinale,
			CheckSumError,
			FileError
		}


        //--------------------------------------------------------------------------------------------------
        // Proprietà di indirizzo inferiore e superiore di diversità. All'interno di questo intervallo
        // estremi compresi c'è diversità fra il contenuto in memoria e quello sul file in lettura
        //--------------------------------------------------------------------------------------------------
        private uint infDiv;
        private uint supDiv;
        private uint iniPrg;
        private uint finPrg;
        private uCType picInMem = 0;

        public uint SupDiv { get => supDiv; }
        public uint InfDiv { get => infDiv; }
        public uint IniPrg { get => iniPrg; }
        public uint FinPrg { get => finPrg; }
        public uCType Pictype { get => picInMem; }

		private uint DimWritePage { get; set; }
		public uint DimErasePage { get; private set; }

		public uint Pos_EEprom { get; private set; }

		public uint InfMem { get; private set; }
        public uint SupMem { get; private set; }

        //--------------------------------------------------------------------------------------------------
        // Costanti di lettura del file *.hex (PIC18F MCUs)
        //--------------------------------------------------------------------------------------------------
        private const uint INI_FLASH = 0x0;       //Inizio codici macchina
		private const uint INI_IDLOC = 0x200000;  //Inizio Id Location (possono essere lette anche in code protection)
		private const uint INI_CNFBIT = 0x300000; //Inizio Bit di configurazione
		private const uint INI_DEVID = 0x3FFFFE;  //Inizio device identification  \Usato solo dal programmatore
		
		
		public byte[] PrgMem;
		public byte[] IdLoc;
		public byte[] CnfBit;
		public byte[] DevId;
		public byte[] EEprom;
		
		// Costruttore definisce le dimensioni delle matrici in memoria in base al uP
		public uCMem(uCType pT, uint IniProg) {
			switch (pT) {
				case uCType.Z80:
					finPrg = 0xFFFF;
					this.PrgMem = new byte[finPrg + 1]; //64K
					Pos_EEprom = 0xF00000;
					DimErasePage = 0;
					DimWritePage = 0;
					break;
				case uCType.PIC18F25K80:
                    finPrg = 0x7FFF;
                    this.PrgMem = new byte[finPrg+1]; //32K
					this.IdLoc = new byte[8];
					this.CnfBit = new byte[16];
					this.DevId = new byte[2];
					this.EEprom = new byte[1024];
					Pos_EEprom = 0xF00000;
					DimErasePage = 64;
					DimWritePage = 64;
					break;
				case uCType.PIC18F2480:
				case uCType.PIC18F4480:
                    finPrg = 0x3FFF;
                    this.PrgMem = new byte[finPrg+1]; //16K
					this.IdLoc = new byte[8];
					this.CnfBit = new byte[16];
					this.DevId = new byte[2];
					this.EEprom = new byte[256];
					Pos_EEprom = 0xF00000;
					DimErasePage = 64;
					DimWritePage = 8;
					break;
                case uCType.PIC18F46K80:
                case uCType.PIC18F4620:					
                    finPrg = 0xFFFF;
					this.PrgMem = new byte[finPrg+1]; //64K
					this.IdLoc = new byte[8];
					this.CnfBit = new byte[16];
					this.DevId = new byte[2];
					this.EEprom = new byte[1024];
					Pos_EEprom = 0xF00000;
					DimErasePage = 64;
					DimWritePage = 64;
					break;
                case uCType.PIC18F4525:
                    finPrg = 0xBFFF;
                    this.PrgMem = new byte[finPrg + 1]; //64K
                    this.IdLoc = new byte[8];
                    this.CnfBit = new byte[16];
                    this.DevId = new byte[2];
                    this.EEprom = new byte[1024];
					Pos_EEprom = 0xF00000;
					DimErasePage = 64;
					DimWritePage = 64;
					break;
                case uCType.PIC18F47Q43:
                    finPrg = 0x1FFFF;
                    this.PrgMem = new byte[finPrg + 1]; //128K
                    this.IdLoc = new byte[64]; //0x200000 User Id
                    this.CnfBit = new byte[16]; //
                    this.DevId = new byte[2];
                    this.EEprom = new byte[1024];
					Pos_EEprom = 0x380000;
					DimErasePage = 256;
					DimWritePage = 256;
					break;
				case uCType.PIC18F14K22:
					finPrg = 0x3FFF;
					this.PrgMem = new byte[finPrg + 1]; //16K
					this.IdLoc = new byte[64]; //0x200000 User Id
					this.CnfBit = new byte[16]; //0x300000 Conf bit
					this.DevId = new byte[2];
					this.EEprom = new byte[256];
					Pos_EEprom = 0xF00000;
					DimErasePage = 64;
					DimWritePage = 16;
					break;
				default:
                    MessageBox.Show("Errore: uController non in elenco: " + pT);
                    break;
			}
            picInMem = pT;
            if (IniProg < PrgMem.Length) {
                iniPrg = IniProg;
            } else {
                MessageBox.Show("Inizio Programmazioni fuori dalla memoria: " + String.Format("{0:X6}", IniProg));
            }
			ResetMem(0xFF);
		}

		/// <summary>
		/// Reset Memoria a 0xFF
		/// </summary>
		public void ResetMem(byte ValInit)
        {
			for (int i = 0; i < PrgMem.Length; i++) PrgMem[i] = ValInit; //Resetta a =xFF
			infDiv = finPrg;
			supDiv = 0;
			if (EEprom != null) for (int i = 0; i < EEprom.Length; i++) EEprom[i] = 0xFF; //Resetta locazioni EEProm se è Definita
		}


		/// <summary>
		/// Metodo per la scrittura del file HEX per il PIC
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		// Scrive il file *.hex PIC
		//-------------------------------------------------------------------------------------------------------------
		public int WritePIC(string FileHEX){
	        try {
	    		using (StreamWriter sw = new StreamWriter(FileHEX)) {
					WriteMem(sw,PrgMem,0);
					WriteMem(sw,IdLoc,INI_IDLOC);
					WriteMem(sw,CnfBit,INI_CNFBIT);
					WriteMem(sw,EEprom,Pos_EEprom);
					sw.WriteLine(":00000001FF");
				}
				return 1;
	        } catch (Exception ex) {
	            MessageBox.Show("Errore: non può aprire il file in scrittura. Original error: " + ex.Message);
	            return 0;
	        }	
		}

		/// <summary>
		/// Metodo per la scrittura del file HEX per lo Z80
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		// Scrive il file *.hex Z80
		//-------------------------------------------------------------------------------------------------------------
		public int WriteZ80(string FileHEX)
		{
			try
			{
				using (StreamWriter sw = new StreamWriter(FileHEX))
				{
					WriteMem(sw, PrgMem, 0);
					sw.WriteLine(":00000001FF");
				}
				return 1;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Errore: non può aprire il file in scrittura. Original error: " + ex.Message);
				return 0;
			}
		}

        //-------------------------------------------------------------------------------------------------------------
        // Scrive il file *.hex 
        //-------------------------------------------------------------------------------------------------------------
        public int WriteHex(string FileHEX, byte[] MemArray, uint offset)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileHEX))
                {
                    WriteMem(sw, MemArray, offset);
                    sw.WriteLine(":00000001FF");
                }
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore: non può aprire il file in scrittura. Original error: " + ex.Message);
                return 0;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        // Scrive una matrice sul file *.hex
        //-------------------------------------------------------------------------------------------------------------
        private void WriteMem(StreamWriter sw,byte[] Mat,uint OffsetFile) {
			//----------------------------------------------------------------------------
			// Inizializzazioni del ciclo di scrittura file
			//----------------------------------------------------------------------------
			uint Indice = OffsetFile & 0xFFFF;
			uint IndirizzoEsteso = (OffsetFile >> 16) & 0xFFFF;
			//----------------------------------------------------------------------
			// Calcola record 04 di Indirizzo Esteso
			//----------------------------------------------------------------------
			string	LineaEx = ":02000004" + String.Format("{0:X4}",IndirizzoEsteso);
			LineaEx += CheckSum(LineaEx);
			sw.WriteLine(LineaEx);
			//----------------------------------------------------------------------------
			// Ciclo su tutti i codici macchina
			//----------------------------------------------------------------------------
			uint iMat = 0;
			do {
				if (Indice > 0xFFFF) {  							//Scrivi indirizzo esteso banco 64K
					Indice = 0;
					IndirizzoEsteso += 1;
					LineaEx = ":02000004" + String.Format("{0:X4}",IndirizzoEsteso);
					LineaEx += CheckSum(LineaEx);
					sw.WriteLine(LineaEx);
				}
			   	//-------------------------------------------------------------------------
			   	// Elimina i codici 0xFF
			   	//-------------------------------------------------------------------------
			   	if (Mat[iMat] == 0xFF ) {
			   		Indice++;
			   		iMat++;
			   	} else {
					//----------------------------------------------------------------------
					// Costruisce il record HEX
					// Interrompe la riga se ci sono almeno due FF consecutivi
					// Ordina le righe in modo che l'indirizzo finisca per 0xiii0
					//----------------------------------------------------------------------
					string Linea = String.Format("{0:X4}",Indice) + "00"; //indirizzo Linea File
					int NumByteRiga = 0;
					do {
						byte CM = Mat[iMat];
						if (iMat < Mat.Length - 1) {
							byte CMp1 = Mat[iMat+1];
							if (CM == 0xFF && CMp1 ==0xFF) break; //interrompi la linea se ci sono due 0xFF consecutivi
						}
						Linea += String.Format("{0:X2}",CM);
						NumByteRiga++;
						iMat++;
						Indice++;
						if (iMat == Mat.Length) break;       //interrompi la linea se è finita la matrice
						if (Indice > 0xFFFF) break; 		//interrompi la linea se e' finito banco da 64K
						if ((Indice & 0x000F) == 0) break;  //Le linee del file hanno sempre indirizzo allineato alla pagina da 16 bit
					} while ( NumByteRiga < 16);            //Massimo numero 16 byte per linea di file
					Linea = ":" + String.Format("{0:X2}",NumByteRiga) + Linea;  //
					Linea += CheckSum(Linea);          							// Scrivi la linea sul file
				   		sw.WriteLine(Linea);										//
			   	}
			} while (iMat < Mat.Length);
		}
		
		
		//----------------------------------------------------------------------------------------------------
		// Restituisce il CheckSum della linea
		//----------------------------------------------------------------------------------------------------
		private string CheckSum(string Linea) {
			int Csum = 0;
			for (int i=1;i<Linea.Length;i+=2) {
				Csum += Convert.ToInt32(Linea.Substring(i,2), 16);
			}
			return String.Format("{0:X2}",(-Csum & 0xFF));;
		}

		
		/// <summary>
		/// Metodo per la lettura del file HEX e popolazione matrici in memoria
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		// Legge il file *.hex
		//-------------------------------------------------------------------------------------------------------------
		public ReadHexResult Read_uPMem(string FileHEX) {
            uint AddFlash = 0;
            ReadHexResult risultato = ReadHexResult.OK;
	        try {
	    		using (StreamReader sr = new StreamReader(FileHEX)) {
					uint IndirizzoEsteso = 0;
					infDiv = finPrg;
					supDiv = 0;
					while (true){
	    				// ------------------------- Legge una linea del file testo *.hex alla volta ------------------
	    				String line = sr.ReadLine();
	    				//Debug.WriteLine(line);
	    				// ------------------------- Controllo se la linea è l'ultima del file ------------------------
	    				if (line == ":00000001FF") {
	    					risultato = ReadHexResult.OK;
	    					break;
	    				}
	    				// ------------------------- Errore se raggiunta la fine del file -----------------------------
	    				if (sr.EndOfStream) { 
	    					risultato = ReadHexResult.MancaRecordFinale;
	    					break;
	    				}
	    				// ------------------------- Errore di formato "Intel INHX8M" ---------------------------------
	    				if (line.Substring(0,1) != ":") {
	    					risultato = ReadHexResult.MancanoDuePunti;
	    					break;
	    				}
	    				int NumByteRiga = Convert.ToInt32(line.Substring(1,2), 16);
	    				// ------------------------- Controllo CheckSum -----------------------------------------------
	    				long CheckSum = 0;
	    				for (int i=0;i<NumByteRiga+5;i++) {
	    					CheckSum = CheckSum + Convert.ToInt32(line.Substring(1+i*2,2), 16);
	    				}
	    				// ------------------------- Errore di checksum   ---------------------------------
	    				if ((CheckSum & 0xFF) != 0){
	    					risultato = ReadHexResult.CheckSumError;
	    					break;
	    				}
	    				// ------------------------- Analisi dei codici linea -----------------------------------------
	    				// : 04 0018 00 08 EF 04    F0 F9  <- Esempio  
	    				// : nb IIii cl b1 b2 b3 .. bn CS
	    				// |  |   |   |  |  |  |     |  +--> Check Sum = -(nb + II + ii + cl + b1 + b2 + ... + bn)  
						// |  |   |   |  +--+--+-----+-----> Byte Linea <------+
						// |  |   |   +--------------------> Codice Linea      |
						// |  |   +------------------------> Indirizzo Linea   |
						// |  +----------------------------> Numero Byte Linea ^
						// +-------------------------------> Due punti iniziali = record HEX
	    				uint IndirizzoLinea = Convert.ToUInt32(line.Substring(3,4), 16);
	    				int CodiceLinea = Convert.ToInt32(line.Substring(7,2), 16);
	    				switch (CodiceLinea) {
	    				// .................................. Data record .............................................
	    				case 0:
	    					uint Indirizzo = IndirizzoLinea;
	    					int i = 9;
	    					// per tutti i byte della linea
	    					for (int k=0;k<NumByteRiga;k++) {
	    						byte ReadByte = Convert.ToByte(line.Substring(i,2), 16);
		    					if (IndirizzoEsteso + Indirizzo >= Pos_EEprom) {
		    						uint Add = Indirizzo + IndirizzoEsteso - Pos_EEprom;
		    						this.EEprom[Add]=ReadByte;
		    					} else if (IndirizzoEsteso + Indirizzo >= INI_DEVID  ) {
	    							// Non presenti nel file Hex, lette solo dal programmatore
	    							uint Add = Indirizzo + IndirizzoEsteso - INI_DEVID;
		    						this.DevId[Add]=ReadByte;
		    					} else if (IndirizzoEsteso + Indirizzo >= INI_CNFBIT ) {
		    						uint Add = Indirizzo + IndirizzoEsteso - INI_CNFBIT;
		    						this.CnfBit[Add]=ReadByte;
		    					} else if (IndirizzoEsteso + Indirizzo >= INI_IDLOC ) {
		    						uint Add = Indirizzo + IndirizzoEsteso - INI_IDLOC;
		    						this.IdLoc[Add]=ReadByte;
		    					} else if (IndirizzoEsteso + Indirizzo >= INI_FLASH) {
		    						AddFlash = Indirizzo + IndirizzoEsteso - INI_FLASH;
                                    //if (this.PrgMem[AddFlash] != ReadByte) {
                                        this.PrgMem[AddFlash] = ReadByte;
                                        if (AddFlash < infDiv) infDiv = AddFlash;
										if (AddFlash > supDiv) supDiv = AddFlash;
									//}
		    					}
	    						i += 2;
	    						Indirizzo++;
	    					}
	    					break;
	    				// .................................... End of file record ....................................
	    				case 1: 
	    					break;
	    				// .................................... Extended segment address record .......................
	    				case 2: 
	    					break;
	    				// .................................... Start segment address record ..........................
	    				case 3: 
	    					break;
	    				// .................................... Extended linear address record ........................
	    				// :02 0000 04 0000 FA  -> 0000 = Banco da  0000 a  FFFF
						// :02 0000 04 0001 F9  -> 0001 = Banco da 10000 a 1FFFF   ecc...
	    				case 4: 
							IndirizzoEsteso = 0x10000 * Convert.ToUInt32(line.Substring(9,4), 16);
	    					break;
	    				// .................................... Start linear address record ...........................
	    				case 5: 
	    					break;
	    				}
					}
				}    
	        } catch (Exception ex) {
	            MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
	            risultato = ReadHexResult.FileError;
	        }
            return risultato;
		}

		public void SetRangeAllMem(uint codevuoto = 0xFF)
		{
            InfMem = finPrg;
            SupMem = 0;
            for (uint AddFlash=0; AddFlash < PrgMem.Length; AddFlash++)
            {
                if (PrgMem[AddFlash] != codevuoto)
                {
                    if (AddFlash < InfMem) InfMem = AddFlash;
                    if (AddFlash > SupMem) SupMem = AddFlash;
                }
            }
		}


	}
}
