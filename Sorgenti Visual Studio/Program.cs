using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Andrea_NameSpace
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DashBoardEmulEP());
        }
    }


    //-----------------------------------------------------------------------------------------------------------------
    // Metodi di estensione per gli Array
    //-----------------------------------------------------------------------------------------------------------------
    public static class Estensione_Array
    {
        /// <summary>  Controlla se i due Array sono identici </summary>
        public static bool ValEqual(this Array a, Array b)
        {
            if (a.Length == b.Length)
            {
                int i;
                for (i = 0; i < a.Length; i++)
                {
                    if (!a.GetValue(i).Equals(b.GetValue(i)))
                    {
                        break;
                    }
                }
                if (i == a.Length) return true;
            }
            return false;
        }

        /// <summary> Confronta il primo Array con il secondo a partire da ind del secondo </summary>
        public static bool ValEqual(this Array a, Array b, long ind)
        {
            int i;
            for (i = 0; i < a.Length; i++)
            {
                if (!a.GetValue(i).Equals(b.GetValue(i + ind)))
                {
                    break;
                }
            }
            if (i == a.Length) return true;
            else return false;
        }

    }

}
