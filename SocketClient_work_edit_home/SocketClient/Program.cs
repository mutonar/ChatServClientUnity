using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketClient
{
    class Program
    {
        private String addressServer = null;
        private int port = 80;
        private Socket serverSocket = null;
        static void Main(string[] args)
        {
            try
            {
                String addS = "127.0.0.1";
                if (args != null && args.Length > 0)
                {
                    addS = args[0];
                }
                Program program = new Program();
                //program.ConnectServer("192.168.137.1", 11000);
                program.ConnectServer(addS, 11000);
                program.senMessageFromConsoleData();
                program.setDataToConsoleSocket();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
        }

        public void ConnectServer(String addressServer, int port)
        {
            this.addressServer = addressServer;
            this.port = port;

            //IPHostEntry ipHost = Dns.GetHostEntry("172.16.35.23");
            //IPAddress ipAddr = ipHost.AddressList[0];

            IPAddress ipAddr = IPAddress.Parse(addressServer);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            serverSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(ipEndPoint);
            Console.WriteLine("Сокет соединяется с {0} ", serverSocket.RemoteEndPoint.ToString());
        }

        void senMessageFromConsoleData()
        {
            Thread _ThreadDataServ = new Thread(() =>
            {
                while (true)
                {
                    Console.Write("Введите сообщение: ");
                    String dataConsole = Console.ReadLine();
                    setDataSocket(dataConsole);
                }
            });
            _ThreadDataServ.Start();
        }

        void setDataToConsoleSocket()
        {
            Thread _ThreadReadSocket = new Thread(() =>
            {
                while (true)
                {
                    String dataFromServer = getDataSocket();
                    Console.WriteLine("Ответ от сервера: {0}", dataFromServer);
                }
            });
            _ThreadReadSocket.Start();
        }
         
            
        

        public void setDataSocket(String message)
        {
            byte[] bytes = new byte[1024];

            bool part1 = serverSocket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (serverSocket.Available == 0);
            
            if (part1 && part2)
            {
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
            }
            else
            {
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = serverSocket.Send(msg);
            }
        }

        public String getDataSocket()
        {
            byte[] bytes = new byte[1024];
            bool part1 = serverSocket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (serverSocket.Available == 0);

            if (part1 && part2)
            {
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
                return null;
            }
            else
            {
                int bytesRec = serverSocket.Receive(bytes);
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
        }
    }
}