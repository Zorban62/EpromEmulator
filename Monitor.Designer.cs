namespace Andrea_NameSpace
{
    partial class Monitor
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.lboxMonitor = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lboxMonitor
            // 
            this.lboxMonitor.BackColor = System.Drawing.Color.Black;
            this.lboxMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lboxMonitor.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lboxMonitor.ForeColor = System.Drawing.Color.White;
            this.lboxMonitor.FormattingEnabled = true;
            this.lboxMonitor.Items.AddRange(new object[] {
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""});
            this.lboxMonitor.Location = new System.Drawing.Point(0, 0);
            this.lboxMonitor.Name = "lboxMonitor";
            this.lboxMonitor.Size = new System.Drawing.Size(862, 241);
            this.lboxMonitor.TabIndex = 21;
            this.lboxMonitor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lboxMonitor_MouseDoubleClick);
            // 
            // Monitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lboxMonitor);
            this.Name = "Monitor";
            this.Size = new System.Drawing.Size(862, 241);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lboxMonitor;
    }
}
