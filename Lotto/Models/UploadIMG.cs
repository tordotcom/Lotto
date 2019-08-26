using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class UploadIMG
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HttpPostedFileBase Image { get; set; }
    }
}