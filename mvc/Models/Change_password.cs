using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketPlace.Models
{
    public class Change_password
    {


        [Required(ErrorMessage = "Please enter your password")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{6,24}$", ErrorMessage = " between 6 and 24 characters long <br/>at least 1 lowercase character <br/>It must have at least 1 special character <br/> It must have at least 1-digit character <br/>It must not contain whitespaces")]


        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{6,24}$", ErrorMessage = " between 6 and 24 characters long <br/>at least 1 lowercase character <br/>It must have at least 1 special character <br/> It must have at least 1-digit character <br/>It must not contain whitespaces")]


        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please enter confirm your password")]
        [Compare("NewPassword", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}