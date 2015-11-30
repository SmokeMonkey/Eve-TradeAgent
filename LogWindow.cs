using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eve_TradingAgent
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public void SetConfig(Config config)
        {
            _config = config;
        }

        private Config _config;

        /// <summary>
        /// If the window has never been shown, any value other than -1 of SelectedIndex will throw exception.
        /// </summary>
        public bool HasEverBeenShown = false;

        // Expose 
        public object DataSource
        {
            get
            {
                return _logConsole.DataSource;
            }
            set
            {
                _logConsole.DataSource = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _logConsole.SelectedIndex;
            }
            set
            {
                _logConsole.SelectedIndex = value;
            }
        }

        public int ItemCount
        {
            get
            {
                return _logConsole.Items.Count;
            }
        }

        private void LogWindow_Resize(object sender, EventArgs e)
        {
            _adjustConsoleSize();
        }

        private void _adjustConsoleSize()
        {
            _logConsole.Left = 10;
            _logConsole.Top = 10;
            _logConsole.Height = this.Height - 70;

            // text console cannot have continues height (single line height*n), we adjust window backward to it.
            if (this.Height != _logConsole.Height + 70)
            {
                this.Height = _logConsole.Height + 70;
            }
            _logConsole.Width = this.Width - 35;
        }

        // Dont destroy the object
        private void LogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            if (_config != null)
            {
                _config.ShowLogWindow = false;
            }
            e.Cancel = true; // this cancels the close event.
        }

        private void LogWindow_Shown(object sender, EventArgs e)
        {
            _adjustConsoleSize();
            HasEverBeenShown = true;
        }
    }
}
