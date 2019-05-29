using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lotto.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index() //หน้าแรก
        {
            return View();
        }

        public ActionResult Bet() //แทงโพย
        {
            return View();
        }

        public ActionResult List() //ดูโพย
        {
            return View();
        }

        public ActionResult Return() //เลขคืน
        {
            return View();
        }

        public ActionResult Result() //ดูผล
        {
            return View();
        }

        public ActionResult Password() //รหัสผ่าน
        {
            return View();
        }

        public ActionResult Howto() //วิธีแทง
        {
            return View();
        }

        public ActionResult Contact() //ติดต่อเจ้ามือ
        {
            return View();
        }

    }
}