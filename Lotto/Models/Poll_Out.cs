using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class Poll_Out
    {
        public string UID { get; set; }
        public string Send_To_UID { get; set; }
        public string Period_ID { get; set; }
        public string Status { get; set; }
        public string Count { get; set; }
        public string Amount { get; set; }
        public string AmountDiscount { get; set; }
        public string AmountWin { get; set; }
        public string Name { get; set; }
    }
}