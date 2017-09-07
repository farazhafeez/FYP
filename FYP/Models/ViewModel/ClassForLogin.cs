using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP.Models.ViewModel
{
    public partial class ClassForLogin
    {
        [Required(ErrorMessage = "Username required !")]
        public string User_Id { get; set; }
        [Required(ErrorMessage = "Password required !")]
        public string Password { get; set; }
    }
}