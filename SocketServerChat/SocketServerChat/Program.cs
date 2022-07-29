using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace SocketServerChat
{
    class Program
    {
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            //IPHostEntry ipHost = Dns.GetHostEntry("192.168.10.253");
            //IPHostEntry ipHost = Dns.GetHostEntry("172.16.35.23");            
            //IPAddress ipAddr = ipHost.AddressList[0];
            IPAddress ipAddr = IPAddress.Parse("0.0.0.0");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                
                List<Socket> socketClients = new List<Socket>();

                DataChat _DataChat = new DataChat();

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);
                    
                    // Программа приостанавливается, ожидая входящее соединение(он еще один сокет создает)
                    Socket handler = sListener.Accept();
                    socketClients.Add(handler);
                    
                    ThreadSocket threadSocket = new ThreadSocket(handler, _DataChat); // сокет И сам чат передаем клиенту

                    Thread newThread = new Thread(new ThreadStart(threadSocket.Listeners));
                    newThread.Start();

                    Console.WriteLine("Count socket clients" + socketClients.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

       
    }
}