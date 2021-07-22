using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.NetCoreIdentity.Models
{
    public class UserCreateModel
    {
        [Required(ErrorMessage ="User Name Is Required")]
        public string UserName{ get; set; }
        [EmailAddress(ErrorMessage ="Please Your Email With Email Format")]
        [Required(ErrorMessage ="Email Is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }        
        [Compare("Password", ErrorMessage = "Passwords Can Not Matching")]
        public string ConfirmPassword { get; set; } 
        [Required(ErrorMessage ="Gender Is Required")]
        public string Gender { get; set; }
    }
}
