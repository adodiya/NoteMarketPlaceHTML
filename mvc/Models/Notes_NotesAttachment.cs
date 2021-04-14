using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class Notes_NotesAttachment
    {

        public Note Note { get; set; }
        
        public NotesAttachment  NotesAttachment { get; set; }

       public List<NotesAttachment> AttachmentList { get; set; }
    }
}