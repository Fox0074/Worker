using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Users
{
    public interface IAdmin
    {
        bool TryFindObject(out object obj);
        int Bark(int nTimes);
    }
}
