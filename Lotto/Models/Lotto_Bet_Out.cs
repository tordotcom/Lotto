//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lotto.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lotto_Bet_Out
    {
        public int ID { get; set; }
        public Nullable<int> Poll_Out_ID { get; set; }
        public string Type { get; set; }
        public string NumLen { get; set; }
        public string Number { get; set; }
        public string Amount { get; set; }
        public string AmountDiscount { get; set; }
        public string AmountWin { get; set; }
        public string Result_Status { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<System.DateTime> update_date { get; set; }
    }
}