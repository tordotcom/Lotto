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
    
    public partial class Poll_Bet_Out
    {
        public int ID { get; set; }
        public Nullable<int> UID { get; set; }
        public Nullable<int> Send_To_UID { get; set; }
        public Nullable<int> Poll_ID { get; set; }
        public string Poll_Name { get; set; }
        public Nullable<int> Period_ID { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<System.DateTime> update_date { get; set; }
    }
}