using System;

namespace Interfaces
{
    public interface ICommon
    {
        string[] GetAvailableUsers();
        void ChangePrivileges(string Login, string password);
    }

    public interface IDog
    {
        bool TryFindObject(out object obj);
        int Bark(int nTimes);
    }

    public interface ICat
    {
        void CutTheText(ref string Text);
    }

    // основной класс для передачи данных
    [Serializable]
    public class Message
    {
        public Message(string Command, object[] Parameters)
        {
            this.Command = Command;
            if (Parameters != null) this.prms = Parameters;
        }

        public bool IsSync;
        public bool IsEmpty = true;
        public readonly string Command;
        public object ReturnValue;
        public object[] prms;
        public Exception Exception;
    }
}
