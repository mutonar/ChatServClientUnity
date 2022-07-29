using System;
namespace SocketServerChat
{
    public interface IObservable
    {
        void RegisterObserver(IObserver o);
        void RemoveObserver(IObserver o);
        void NotifyObservers();
        void NotifyIndividualObserver(IObserver o, int i);
    }
}