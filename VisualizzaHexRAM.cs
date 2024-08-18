using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Be.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using WpfHexaEditor;

namespace Andrea_NameSpace
{
    public partial class VisualizzaHexRAM : Form
    {

        public HexBox hexBox;

        public byte[] ArrayHex { get; set; }
        public long Start { get; set; }
        public long Lunghezza //{ get; set; }
        {
            get => vsbKbyte.Maximum;
            set => vsbKbyte.Maximum = (int)value -1;
        }

    byte[] ArraySector = new byte[256];

        byte[] visArray = new byte[1024];

        bool ChangeEvent = true;

        public VisualizzaHexRAM()
        {
            InitializeComponent();
            hexBox = new HexBox()
            {
                Top = ph1.Top,
                Width = ph1.Width,
                Height = ph1.Height,
                Left = ph1.Left,
                Visible = true,
                UseFixedBytesPerLine = true,
                BytesPerLine = 32,
                ColumnInfoVisible = true,
                LineInfoVisible = true,
                StringViewVisible = true,
                VScrollBarVisible = false,
                InsertActive = false,
                ReadOnly = true
            };

            //float currentSize = hexBox.Font.Size;
            //currentSize -= 1.5F;
            //hexBox.Font = new Font(hexBox.Font.Name, currentSize, hexBox.Font.Style);

            hexBox.KeyDown += keygiu;
            hexBox.KeyUp += keysu;
            hexBox.SelectionStartChanged += HexBox_SelectionStartChanged;
            hexBox.ContextMenuStrip = contextMenuStrip;
            hexBox.SelectionLengthChanged += HexBox_SelectionLengthChanged;
            hexBox.GroupSeparatorVisible = true;
            hexBox.GroupSize = 8;
            hexBox.SelectionLength = 0;

            this.Controls.Add(hexBox);
            this.Controls.Remove(ph1);
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Visualizza 1 K della memoria RAM </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void VisualHex1K(long Start)
        {
            Array.Copy(ArrayHex, Start, visArray, 0, visArray.Length);
            hexBox.ByteProvider = new DynamicByteProvider(visArray);
            hexBox.ByteProvider.LengthChanged += ByteProvider_LengthChanged;
            hexBox.LineInfoOffset = Start;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Visualizza Indirizzo del Byte </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void HexBox_SelectionStartChanged(object sender, EventArgs e)
        {
            if (ChangeEvent)
            {
                ChangeEvent = false;
                nudAddress.Value = hexBox.SelectionStart + hexBox.LineInfoOffset;
                ChangeEvent = true;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Impedisce la selezione multipla </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void HexBox_SelectionLengthChanged(object sender, EventArgs e)
        {
            if (hexBox.SelectionLength > 1) hexBox.SelectionLength = 1;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Impedisce l'aggiunta di Byte </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void ByteProvider_LengthChanged(object sender, EventArgs e)
        {
            if (hexBox.ByteProvider.Length > visArray.Length)
                hexBox.ByteProvider.DeleteBytes((long)(hexBox.ByteProvider.Length-1), 1);
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Disabilita i tasti Back e Delete </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void keygiu(object sender, KeyEventArgs e)
        {
            HexBox h = (HexBox)sender;
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary> Disabilita la possibilità di andare in Insert </summary>
        //-------------------------------------------------------------------------------------------------------------
        private void keysu(object sender, KeyEventArgs e)
        {
            HexBox h = (HexBox)sender;
            if (e.KeyCode == Keys.Insert)
            { 
                h.InsertActive = true;
            }
        }

        private void VisualizzaHex_Shown(object sender, EventArgs e)
        {
            VisualHex1K(vsbKbyte.Value);
            hexBox.Focus();
        }

        private void btnModRam_Click(object sender, EventArgs e)
        {
            for (long i = 0; i < visArray.Length; i++)
            {
                ArrayHex[i + vsbKbyte.Value] = this.hexBox.ByteProvider.ReadByte(i);
            }
            chbReadOnly.Checked = true;
            hexBox.Focus();
        }

        private void vsbKbyte_ValueChanged(object sender, EventArgs e)
        {
            if (!chbReadOnly.Checked)
            {
                DialogResult result = MessageBox.Show(this, "Vuoi cancellare le modifiche effettuate?", "Attenzione!", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    VisualHex1K(vsbKbyte.Value);
                    nudAddress.Value = hexBox.LineInfoOffset;
                    chbReadOnly.Checked = true;
                };
            }
            else
            {
                VisualHex1K(vsbKbyte.Value);
                nudAddress.Value = hexBox.LineInfoOffset;
            }
        }

        private void chbReadOnly_CheckedChanged(object sender, EventArgs e)
        {
            btnModRAM.Enabled = !chbReadOnly.Checked;
            hexBox.ReadOnly = chbReadOnly.Checked;
            if (chbReadOnly.Checked)
            {
                VisualHex1K(vsbKbyte.Value);
            }
            hexBox.Focus();
        }
        
        private void nudAddress_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeEvent)
            {
                ChangeEvent = false;
                hexBox.SelectionStart = (long)nudAddress.Value - hexBox.LineInfoOffset;
                hexBox.Focus();
                ChangeEvent = true;
            }
        }

        
    }
    
}
