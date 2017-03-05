using System;
using System.Windows.Forms;

namespace Socket_Server
{
    public partial class Main : Form
    {
        public static string PC_Name;
        public static ContextMenuStrip contextMenu = new ContextMenuStrip();

        public Main()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;

            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            notifyIcon_server.Icon = Properties.Resources.Cool;
            notifyIcon_server.Visible = true;

            //ContextMenu'ye eklenecekleri eklemek icin
            MenuStripSettings();

            try
            {
                SocketServer_11500.Start();
            }
            catch (Exception)
            {
                Application.Exit();
            }
        }

        public static void MenuStripSettings()
        {
            ToolStripMenuItem PCs = new ToolStripMenuItem();
            ToolStripMenuItem Exit = new ToolStripMenuItem();

            PCs.Text = "Bilgisayarlar";
            Exit.Text = "Çıkış";

            contextMenu.Items.Add(PCs);
            contextMenu.Items.Add(Exit);

            contextMenu.ItemClicked += new ToolStripItemClickedEventHandler(
                contextMenu_ItemClicked);
        }

        //ContextMenu itemlarına basıldıgında; EventHandlerlar
        private static void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            if (item.Text == "Çıkış")
                Environment.Exit(0);
        }

        public static void addItemsToStrip(string pcname)
        {
            PC_Name = pcname;
            (contextMenu.Items[0] as ToolStripMenuItem).DropDownItems.Add(PC_Name);
        }

        private void notifyIcon_server_MouseClick(object sender, MouseEventArgs e)
        {
            contextMenu.Show(MousePosition);
        }
    }
}
