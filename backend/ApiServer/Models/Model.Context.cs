﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiServer.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DatabaseEntities : DbContext
    {
        public DatabaseEntities()
            : base("name=DatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Characteristic> Characteristic { get; set; }
        public virtual DbSet<Devices> Devices { get; set; }
        public virtual DbSet<Entities> Entities { get; set; }
        public virtual DbSet<Sockets> Sockets { get; set; }
        public virtual DbSet<Types> Types { get; set; }
        public virtual DbSet<DeviceToSocket> DeviceToSocket { get; set; }
        public virtual DbSet<DeviceToType> DeviceToType { get; set; }
    }
}