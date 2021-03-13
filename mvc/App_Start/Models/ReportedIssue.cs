//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ReportedIssue
    {
        public int ID { get; set; }
        public int NoteID { get; set; }
        public int ReportedByID { get; set; }
        public int DownloadID { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    
        public virtual Download Download { get; set; }
        public virtual Note Note { get; set; }
        public virtual User User { get; set; }
    }
}
