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
    using System.Web;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;

    public partial class NotesAttachment
    {
        public int ID { get; set; }
        public int NoteID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Note Note { get; set; }

       
        [NotMapped] public double FileSize { get {

                FileInfo fi = new FileInfo(FilePath);

               // long size = fi.Length;
            
                double len = new FileInfo(FilePath).Length;
               
               
               
                return len;
            } }
       
    }
}
