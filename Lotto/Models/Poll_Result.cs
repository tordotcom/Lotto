using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class Poll_Result
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string NumLen { get; set; }
        public string Number { get; set; }
        public string Amount { get; set; }
        public string AmountDiscount { get; set; }
        public string three_up { get; set; }
        public string three_ood { get; set; }
        public string three_down { get; set; }
        public string two_up { get; set; }
        public string two_ood { get; set; }
        public string two_down { get; set; }
        public string up { get; set; }
        public string down { get; set; }
        public string first_three { get; set; }
        public string first_three_ood { get; set; }
    }
}