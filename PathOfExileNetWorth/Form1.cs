using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using PathOfExileNetWorth.Properties;

namespace PathOfExileNetWorth
{
    public partial class OverlayForm : Form
    {
        
        private int initialStyle;

        public OverlayForm()
        {
            InitializeComponent();
        }

        public string NetWorthLabelText
        {
            get
            {
                return lblNetWorthOverlay.Text;
            }

            set
            {
                lblNetWorthOverlay.Text = value;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            initialStyle = NativeMethods.GetWindowLong(Handle, -20);
            Location = Settings.Default.overlayWindowLocation;

            TransparencyKey = Color.Wheat;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;

            SetWindowTransparent();

            if (lblNetWorthOverlay.Text == "") { lblNetWorthOverlay.Text = "waiting..."; } 
        }

        private void OverlayForm_Deactivate(object sender, EventArgs e)
        {
            Settings.Default.overlayWindowLocation = Location;
            Settings.Default.Save();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == NativeMethods.WM_NCHITTEST)
                m.Result = (IntPtr)(NativeMethods.HT_CAPTION);
        }
        
        public void SetWindowTransparent()
        {
            NativeMethods.SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);
            BackColor = Color.Wheat;
        }

        public void SetWindowOpaque()
        {
            NativeMethods.SetWindowLong(Handle, -20, initialStyle | 0x80000);
            BackColor = Color.Yellow;
        }
    }
}
