using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

public struct Client
{
    private string client_MAC;
    private string client_PCName;
    private string client_IP;
    private string client_port;

    public string Client_MAC
    {
        get { return client_MAC; }
        set { client_MAC = value; }
    }
    public string Client_PCName
    {
        get { return client_PCName; }
        set { client_PCName = value; }
    }
    public string Client_IP
    {
        get { return client_IP; }
        set { client_IP = value; }
    }
    public string Client_Port
    {
        get { return client_port; }
        set { client_port = value; }
    }
};

namespace Socket_Server
{
    class SocketServer_11500
    {
        private static string check, listenport = "11500";
        private static string clientInfo, client_port;
        public static string server_IP;

        public static byte[] buffer = new byte[1024];

        //Client struct'ının elemanları burada tutulacak  
        public static List<Client> client_list = new List<Client>();

        private static Socket accepted = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Socket server_socket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        //Call startListening() method
        public static void Start()
        {
            server_IP = GetIPAddress();

            try
            {
                StartListening();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Environment.Exit(0);
            }
        }

        //Open server_socket and start listening
        //Wait for clients
        private static void StartListening()
        {
            try
            {
                server_socket.Bind(new IPEndPoint(0, Convert.ToInt32(listenport)));
                server_socket.Listen(10);
                Console.WriteLine("Server Dinliyor...");

                while(server_socket.Available == 0)
                {
                    allDone.Reset();
                    server_socket.BeginAccept(new AsyncCallback(AcceptCall), null);
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                server_socket.Close();
                server_socket.Dispose();
            }
        }

        //Connect to client
        //Get local port of client and call receive() method
        private static void AcceptCall(IAsyncResult AR)
        {
            try
            {
                allDone.Set();

                accepted = server_socket.EndAccept(AR);
                string clientInf = accepted.RemoteEndPoint.ToString();
                parseSocketInfo(clientInf);
                Console.WriteLine("Client " + accepted.RemoteEndPoint.ToString() + " Bağlandı");

                if (accepted.Connected)
                {
                    accepted.BeginReceive
                        (buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCall), accepted);
                }
                else
                {
                    server_socket.BeginAccept(new AsyncCallback(AcceptCall), null);
                    accepted.Close();
                    accepted.Dispose();
                }
            }
            catch (Exception ex)
            {
                accepted.Close();
                accepted.Dispose();
                Console.WriteLine(ex.Message.ToString());
                Environment.Exit(0);
            }
        }

        //Read data from the remote device.
        private static void ReceiveCall(IAsyncResult AR)
        {
            Socket listener = (Socket)AR.AsyncState;
            if (listener.Connected)
            {
                try
                {
                    int bytesRead = listener.EndReceive(AR);
                    byte[] data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead); //source, destination, lenght
                    clientInfo = Encoding.ASCII.GetString(data);

                    string mac = parseClientInfo(clientInfo); //Keep connected pc's mac address

                    //Compare mac address that we hold here and mac addresses in client_list
                    //If we have match send it and port address to main()
                    for (int i = 0; i < client_list.Count; i++)
                    {
                        if (mac == client_list[i].Client_MAC)
                        {
                            Client client = new Client();
                            client = client_list[i];
                            //Main.addItemsToStrip(client_list[i].Client_PCName, client_list[i].Client_Port, listener);
                            Main.addItemsToStrip(client, listener);
                        }
                    }
                    Console.WriteLine(clientInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    listener.Close();
                    listener.Dispose();
                    Environment.Exit(0);
                }
            }
            else
            {
                listener.Dispose();
                Main.RemovePC();
                server_socket.BeginAccept(new AsyncCallback(AcceptCall), null);
            }
        }

        public static void Send(Socket listener)
        {
            string request = "ekran";
            byte[] byteData = Encoding.ASCII.GetBytes(request);
            listener.BeginSend
                (byteData, 0, byteData.Length, 0, new AsyncCallback(SendCall), listener);
        }

        private static void SendCall(IAsyncResult AR)
        {
            Socket listener = (Socket)AR.AsyncState;
            int bytesSent = listener.EndSend(AR);
            listener.Close();
            listener.Dispose();

        }

        //Client ip, name, mac adresini ayır
        private static string parseClientInfo(string text)
        {
            char delimeter = ':';

            string[] clientInfo = text.Split(delimeter);

            check = clientInfo[0]; //client gonderdigi text "client_info ile baslıyorsa"
            Client client = new Client();

            if (check.StartsWith("client_info"))
            {
                client.Client_Port = client_port;
                client.Client_IP = clientInfo[1]; //client ip al
                client.Client_PCName = clientInfo[2]; //client pc name al
                client.Client_MAC = clientInfo[3]; //client mac adresini al
            }

            client_list.Add(client);
            return client.Client_MAC;
        }

        //Client portunu al
        private static void parseSocketInfo(string text)
        {
            char delimeter = ':';

            string[] clientInfo = text.Split(delimeter);
            client_port = clientInfo[1]; //client portunu al
        }

        //Server Ip adresini al
        private static string GetIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}