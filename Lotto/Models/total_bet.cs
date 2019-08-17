using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class total_bet
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string Receive { get; set; }
        public string Reject { get; set; }
        public string Amount { get; set; }
        public string AmountReceive { get; set; }
        public string AmountDiscount { get; set; }
        public string AmountWin { get; set; }
        public string AmountBetWin { get; set; }
    }
}