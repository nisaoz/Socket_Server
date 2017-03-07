using System;
using System.Collections.Generic;
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

        public static List<Client> connectedClient_list = new List<Client>();

        public static string PC_Name, client_port;

        public static string[] arg;

        private static Socket socket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static ContextMenuStrip contextMenu = new ContextMenuStrip();

        public Main(string[] args)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;

            InitializeComponent();
            arg = args;

            if(arg != null)
            {

            }   

            this.WindowState = FormWindowState.Minimized;
            notifyIcon_server.Icon = Properties.Resources.Cool;
            notifyIcon_server.Visible = true;

            //ContextMenu'ye eklenecekleri eklemek icin
            MenuStripSettings();

            try
            {
                //ThreadPool;
                //Thread.Sleep(2000);
                Thread th = new Thread(new ThreadStart(SocketServer_11500.Start));
                th.Start();
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Application.Exit();
            }
        }

        private static void socketServer_Start(object o)
        {
            SocketServer_11500.Start();
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
        }

        //Add computer names to context menu
        private static void subMenuItem_Clicked(object sender, EventArgs e)
        {
            var clickedMenuItem = sender as ToolStripMenuItem;
            /////////////////////////////////////
            if (PC_Name == clickedMenuItem.Text)
            {
                SocketServer_11500.Send(socket);
                Start_EkranPaylas(socket);
            }
        }

        //Parametre ile gelene client ip, name, mac adresini ayır
        private static void parseParameter(string text)
        {
            char delimeter = ' ';

            string[] clientInfo = text.Split(delimeter);

            Client client = new Client();

            client.Client_Port = clientInfo[0]; //client port
            client.Client_IP = clientInfo[1]; //client ip al
            client.Client_PCName = clientInfo[2]; //client pc name al
            client.Client_MAC = clientInfo[3]; //client mac adresini al

            connectedClient_list.Add(client);
        }

        //Server'dan girilen komut "ekran" ise ekran paylas calısır
        private static void Start_EkranPaylas(Socket listener)
        {
            Process start_ServerRemote = new Process();
            try
            {
                client_port = (Convert.ToInt32(client_port) + 1).ToString();
                start_ServerRemote.StartInfo.FileName = path;
                start_ServerRemote.StartInfo.Arguments = SocketServer_11500.server_IP + ':' + client_port;
                start_ServerRemote.Start();

                start_ServerRemote.WaitForExit();
                start_ServerRemote.Kill();
                start_ServerRemote.Dispose();
                listener.Dispose();
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

        public static void RemovePC()
        {
            client_port = (Convert.ToInt32(client_port) - 1).ToString();

            for (int i = 0 ; i < connectedClient_list.Count; i++)
            {
                if(connectedClient_list[i].Client_Port == client_port)
                {
                    (contextMenu.Items[0] as ToolStripMenuItem).DropDownItems.RemoveByKey(connectedClient_list[i].Client_PCName);
                    connectedClient_list.RemoveAt(i);
                }
            }
        }

        public static void addItemsToStrip(Client client, Socket s)
        {
            connectedClient_list.Add(client);
            PC_Name = client.Client_PCName;
            client_port = client.Client_Port;
            socket = s;
            
            (contextMenu.Items[0] as ToolStripMenuItem).DropDownItems.Add(client.Client_PCName, null, subMenuItem_Clicked);
        }

        public void notifyIcon_server_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                try
                {
                    contextMenu.Show(Cursor.Position);
                    contextMenu.AutoClose = true;
                }
                catch (Exception ex)
                {
                    contextMenu.Visible = false;
                    Console.WriteLine("Error: " + ex.Message.ToString());
                }
            }
        }
    }
}