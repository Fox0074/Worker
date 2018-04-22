using Interfaces;
using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class Cat_Ring0 : Ring2, ICat
    {
        public Cat_Ring0(User u) : base(u)
        {
            up.UserType = UserType.System;
        }

        public void CutTheText(ref string Text)
        {
            Text = Text.Remove(Text.Length - 1);
        }
    }
}
