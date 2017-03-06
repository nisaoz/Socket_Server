using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Socket_Server
{
    public partial class Main : Form
    {
        private readonly static string fileName = "RemoteConnection_Server.exe";
        private readonly static string path = Path.Combine(Environment.CurrentDirectory, fileName);

        public static string PC_Name, client_port, denemePort = "11501";

        private static Socket socket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                Thread th = new Thread(new ThreadStart(SocketServer_11500.Start));
                th.Start();
                //SocketServer_11500.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Application.Exit();
            }
        }

        private static void MenuStripSettings()
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
        public static void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            if (item.Text == "Çıkış")
                Environment.Exit(0);
            if (item.Text == PC_Name)
            {
                SocketServer_11500.Send(socket);
                Start_EkranPaylas(socket);
            }
        }

        //Server'dan girilen komut "ekran" ise ekran paylas calısır
        private static void Start_EkranPaylas(Socket listener)
        {
            Process start_ServerRemote = new Process();
            try
            {
                start_ServerRemote.StartInfo.FileName = path;
                start_ServerRemote.StartInfo.Arguments = SocketServer_11500.server_IP + ':' + denemePort;
                start_ServerRemote.Start();
                start_ServerRemote.WaitForExit();
                socket.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                listener.Close();
                listener.Dispose();
                start_ServerRemote.Kill();
                start_ServerRemote.Dispose();
            }
        }

        public static void addItemsToStrip(string pcname, string port, Socket s)
        {
            PC_Name = pcname;
            client_port = port;
            socket = s;
            ToolStripMenuItem PC = new ToolStripMenuItem();
            (contextMenu.Items[0] as ToolStripMenuItem).DropDownItems.Add(PC_Name);
        }

        public void notifyIcon_server_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                contextMenu.Show(MousePosition);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message.ToString());
            }
        }
    }
}
