using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "กรุณากรอก Username")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string ID { get; set; }
        public string Last_Login{ get; set; }
        public string Role { get; set; }
        public bool Remember { get; set; }
        public string ParentID { get; set; }
    }
}