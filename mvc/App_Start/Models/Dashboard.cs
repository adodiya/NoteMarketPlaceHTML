using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class Dashboard
    {
        public List<Note> ListA { get; set; }
        public List<Note> ListB { get; set; }

        public Nullable<int> count { get; set; }

        public Nullable<decimal> sum { get; set; }
       
    }
}