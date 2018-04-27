using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Users
{
    public interface ISystem
    {
        void CutTheText(ref string Text);
        string[] GetListUsers();
    }
}
