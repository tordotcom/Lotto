﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ltfomEntities : DbContext
    {
        public ltfomEntities()
            : base("name=ltfomEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Account_Role> Account_Role { get; set; }
        public virtual DbSet<Discount> Discount { get; set; }
        public virtual DbSet<LottoMain> LottoMain { get; set; }
        public virtual DbSet<LottoSub> LottoSub { get; set; }
        public virtual DbSet<Main_Discount> Main_Discount { get; set; }
        public virtual DbSet<Main_Rate> Main_Rate { get; set; }
        public virtual DbSet<Period> Period { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<Rate> Rate { get; set; }
        public virtual DbSet<Result> Result { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Total_Amount_Result> Total_Amount_Result { get; set; }
        public virtual DbSet<Poll_Image> Poll_Image { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<Lotto_Bet_Out> Lotto_Bet_Out { get; set; }
        public virtual DbSet<Poll_Bet_Out> Poll_Bet_Out { get; set; }
        public virtual DbSet<Account_Bet_Out> Account_Bet_Out { get; set; }
        public virtual DbSet<Lotto_Bet_Receive> Lotto_Bet_Receive { get; set; }
    }
}
