using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketPlace.Models
{
    public class User_UserProfile
    {
        public User User { get; set; }
        public UserProfile UserProfile { get; set; }

        public Country Country { get; set; }
    }
}