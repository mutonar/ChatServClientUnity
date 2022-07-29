using System;
using System.Collections.Generic;

namespace SocketServerChat
{
    public class DataChat : IObservable
    {
        private List<String> _listMessage = new List<string>();
        private List<String> _colorPatern = null;
        private List<IObserver> observers;

        public DataChat()
        {
            observers = new List<IObserver>();

            _colorPatern = generationListColor(10);
        }

        private List<String> generationListColor(int countColor)
        {
            List<String> _listColor = new List<String>();
            Random randColor = new Random();
            for (int i = 0; i < countColor; i++)
            {

                string r = randColor.Next(0, 255).ToString("X2");
                string g = randColor.Next(0, 255).ToString("X2");
                string b = randColor.Next(0, 255).ToString("X2");

                String color = new String( "#" +  r + g + b );
                _listColor.Add(color);
            }
            return _listColor;
        }

        public void RegisterObserver(IObserver o)
        {
            NotifyIndividualObserver(o, 4);

            ThreadSocket _socket = (ThreadSocket)o;
            _listMessage.Add("Login user: " + _socket.getUser().name);
            NotifyObservers();
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            /* убрать из наблюдателя
            удалить из списка юзеров и удалить его цвет
            */

            ThreadSocket _socket = (ThreadSocket)o;
            _colorPatern.Add(generationListColor(1)[0]); // просно новый рандомный
            observers.Remove(o);

            _listMessage.Add("Logout user: " + _socket.getUser().name + "\n");
            NotifyObservers();
        }

        public void NotifyObservers()
        {// рассылаем  сообщения
            foreach (IObserver o in observers)
            {
                o.Update(_listMessage[_listMessage.Count - 1]);
            }
        }

        public void NotifyIndividualObserver(IObserver oIndivid, int countMessage)
        {// рассылаем  сообщения индивидуальные сообщения

                for (int i = countMessage -1; i >= 0; i--)
                {
                    if (i < _listMessage.Count)
                    {
                        oIndivid.Update(_listMessage[_listMessage.Count - (1 + i)]);
                    }

                }
        }



        public void addMessage(String str)
        {
            _listMessage.Add(str);

            NotifyObservers();
        }

        public bool checkLogin(String nameUser)
        {
            foreach (var item in observers)
            {
                ThreadSocket _socket = (ThreadSocket)item;

                if (_socket.getUser().name.Equals(nameUser.Trim()))
                {
                    return true;
                }
            }

            return false;
        }

        public String getFreeColor()
        {
            String c = "Color:";
            String arrayDataColor = "";
            for (int i = 0; i < _colorPatern.Count; i++)
            {

                if (i < _colorPatern.Count - 1)
                {
                    c += i.ToString() + ",";
                    arrayDataColor += _colorPatern[i] + "\n";
                }
                else
                {
                    c += i.ToString() + "\n";
                    arrayDataColor += _colorPatern[i];
                }
            }
            
            return c + arrayDataColor;
        }

        public String caseColor(int i)
        {// пользователь если прошел сюда то и заносим в список
            String colorUser = null;
            if (i <_colorPatern.Count)
            {
                Console.Write(_colorPatern.Count);
                colorUser = _colorPatern[i] ;
                _colorPatern.RemoveAt(i);
                Console.Write(_colorPatern.Count);
                return colorUser;
            }

            return colorUser;
        }
    }
}