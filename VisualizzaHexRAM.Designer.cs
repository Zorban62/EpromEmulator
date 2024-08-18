namespace Andrea_NameSpace
{
    partial class VisualizzaHexRAM
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ph1 = new System.Windows.Forms.PictureBox();
            this.btnModRAM = new System.Windows.Forms.Button();
            this.chbReadOnly = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.vsbKbyte = new System.Windows.Forms.VScrollBar();
            this.nudAddress = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.ph1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // ph1
            // 
            this.ph1.Location = new System.Drawing.Point(12, 39);
            this.ph1.Name = "ph1";
            this.ph1.Size = new System.Drawing.Size(1145, 550);
            this.ph1.TabIndex = 0;
            this.ph1.TabStop = false;
            this.ph1.Visible = false;
            // 
            // btnModRAM
            // 
            this.btnModRAM.Enabled = false;
            this.btnModRAM.Location = new System.Drawing.Point(15, 5);
            this.btnModRAM.Name = "btnModRAM";
            this.btnModRAM.Size = new System.Drawing.Size(97, 28);
            this.btnModRAM.TabIndex = 5;
            this.btnModRAM.Text = "Modifica RAM";
            this.btnModRAM.UseVisualStyleBackColor = true;
            this.btnModRAM.Click += new System.EventHandler(this.btnModRam_Click);
            // 
            // chbReadOnly
            // 
            this.chbReadOnly.AutoSize = true;
            this.chbReadOnly.Checked = true;
            this.chbReadOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbReadOnly.Location = new System.Drawing.Point(175, 12);
            this.chbReadOnly.Name = "chbReadOnly";
            this.chbReadOnly.Size = new System.Drawing.Size(76, 17);
            this.chbReadOnly.TabIndex = 8;
            this.chbReadOnly.Text = "Read Only";
            this.chbReadOnly.UseVisualStyleBackColor = true;
            this.chbReadOnly.CheckedChanged += new System.EventHandler(this.chbReadOnly_CheckedChanged);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip2";
            this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // vsbKbyte
            // 
            this.vsbKbyte.LargeChange = 1024;
            this.vsbKbyte.Location = new System.Drawing.Point(127, 5);
            this.vsbKbyte.Maximum = 64512;
            this.vsbKbyte.Name = "vsbKbyte";
            this.vsbKbyte.Size = new System.Drawing.Size(29, 28);
            this.vsbKbyte.SmallChange = 1024;
            this.vsbKbyte.TabIndex = 9;
            this.vsbKbyte.ValueChanged += new System.EventHandler(this.vsbKbyte_ValueChanged);
            // 
            // nudAddress
            // 
            this.nudAddress.Hexadecimal = true;
            this.nudAddress.Location = new System.Drawing.Point(294, 11);
            this.nudAddress.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudAddress.Name = "nudAddress";
            this.nudAddress.Size = new System.Drawing.Size(69, 20);
            this.nudAddress.TabIndex = 12;
            this.nudAddress.ValueChanged += new System.EventHandler(this.nudAddress_ValueChanged);
            // 
            // VisualizzaHexRAM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 601);
            this.Controls.Add(this.nudAddress);
            this.Controls.Add(this.vsbKbyte);
            this.Controls.Add(this.chbReadOnly);
            this.Controls.Add(this.btnModRAM);
            this.Controls.Add(this.ph1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximumSize = new System.Drawing.Size(2000, 2000);
            this.Name = "VisualizzaHexRAM";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Visualizza Hex RAM";
            this.Shown += new System.EventHandler(this.VisualizzaHex_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ph1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ph1;
        private System.Windows.Forms.Button btnModRAM;
        private System.Windows.Forms.CheckBox chbReadOnly;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.VScrollBar vsbKbyte;
        private System.Windows.Forms.NumericUpDown nudAddress;
    }
}