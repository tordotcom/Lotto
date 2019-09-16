using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class PollData
    {
        //public string Name { get; set; }
        public IList<Poll_Data> poll { get; set; }

    }
    public class Poll_Data
    {
        public string bType { get; set; }
        public string nLen { get; set; }
        public string Number { get; set; }
        public string Amount { get; set; }
    }
}