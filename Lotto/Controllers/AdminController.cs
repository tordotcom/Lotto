using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lotto.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index() //หน้าแรก
        {
            return View();
        }

        public ActionResult AllBet() //ดูเลขทั้งหมด
        {
            return View();
        }

        public ActionResult Close() //ปิดหวย
        {
            return View();
        }

        public ActionResult List() //ดูโพย
        {
            return View();
        }
        public ActionResult Bet() //แทงโพย
        {
            return View();
        }
        public ActionResult BetTotal() //ยอดสรุปเป็นใบ
        {
            return View();
        }
        public ActionResult Out() //แทงออก
        {
            return View();
        }
        public ActionResult Cut() //ตัดเลข
        {
            return View();
        }
        public ActionResult OutTotal() //ยอดแทงออก
        {
            return View();
        }
        public ActionResult BetBackward() //ดูโพยย้อนหลัง
        {
            return View();
        }
        public ActionResult LottoBackward() //ดูหวยย้อนหลัง
        {
            return View();
        }
        public ActionResult Setting() //ตั้งค่า
        {
            return View();
        }
        public ActionResult Member() //สมาชิก
        {
            return View();
        }
        public ActionResult MemberPrice() //ราคาสมาชิก
        {
            return View();
        }
        public ActionResult DealerInfo() //ข้อมูลเจ้ามือ
        {
            return View();
        }
    }
}