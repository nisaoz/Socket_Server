namespace Socket_Server
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.notifyIcon_server = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // notifyIcon_server
            // 
            this.notifyIcon_server.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_server.Icon")));
            this.notifyIcon_server.Text = "Server Socket";
            this.notifyIcon_server.Visible = true;
            this.notifyIcon_server.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_server_MouseClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "Main";
            this.ShowInTaskbar = false;
            this.Text = "Server";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon_server;
    }
}

