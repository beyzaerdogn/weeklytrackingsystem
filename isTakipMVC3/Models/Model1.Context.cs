﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace isTakipMVC3.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class İsTakipDBEntities1 : DbContext
    {
        public İsTakipDBEntities1()
            : base("name=İsTakipDBEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Birimler> Birimler { get; set; }
        public virtual DbSet<durumlar_> durumlar_ { get; set; }
        public virtual DbSet<isler> isler { get; set; }
        public virtual DbSet<Personeller> Personeller { get; set; }
        public virtual DbSet<yetkiTurid> yetkiTurid { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
    }
}
