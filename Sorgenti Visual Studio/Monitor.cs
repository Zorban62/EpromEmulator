using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Andrea_NameSpace
{
    public partial class Monitor : UserControl
    {

        bool ssmFirstRow = false;

        public Monitor()
        {
            InitializeComponent();

            //LogFile = new StreamWriter("Monitor.log");
            //LogFile.AutoFlush = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Cancella il monitor </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void ClearMonitor()
        {
            lboxMonitor.Items.Clear();
            ssmFirstRow = false;
        }

        

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive sulla ListBox del Monitor - Rimanendo sulla stessa riga - </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void ScriviSuMonitorSR(string inMsg)
        {
            //string Tempo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //string msg = Tempo + " - "; //Inizia la stringa con il Time Stamp
            string msg = "";

            if (ssmFirstRow && (lboxMonitor.Items.Count != 0)) lboxMonitor.Items.RemoveAt(lboxMonitor.Items.Count - 1);
            lboxMonitor.Items.Add(msg + inMsg); //Addiziona la stringa al controllo ListBox
                                                //lboxMonitor.Items.Insert(lboxMonitor.Items.Count,inMsg);
            if (lboxMonitor.Items.Count == 1000) lboxMonitor.Items.RemoveAt(0); //Limita listbox a 1000 items
            lboxMonitor.SelectedIndex = lboxMonitor.Items.Count - 1; //Seleziona e quindi visualizza sempre l'ultimo con evidenziazione
                                                                     //lboxMonitor.TopIndex = lboxMonitor.Items.Count-1; //Visualizza sempre l'ultimo usando la proprietà TopIndex senza evidenziazione
                                                                     //lboxMonitor.Refresh();
            ssmFirstRow = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive sulla ListBox del Monitor stringa formattata- Rimanendo sulla stessa riga - </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void ScriviSuMonitorSR(String format, params object[] arg0)
        {
            //string Tempo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //string msg = Tempo + " - "; //Inizia la stringa con il Time Stamp
            //string msg = "";

            if (ssmFirstRow && (lboxMonitor.Items.Count != 0)) lboxMonitor.Items.RemoveAt(lboxMonitor.Items.Count - 1);
            lboxMonitor.Items.Add(String.Format(format, arg0)); //Addiziona la stringa al controllo ListBox
                                                //lboxMonitor.Items.Insert(lboxMonitor.Items.Count,inMsg);
            if (lboxMonitor.Items.Count == 1000) lboxMonitor.Items.RemoveAt(0); //Limita listbox a 1000 items
            lboxMonitor.SelectedIndex = lboxMonitor.Items.Count - 1; //Seleziona e quindi visualizza sempre l'ultimo con evidenziazione
                                                                     //lboxMonitor.TopIndex = lboxMonitor.Items.Count-1; //Visualizza sempre l'ultimo usando la proprietà TopIndex senza evidenziazione
                                                                     //lboxMonitor.Refresh();
            ssmFirstRow = true;
        }



        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive sulla ListBox del Monitor - Passando alla riga successiva -  </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void ScriviSuMonitor(string inMsg)
        {
            while (true)
            {
                int finelinea = inMsg.IndexOf("\r\n");
                if (finelinea == -1)
                {
                    finelinea = inMsg.IndexOf("\n");
                    if (finelinea == -1)
                    {
                        if (inMsg != "") ScrSuMon(inMsg);
                        break;
                    }
                    else
                    {
                        string linea = inMsg.Remove(finelinea);
                        ScrSuMon(linea);
                        inMsg = inMsg.Substring(finelinea + 1);
                    }
                }
                else
                {
                    string linea = inMsg.Remove(finelinea);
                    ScrSuMon(linea);
                    inMsg = inMsg.Substring(finelinea + 2);
                }
            }

            void ScrSuMon(string msg) {
                ssmFirstRow = false;
                lboxMonitor.Items.Add(msg); //Addiziona la stringa al controllo ListBox
                //LogFile.WriteLine(msg);
                //lboxMonitor.Items.Insert(lboxMonitor.Items.Count,inMsg);
                if (lboxMonitor.Items.Count == 1000) lboxMonitor.Items.RemoveAt(0); //Limita listbox a 1000 items
                lboxMonitor.SelectedIndex = lboxMonitor.Items.Count - 1; //Seleziona e quindi visualizza sempre l'ultimo con evidenziazione
                                                                         //lboxMonitor.TopIndex = lboxMonitor.Items.Count-1; //Visualizza sempre l'ultimo usando la proprietà TopIndex senza evidenziazione
                                                                         //lboxMonitor.Refresh();
            }

        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive sulla ListBox del Monitor stringa formattata - Passando alla riga successiva - </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void ScriviSuMonitor(String format, params object[] arg0)
        {
            ssmFirstRow = false;
            //string Tempo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //string msg = Tempo + " - "; //Inizia la stringa con il Time Stamp
            lboxMonitor.Items.Add(String.Format(format, arg0)); //Addiziona la stringa al controllo ListBox
                                                                //lboxMonitor.Items.Insert(lboxMonitor.Items.Count,inMsg);
            if (lboxMonitor.Items.Count == 1000) lboxMonitor.Items.RemoveAt(0); //Limita listbox a 1000 items
            lboxMonitor.SelectedIndex = lboxMonitor.Items.Count - 1; //Seleziona e quindi visualizza sempre l'ultimo con evidenziazione
                                                                     //lboxMonitor.TopIndex = lboxMonitor.Items.Count-1; //Visualizza sempre l'ultimo usando la proprietà TopIndex senza evidenziazione
                                                                     //lboxMonitor.Refresh();
            //LogFile.WriteLine(String.Format(format, arg0));

        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Scrive sulla ListBox del Monitor una matrice  </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void ScriviSuMonitor(string msg, byte[] matbyte)
        {
            ssmFirstRow = false;
            //string msg = "Mat. Byte: "; //crea la stringa del messaggio in esadecimale
            //for (int i = 0; i < matbyte.Length; i++)

            int i=0;
            while(i < matbyte.Length)
            {
                msg += String.Format("{0:X2} ", matbyte[i]);
                i++;
                if ((i % 32) == 0)
                {
                    lboxMonitor.Items.Add(msg);
                    //LogFile.WriteLine(msg);
                    msg = "";
                }
            }
            lboxMonitor.Items.Add(msg); //Addiziona la stringa al controllo ListBox
            //LogFile.WriteLine(msg);
            if (lboxMonitor.Items.Count == 1000) lboxMonitor.Items.RemoveAt(0); //Limita listbox a 1000 items
            lboxMonitor.SelectedIndex = lboxMonitor.Items.Count - 1; //Seleziona e quindi visualizza sempre l'ultimo con evidenziazione
                                                                     //lboxMonitor.TopIndex = lboxMonitor.Items.Count-1; //Visualizza sempre l'ultimo usando la proprietà TopIndex senza evidenziazione
                                                                     //lboxMonitor.Refresh();
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Cancella la ListBox del Monitor </summary>
        //-------------------------------------------------------------------------------------------------------------
        public void lboxMonitor_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ClearMonitor();
        }
                

    }
}
