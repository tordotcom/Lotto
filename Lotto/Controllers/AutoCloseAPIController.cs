using Lotto.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lotto.Controllers
{
    public class AutoCloseAPIController : ApiController
    {
        ltfomEntities db = new ltfomEntities();
        [HttpGet]
        public void CheckTime()
        {
            var time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime t = DateTime.ParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dt = DateTime.ParseExact("15:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);
            if (t.TimeOfDay >= dt.TimeOfDay)
            {
                int maxpid = db.Period.Max(p => p.ID);
                Period P = db.Period.Where(x => x.ID == maxpid).FirstOrDefault<Period>();
                if (P != null && P.Status=="1")
                {
                    if (P.Date == t.Date)
                    {                            
                        P.update_date = DateTime.Now;
                        P.Status = "0";
                        P.BetStatus = "0";
                        P.Close_BY = "AUTO";
                        db.Entry(P).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
