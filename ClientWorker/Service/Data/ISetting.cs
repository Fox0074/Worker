using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    [Serializable]
    public enum DDMiners { none, XMR_N, XMR_A, ETH_N }
    [Serializable]
    public class ISetting
    {
        public string Comp_name { get; set; }
        public bool IsMiner { get; set; }
        public int Open_sum { get; set; }
        public DateTime Start_time { get; set; }
        public string Key { get; set; }
        public string Version { get; set; }
        public string MFTPFloader { get; set; }
        public string MLocalFloader { get; set; }
        public string MFileName { get; set; }
        public string MArgs { get; set; }
        public DDMiners MValut { get; set; }
    }
}
