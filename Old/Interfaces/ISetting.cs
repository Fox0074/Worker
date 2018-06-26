using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    public class ISetting
    {
        public string Comp_name { get; set; }
        public bool IsMiner { get; set; }
        public int Open_sum { get; set; }
        public DateTime Start_time { get; set; }
        public string Key { get; set; }
        public string Version { get; set; }
    }
}
