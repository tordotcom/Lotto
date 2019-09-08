using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class User_Role
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string create_date { get; set; }
        public string update_date { get; set; }
        public string Create_By_UID { get; set; }
        public string Last_Login { get; set; }
        public string Role { get; set; }
        public string Descript { get; set; }
    }
}