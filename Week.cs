using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitHubAPISandbox
{
    public class Week
    {
        public DateTime weekBinary {get; set;}
        public DateTime weekFileTime { get; set; }
        public DateTime weekFileTimeUTC { get; set; }
        public DateTime weekOADate { get; set; }
        public int additions { get; set; }
        public int deletions { get; set; }
        public int commits { get; set; }
        public long ticks { get; set; }
        public string sTicks { get; set; }
    }
}
