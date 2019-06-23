using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class Close
    {
        public string PID { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public string CountReceive { get; set; }
        public string CountUser { get; set; }
        public string CloseBy { get; set; }
        public string BetStatus { get; set; }
        public string CreateDate { get; set; }
        public string CloseDate { get; set; }
    }
}