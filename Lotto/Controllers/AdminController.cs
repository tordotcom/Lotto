﻿using Lotto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
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

        //------------------------------------------ function------------------------------------------------//

        [HttpPost]
        public ActionResult getRate()
        {
            string connetionString = null;
            var data = new List<Rate_Discount>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT [three_up],[three_ood],[three_down],[two_up],[two_ood],[two_down],[up],[down],[first_three], md.three_up_discount,md.three_ood_discount,md.three_down_discount,md.two_up_discount,md.two_ood_discount,md.two_down_discount,md.up_discount,md.down_discount,md.first_three_discount FROM[dbo].[Main_Rate] mr join(SELECT[three_up] as three_up_discount,[three_ood] as three_ood_discount,[three_down] as three_down_discount,[two_up] as two_up_discount,[two_ood] as two_ood_discount,[two_down] as two_down_discount,[up] as up_discount,[down] as down_discount,[first_three] as first_three_discount FROM[dbo].[Main_Discount]) md on 1 = 1";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        data.Add(new Rate_Discount
                        {
                            ThreeUP = Reader["three_up"].ToString(),
                            ThreeDown = Reader["three_down"].ToString(),
                            FirstThree = Reader["first_three"].ToString(),
                            ThreeOod = Reader["three_ood"].ToString(),
                            TwoUp = Reader["two_up"].ToString(),
                            TwoOod = Reader["two_ood"].ToString(),
                            TwoDown = Reader["two_down"].ToString(),
                            Up = Reader["up"].ToString(),
                            Down = Reader["down"].ToString(),
                            ThreeUP_discount = Reader["three_up_discount"].ToString(),
                            ThreeDown_discount = Reader["three_down_discount"].ToString(),
                            FirstThree_discount = Reader["first_three_discount"].ToString(),
                            ThreeOod_discount = Reader["three_ood_discount"].ToString(),
                            TwoUp_discount = Reader["two_up_discount"].ToString(),
                            TwoOod_discount = Reader["two_ood_discount"].ToString(),
                            TwoDown_discount = Reader["two_down_discount"].ToString(),
                            Up_discount = Reader["up_discount"].ToString(),
                            Down_discount = Reader["down_discount"].ToString()
                        });
                    }
                    cnn.Close();
                }
                catch
                {

                }
            }
            catch
            {

            }            
            return Json(data);
        }
    }
}