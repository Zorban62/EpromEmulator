
using System.Globalization;
using System;
using System.Windows.Forms;

namespace Andrea_NameSpace
{
    public partial class HexTextWord : UserControl
    {
        private string oldtext;
        private UInt16 _value = 0;
        private byte _valueL = 0;
        private byte _valueH = 0;
        public UInt16 Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _valueL = (byte)(_value & 0xFF);
                _valueH = (byte)((_value >> 8) & 0xFF);
                textBox.Text = String.Format("{0:X4}", _value);
            }
        }

        public byte ValueH
        {
            get { return _valueH; }
            set
            {
                _valueH = value;
                _value = (ushort)(_valueH * 256 + _valueL);
                textBox.Text = String.Format("{0:X4}", _value);
            }
        }

        public byte ValueL
        {
            get { return _valueL; }
            set
            {
                _valueL = value;
                _value = (ushort)(_valueH * 256 + _valueL);
                textBox.Text = String.Format("{0:X4}", _value);
            }
        }

        private bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (_selected) textBox.BackColor = System.Drawing.Color.Red;
                else textBox.BackColor = System.Drawing.SystemColors.Window;
            }
        }

        public bool EnableSelect { get; set; } = false;


        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = ValueChanged;
            handler?.Invoke(this, e);
        }


        public HexTextWord()
        {
            InitializeComponent();
            oldtext = textBox.Text;
            Value = 0;
        }

        private void textBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UInt16 byteValue;

            if (UInt16.TryParse(textBox.Text, NumberStyles.HexNumber, null as IFormatProvider, out byteValue))
            {
                if (Value != byteValue)
                {
                    Value = byteValue;
                    this.OnValueChanged(e);
                }
            }
            else
            {
                MessageBox.Show("Inserire un valore esadecimale tra 0x0000 e 0xFFFF", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox.Text = oldtext;
            }
            textBox.DeselectAll();
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            oldtext = textBox.Text;
        }

        private void textBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                this.OnMouseDown(e);
                if (EnableSelect) Selected ^= true;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 4) 
            {
                textBox.Text = "";
            }
        }

        
    }
}
