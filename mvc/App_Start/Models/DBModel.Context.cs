﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NotesMarketPlace.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class NotesMarketEntities2 : DbContext
    {
        public NotesMarketEntities2()
            : base("name=NotesMarketEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Download> Downloads { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<NotesAttachment> NotesAttachments { get; set; }
        public virtual DbSet<NotesReview> NotesReviews { get; set; }
        public virtual DbSet<ReferenceData> ReferenceDatas { get; set; }
        public virtual DbSet<ReportedIssue> ReportedIssues { get; set; }
        public virtual DbSet<Type> Types { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }

      
    }
}
