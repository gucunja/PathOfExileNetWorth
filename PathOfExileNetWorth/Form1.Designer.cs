namespace PathOfExileNetWorth
{
    partial class OverlayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverlayForm));
            this.lblNetWorthOverlay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNetWorthOverlay
            // 
            this.lblNetWorthOverlay.AutoSize = true;
            this.lblNetWorthOverlay.BackColor = System.Drawing.SystemColors.Control;
            this.lblNetWorthOverlay.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblNetWorthOverlay.Location = new System.Drawing.Point(35, -1);
            this.lblNetWorthOverlay.Name = "lblNetWorthOverlay";
            this.lblNetWorthOverlay.Size = new System.Drawing.Size(0, 27);
            this.lblNetWorthOverlay.TabIndex = 0;
            this.lblNetWorthOverlay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OverlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(166, 44);
            this.Controls.Add(this.lblNetWorthOverlay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OverlayForm";
            this.Text = "PoE NetWorth Overlay";
            this.Deactivate += new System.EventHandler(this.OverlayForm_Deactivate);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNetWorthOverlay;
    }
}