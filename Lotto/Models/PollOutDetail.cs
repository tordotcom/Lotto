using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class PollOutDetail
    {
        public string ID { get; set; }
        public string LottoID { get; set; }
        public string UID { get; set; }
        public string Send_To_UID { get; set; }
        public string Status { get; set; }
        public string SendDate { get; set; }
        public string Type { get; set; }
        public string NumLen { get; set; }
        public string Number { get; set; }
        public string Amount { get; set; }
        public string AmountWin { get; set; }
        public string ResultStatus { get; set; }
        public string TotalAmount { get; set; }
        public string Name { get; set; }
        public string SendToUsername { get; set; }
        public string Username { get; set; }
        public string AmountDiscount { get; set; }
    }
}