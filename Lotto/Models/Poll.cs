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
    
    public partial class Poll
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Poll()
        {
            this.LottoMain = new HashSet<LottoMain>();
        }
    
        public int ID { get; set; }
        public Nullable<int> UID { get; set; }
        public Nullable<int> Period_ID { get; set; }
        public string Receive { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<System.DateTime> update_date { get; set; }
        public string Poll_Name { get; set; }
        public string IP { get; set; }
        public string Check_Status { get; set; }
    
        public virtual Account Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LottoMain> LottoMain { get; set; }
        public virtual Period Period { get; set; }
    }
}
