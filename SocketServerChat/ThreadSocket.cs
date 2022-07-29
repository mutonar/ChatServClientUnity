using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace SocketServerChat
{
    public class ThreadSocket : IObserver
    {
        private Socket _handler = null;
        private DataChat _DataChat = null;
        private UserChat _user = null;

        public ThreadSocket(Socket _handler, DataChat _DataChat)
        {
            _user = new UserChat();
            this._handler = _handler;
            this._DataChat = _DataChat;

        }

        public void Update(object ob)
        {
            sendMessageToUser((String)ob);
        }

        private void sendMessageToUser(String str) {

            byte[] messageToClient = Encoding.UTF8.GetBytes(str);
            _handler.Send(messageToClient);
        }

        private String getDataFromUser()
        {
            string data = null;
            try
            {
                byte[] bytes = new byte[1024];
                int bytesRec = _handler.Receive(bytes);
                data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return data;
        }

        public UserChat getUser()
        {
            return _user;
        }

        public void Listeners()
        {
            try
            {

                while (true)
                {
                    if (_user.name == null)
                    {
                        sendMessageToUser("Enter username:");
                        String usernameTMP = getDataFromUser();
                        if (_DataChat.checkLogin(usernameTMP) == false)
                        {
                            _user.name = usernameTMP;
                        }
                    }
                    if (_user.color == null)
                    {
                        sendMessageToUser("Chose color: ");
                        sendMessageToUser(_DataChat.getFreeColor());
                        try
                        {
                            int countColor = Int16.Parse(getDataFromUser());
                            _user.color = _DataChat.caseColor(countColor);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            continue;
                        }
                       

                        if (_user.color != null)
                        {
                            sendMessageToUser("Welcome to Unity chat, out from server send \"exit\" !\n");
                            
                            IObservable _IObservable = (IObservable)_DataChat; //регистрируемся как наблюдатель
                            _IObservable.RegisterObserver(this);
                            continue;
                        }
                        else
                        {
                            _user.color = null;
                        }
                    }
                    if (_user.color != null && _user.name != null)
                    {
                        String data = getDataFromUser();
                        if (data != null)
                        {
                            _DataChat.addMessage(_user.name + "::" + _user.color + "::" + data + "\n");

                            Console.Write("Data from Client: " + data + "\n");

                            if (data.IndexOf("exit") > -1)
                            {
                                IObservable _IObservable = (IObservable)_DataChat;
                                _IObservable.RemoveObserver(this);

                                Console.WriteLine("Client out of server .");
                                _handler.Shutdown(SocketShutdown.Both);
                                _handler.Close();
                                break;
                            }
                        }
                        else
                        {
                            break; //  не уверен что так надо. может нужно воостанавливать сокет
                        }

                    }

                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _handler.Close();
                IObservable _IObservable = (IObservable)_DataChat;
                _IObservable.RemoveObserver(this);
            }
        }
    }
}
