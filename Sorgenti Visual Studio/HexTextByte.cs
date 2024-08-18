
using System.Globalization;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Andrea_NameSpace
{
    public partial class HexTextByte : UserControl
    {
        private string oldtext;
        private byte _value = 0;
        public byte Value
        {
            get { return _value; }
            set
            {
                _value = value;
                textBox.Text = String.Format("{0:X2}", _value);
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

        public HexTextByte()
        {
            InitializeComponent();
            oldtext = textBox.Text;
            Value = 0;
        }

        private void textBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            byte byteValue;

            if (Byte.TryParse(textBox.Text, NumberStyles.HexNumber, null as IFormatProvider, out byteValue))
            {
                if (Value != byteValue)
                {
                    Value = byteValue;
                    this.OnValueChanged(e);
                }
            }
            else
            {
                MessageBox.Show("Inserire un valore esadecimale tra 00 e FF", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            if (textBox.Text.Length > 2)
            {
                textBox.Text = "";
            }
        }
    }
}
