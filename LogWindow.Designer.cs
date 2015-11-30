namespace Eve_TradingAgent
{
    partial class LogWindow
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
            this._logConsole = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _logConsole
            // 
            this._logConsole.Cursor = System.Windows.Forms.Cursors.Default;
            this._logConsole.FormattingEnabled = true;
            this._logConsole.ItemHeight = 16;
            this._logConsole.Location = new System.Drawing.Point(155, 122);
            this._logConsole.Name = "_logConsole";
            this._logConsole.ScrollAlwaysVisible = true;
            this._logConsole.Size = new System.Drawing.Size(750, 500);
            this._logConsole.TabIndex = 4;
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 656);
            this.Controls.Add(this._logConsole);
            this.MinimumSize = new System.Drawing.Size(350, 300);
            this.Name = "LogWindow";
            this.Text = "LogWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogWindow_FormClosing);
            this.Shown += new System.EventHandler(this.LogWindow_Shown);
            this.Resize += new System.EventHandler(this.LogWindow_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _logConsole;
    }
}