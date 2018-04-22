using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.Server
{
    public class SyncAccess
    {
        private List<User> userList = new List<User>();
        private readonly object listLock = new object();

        public void Add(User item)
        {
            lock (listLock)
            {
                userList.Add(item);
            }
        }

        public bool Remove(User up)
        {
            lock (listLock)
            {
                up.Dispose();
                return userList.Remove(up);
            }
        }

        public User[] ToArray()
        {
            lock (listLock)
            {
                return userList.ToArray();
            }
        }
    }
}
