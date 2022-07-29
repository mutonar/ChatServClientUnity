using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


class Program
{
    private String addressServer = "127.0.0.1";
    private int port = 80;
    private Socket serverSocket = null;
    BuferData _BuferData = null;

    public String ConnectServer(String addressServer, int port, BuferData _BuferData)
    {
        this.addressServer = addressServer;
        this.port = port;
        this._BuferData = _BuferData;

        IPAddress ipAddr = IPAddress.Parse(addressServer);
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

        serverSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Connect(ipEndPoint);

        senMessage();
        setDataToConsoleSocket();

        String information = "Сокет соединен" + serverSocket.RemoteEndPoint.ToString();
        return information;
    }

    void senMessage()
    {
        Thread _ThreadDataServ = new Thread(() =>
        {
            while (true)
            {
                if (_BuferData._dataToSocket != null)
                {
                    setDataSocket(_BuferData._dataToSocket);
                    _BuferData._dataToSocket = null;
                }
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
                _BuferData._dataFromSocket = dataFromServer;
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