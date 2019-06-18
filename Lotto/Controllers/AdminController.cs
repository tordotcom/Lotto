using Lotto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Lotto.Controllers
{
    public class AdminController : Controller
    {
        ltfomEntities db = new ltfomEntities();
        // GET: Admin
        public ActionResult Index() //หน้าแรก
        {
            return View();
        }

        public ActionResult AllBet() //ดูเลขทั้งหมด
        {
            int id = db.Period.Max(p => p.ID);
            if (id != 0)
            {
                string connetionString = null;
                var all = new List<All_Number>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "select t1.Type,t1.Number,t1.Amount,t1.nLen,t2.max from (SELECT ls.Type, ls.Number,sum(CAST(LS.Amount as int)) as Amount,LEN(ls.Number) as nLen FROM[dbo].[Poll] p left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm on p.ID = lm.Poll_ID left join(SELECT[ID], [Lotto_ID], [Type], [Number], [Amount] FROM [dbo].[LottoSub]) ls on lm.ID = ls.Lotto_ID where p.Receive = '1' and p.Period_ID = @period group by ls.Type,ls.Number) t1 left join(select MAX(tt.num) as max from(select t.Type,t.nLen,COUNT(*) as num from(SELECT ls.Type,LEN(ls.Number) as nLen,ls.number  FROM[dbo].[Poll] p left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm on p.ID = lm.Poll_ID left join(SELECT[ID], [Lotto_ID], [Type], [Number], [Amount] FROM [dbo].[LottoSub]) ls on lm.ID = ls.Lotto_ID where p.Receive = '1' and p.Period_ID = @period group by ls.Type,LEN(ls.Number),ls.number) t group by t.Type,t.nLen) tt) t2 on 1=1 order by t1.Type , t1.Number";
                    //string query = "SELECT ls.Type, ls.Number,sum(CAST(LS.Amount as int)) as Amount,LEN(ls.Number) as nLen FROM[dbo].[Poll] p left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm on p.ID = lm.Poll_ID left join(SELECT[ID], [Lotto_ID], [Type], [Number], [Amount] FROM [dbo].[LottoSub]) ls on lm.ID = ls.Lotto_ID where p.Receive = '1' and p.Period_ID = @Period group by ls.Type,ls.Number order by ls.type , ls.Number";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@period", id.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            all.Add(new All_Number
                            {
                                Type = Reader["Type"].ToString(),
                                Number = Reader["Number"].ToString(),
                                Amount = Reader["Amount"].ToString(),
                                nLen = Reader["nLen"].ToString(),
                                Max = Reader["max"].ToString(),
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
                return View(all);
            }
            return View();
        }

        public ActionResult Close() //ปิดหวย
        {
            return View();
        }

        public ActionResult List() //ดูโพย
        {
            int id = db.Period.Max(p => p.ID);
            if (id != 0)
            {
                string connetionString = null;
                var pollDetail = new List<Poll_Detail>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "select t1.ID,t1.UID,t1.Name,t1.Receive,t1.Create_By,t1.amount,t1.discount,t1.create_date,Row_Number() OVER (Partition BY t1.UID ORDER BY t1.UID) as rNumber from(SELECT p.[ID],p.UID,a.Name,p.[Receive],p.[Create_By],sum(CAST(LS.Amount as int)) as amount,sum(CAST(ls.AmountDiscount as int)) as discount,p.create_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID  left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID  left join(SELECT [ID],[Name] FROM [dbo].[Account]) a on p.UID =a.ID where p.Period_ID=@period group by p.ID,p.Receive,p.create_date,p.[Create_By],a.Name,p.UID) t1 order by Row_Number() OVER (Partition BY t1.Name ORDER BY t1.Name) desc";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@period", id.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            pollDetail.Add(new Poll_Detail
                            {
                                ID = Reader["ID"].ToString(),
                                name = Reader["Name"].ToString(),
                                Receive = Reader["Receive"].ToString(),
                                Amount = Reader["amount"].ToString(),
                                Discount = Reader["discount"].ToString(),
                                Create_By = Reader["Create_By"].ToString(),
                                create_date = Reader["create_date"].ToString(),
                                poll_number = Reader["rNumber"].ToString(),
                                UID = Reader["UID"].ToString(),
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
                return View(pollDetail);
            }
            return View();
        }
        public ActionResult Bet() //แทงโพย
        {
            return View();
        }
        public ActionResult BetTotal() //ยอดสรุปเป็นใบ
        {
            int id = db.Period.Max(p => p.ID);
            if (id != 0)
            {
                string connetionString = null;
                var all = new List<total_bet>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT po.UID,a.Name,r.poll_receive,rr.poll_reject,sum(CAST(LS.Amount as int)) as Amount,sum(CASE WHEN po.Receive ='0' THEN 0WHEN po.Receive = '1' THEN CAST(LS.Amount as int)END) as AmountReceive,sum(CASE WHEN po.Receive ='0' THEN 0 WHEN po.Receive = '1' THEN CAST(LS.AmountDiscount as int)END) as AmountDiscount FROM [dbo].[Period] p left join(SELECT [ID],[UID],[Period_ID],[Receive] FROM [dbo].[Poll]) po on p.ID=po.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[Type],[Number],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID left join(SELECT [ID],[Name] FROM [dbo].[Account] where Status='1') a on po.UID=a.ID left join(SELECT Period_ID,count(*) as poll_receive FROM [dbo].[Poll] where [Receive]='1' group by [UID],[Period_ID],[Receive] ) r on p.ID=r.Period_ID left join(SELECT Period_ID,count(*) as poll_reject FROM [dbo].[Poll] where [Receive]='0' group by [UID],[Period_ID],[Receive] ) rr on p.ID=rr.Period_ID where p.id=@period group by po.UID,a.Name,r.poll_receive,rr.poll_reject";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@period", id.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            all.Add(new total_bet
                            {
                                UID = Reader["UID"].ToString(),
                                Name = Reader["Name"].ToString(),
                                Amount = Reader["Amount"].ToString(),
                                AmountDiscount = Reader["AmountDiscount"].ToString(),
                                AmountReceive = Reader["AmountReceive"].ToString(),
                                Receive = Reader["poll_receive"].ToString(),
                                Reject = Reader["poll_reject"].ToString(),
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
                return View(all);
            }
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
        public ActionResult Result() //ดูเลขหวย
        {
            return View(db.Result.ToList());
        }
        public ActionResult Setting() //ตั้งค่า
        {
            return View();
        }
        public ActionResult Member() //สมาชิก
        {
            string connetionString = null;
            var user = new List<User_Role>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT a.[ID], a.[Username],a.[Name],a.[Description],a.[Status],a.[create_date],a.Create_By_UID, a.Last_Login,a.update_date,r.Role FROM[dbo].[Account] a left join(SELECT TOP (1000) [ID],[UID],[Role_ID] FROM[dbo].[Account_Role]) ar on a.ID=ar.UID left join(SELECT[ID], [Role] FROM [dbo].[Role]) r on ar.Role_ID=r.ID";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        user.Add(new User_Role
                        {
                            ID = Reader["ID"].ToString(),
                            Username = Reader["Username"].ToString(),
                            Name = Reader["Name"].ToString(),
                            Description = Reader["Description"].ToString(),
                            Status = Reader["Status"].ToString(),
                            create_date = Reader["create_date"].ToString(),
                            update_date = Reader["update_date"].ToString(),
                            Create_By_UID = Reader["Create_By_UID"].ToString(),
                            Last_Login = Reader["Last_Login"].ToString(),
                            Role = Reader["Role"].ToString()
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
            if (user.Count > 0)
            {
                for(int i=0;i<user.Count;i++)
                {
                    bool role = Check_Role("user", user[i].Role);
                    if(!role)
                    {
                        user.RemoveAt(i);
                    }
                }
            }                
            return View(user);
        }
        public ActionResult MemberPrice() //ราคาสมาชิก
        {
            string connetionString = null;
            var user = new List<User_Rate_Discount>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT a.[ID], a.[Username],a.[Name],r.Role ,ra.[ID] as rateID,ra.[three_up],ra.[three_ood],ra.[three_down],ra.[two_up],ra.[two_ood],ra.[two_down],ra.[up],ra.[down],ra.[first_three],ra.[first_three_ood],d.[ID] as discountID,d.[three_up] as three_up_d,d.[three_ood] as three_ood_d,d.[three_down] as three_down_d,d.[two_up] as two_up_d,d.[two_ood] as two_ood_d,d.[two_down] as two_down_d,d.[up] as up_d,d.[down] as down_d,d.[first_three] as first_three_d,d.[first_three_ood] as first_three_ood_d FROM[dbo].[Account] a left join(SELECT[ID],[UID],[Role_ID] FROM[dbo].[Account_Role]) ar on a.ID = ar.UID left join(SELECT[ID], [Role] FROM[dbo].[Role]) r on ar.Role_ID = r.ID left join(SELECT [ID], [UID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Rate]) ra on a.ID = ra.UID left join(SELECT [ID], [UID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Discount]) d on a.ID = d.UID where ra.UID is not null";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        user.Add(new User_Rate_Discount
                        {
                            ID = Reader["ID"].ToString(),
                            rateID = Reader["rateID"].ToString(),
                            discountID = Reader["discountID"].ToString(),
                            Username = Reader["Username"].ToString(),
                            Name = Reader["Name"].ToString(),
                            Role = Reader["Role"].ToString(),
                            ThreeUP = Reader["three_up"].ToString(),
                            ThreeDown = Reader["three_down"].ToString(),
                            ThreeOod = Reader["three_ood"].ToString(),
                            FirstThree = Reader["first_three"].ToString(),
                            FirstThreeOod = Reader["first_three_ood"].ToString(),
                            TwoUp = Reader["two_up"].ToString(),
                            TwoOod = Reader["two_ood"].ToString(),
                            TwoDown = Reader["two_down"].ToString(),
                            Up = Reader["up"].ToString(),
                            Down = Reader["down"].ToString(),
                            ThreeUP_discount = Reader["three_up_d"].ToString(),
                            ThreeDown_discount = Reader["three_down_d"].ToString(),
                            ThreeOod_discount = Reader["three_ood_d"].ToString(),
                            FirstThree_discount = Reader["first_three_d"].ToString(),
                            FirstThreeOod_discount = Reader["first_three_ood_d"].ToString(),
                            TwoUp_discount = Reader["two_up_d"].ToString(),
                            TwoOod_discount = Reader["two_ood_d"].ToString(),
                            TwoDown_discount = Reader["two_down_d"].ToString(),
                            Up_discount = Reader["up_d"].ToString(),
                            Down_discount = Reader["down_d"].ToString(),
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
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT a.[ID], a.[Username],a.[Name],r.Role ,ra.[three_up],ra.[three_ood],ra.[three_down],ra.[two_up],ra.[two_ood],ra.[two_down],ra.[up],ra.[down],ra.[first_three],ra.first_three_ood,d.[three_up] as three_up_d,d.[three_ood] as three_ood_d,d.[three_down] as three_down_d,d.[two_up] as two_up_d,d.[two_ood] as two_ood_d,d.[two_down] as two_down_d,d.[up] as up_d,d.[down] as down_d,d.[first_three] as first_three_d,d.first_three_ood as first_three_ood_d FROM[dbo].[Account] a left join(SELECT[ID],[UID],[Role_ID] FROM[dbo].[Account_Role]) ar on a.ID = ar.UID left join(SELECT[ID], [Role] FROM[dbo].[Role]) r on ar.Role_ID = r.ID left join(SELECT[ID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Main_Rate]) ra on 1 = 1 left join(SELECT[ID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Main_Discount]) d on 1 = 1 left join(SELECT[ID], [UID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three] FROM[dbo].[Rate]) rate on a.ID = rate.UID where rate.UID is null";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        user.Add(new User_Rate_Discount
                        {
                            ID = Reader["ID"].ToString(),
                            Username = Reader["Username"].ToString(),
                            Name = Reader["Name"].ToString(),
                            Role = Reader["Role"].ToString(),
                            ThreeUP = Reader["three_up"].ToString(),
                            ThreeDown = Reader["three_down"].ToString(),
                            ThreeOod = Reader["three_ood"].ToString(),
                            FirstThree = Reader["first_three"].ToString(),
                            FirstThreeOod = Reader["first_three_ood"].ToString(),
                            TwoUp = Reader["two_up"].ToString(),
                            TwoOod = Reader["two_ood"].ToString(),
                            TwoDown = Reader["two_down"].ToString(),
                            Up = Reader["up"].ToString(),
                            Down = Reader["down"].ToString(),
                            ThreeUP_discount = Reader["three_up_d"].ToString(),
                            ThreeDown_discount = Reader["three_down_d"].ToString(),
                            ThreeOod_discount = Reader["three_ood_d"].ToString(),
                            FirstThree_discount = Reader["first_three_d"].ToString(),
                            FirstThreeOod_discount = Reader["first_three_ood_d"].ToString(),
                            TwoUp_discount = Reader["two_up_d"].ToString(),
                            TwoOod_discount = Reader["two_ood_d"].ToString(),
                            TwoDown_discount = Reader["two_down_d"].ToString(),
                            Up_discount = Reader["up_d"].ToString(),
                            Down_discount = Reader["down_d"].ToString(),
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
            if (user.Count > 0)
            {
                for (int i = 0; i < user.Count; i++)
                {
                    bool role = Check_Role("user", user[i].Role);
                    if (!role)
                    {
                        user.RemoveAt(i);
                    }
                }
            }
            return View(user);
        }
        public ActionResult DealerInfo() //ข้อมูลเจ้ามือ
        {
            return View();
        }

        //---------------------------------------------------------------------------------------------------//
        //-------------------------------------------function------------------------------------------------//
        //---------------------------------------------------------------------------------------------------//
        //----------------------------------------check role-------------------------------------------------//
        public bool Check_Role(string plainText, string hashValue)
        {
            bool verify = VerifyHash(plainText, hashValue);
            return verify;
        }
        //------------------------------------------compute hash---------------------------------------------//
        public string ComputeHash(string plainText, byte[] saltBytes)
        {
            if (saltBytes == null)
            {
                int minSaltSize = 4;
                int maxSaltSize = 8;
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);
                saltBytes = new byte[saltSize];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetNonZeroBytes(saltBytes);
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

            for (int i = 0; i < plainTextBytes.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainTextBytes[i];
            }

            for (int i = 0; i < saltBytes.Length; i++)
            {
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];
            }

            HashAlgorithm hash;
            hash = new SHA256Managed();

            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashWithSaltBytes[i] = hashBytes[i];
            }

            for (int i = 0; i < saltBytes.Length; i++)
            {
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];
            }

            string hashValue = Convert.ToBase64String(hashWithSaltBytes);
            return hashValue;
        }
        //-----------------------------------------verify role-----------------------------------------------//
        public bool VerifyHash(string plainText, string hashValue)
        {
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);
            int hashSizeInBits, hashSizeInBytes;
            hashSizeInBits = 256;
            hashSizeInBytes = hashSizeInBits / 8;

            if (hashWithSaltBytes.Length < hashSizeInBytes)
            {
                return false;
            }

            byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

            for (int i = 0; i < saltBytes.Length; i++)
            {
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];
            }

            string expectedHashString = ComputeHash(plainText, saltBytes);

            return (hashValue == expectedHashString);
        }
        //------------------------------------get rate and discount------------------------------------------//
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
                string query = "SELECT [three_up],[three_ood],[three_down],[two_up],[two_ood],[two_down],[up],[down],[first_three],[first_three_ood], md.three_up_discount,md.three_ood_discount,md.three_down_discount,md.two_up_discount,md.two_ood_discount,md.two_down_discount,md.up_discount,md.down_discount,md.first_three_discount,md.first_three_ood_discount FROM[dbo].[Main_Rate] mr join(SELECT[three_up] as three_up_discount,[three_ood] as three_ood_discount,[three_down] as three_down_discount,[two_up] as two_up_discount,[two_ood] as two_ood_discount,[two_down] as two_down_discount,[up] as up_discount,[down] as down_discount,[first_three] as first_three_discount,[first_three_ood] as first_three_ood_discount FROM[dbo].[Main_Discount]) md on 1 = 1";
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
                            FirstThreeOod = Reader["first_three_ood"].ToString(),
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
                            FirstThreeOod_discount = Reader["first_three_ood_discount"].ToString(),
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

        //-------------------------------------update rate and discount--------------------------------//
        [HttpPost]
        public ActionResult UpdateRateDiscount(Rate_Discount RateDiscountArr)
        {
            Main_Rate MR = db.Main_Rate.Where(s => s.ID == 1).FirstOrDefault<Main_Rate>();
            Main_Discount MD = db.Main_Discount.Where(s => s.ID == 1).FirstOrDefault<Main_Discount>();
            if (MR != null && MD != null)
            {
                MR.update_date = DateTime.Now;
                MR.three_up = RateDiscountArr.ThreeUP;
                MR.three_ood = RateDiscountArr.ThreeOod;
                MR.three_down = RateDiscountArr.ThreeDown;
                MR.two_up = RateDiscountArr.TwoUp;
                MR.two_ood = RateDiscountArr.TwoOod;
                MR.two_down = RateDiscountArr.TwoDown;
                MR.up = RateDiscountArr.Up;
                MR.down = RateDiscountArr.Down;
                MR.first_three = RateDiscountArr.FirstThree;
                MR.first_three_ood = RateDiscountArr.FirstThreeOod;
                db.Entry(MR).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                MD.update_date = DateTime.Now;
                MD.three_up = RateDiscountArr.ThreeUP_discount;
                MD.three_ood = RateDiscountArr.ThreeOod_discount;
                MD.three_down = RateDiscountArr.ThreeDown_discount;
                MD.two_up = RateDiscountArr.TwoUp_discount;
                MD.two_ood = RateDiscountArr.TwoOod_discount;
                MD.two_down = RateDiscountArr.TwoDown_discount;
                MD.up = RateDiscountArr.Up_discount;
                MD.down = RateDiscountArr.Down_discount;
                MD.first_three = RateDiscountArr.FirstThree_discount;
                MD.first_three_ood = RateDiscountArr.FirstThreeOod_discount;
                db.Entry(MD).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }
        //-------------------------------------update user rate and discount--------------------------------//
        [HttpPost]
        public ActionResult UpdateUserRateDiscount(User_Rate_Discount User_Rate_Discount)
        {
            if(User_Rate_Discount.rateID != "null")
            {
                int rateID = Int32.Parse(User_Rate_Discount.rateID);
                int discountID = Int32.Parse(User_Rate_Discount.discountID);
                Rate R = db.Rate.Where(s => s.ID == rateID).FirstOrDefault<Rate>();
                Discount D = db.Discount.Where(s => s.ID == discountID).FirstOrDefault<Discount>();
                if (R != null && D != null)
                {
                    R.update_date = DateTime.Now;
                    R.three_up = User_Rate_Discount.ThreeUP;
                    R.three_ood = User_Rate_Discount.ThreeOod;
                    R.three_down = User_Rate_Discount.ThreeDown;
                    R.two_up = User_Rate_Discount.TwoUp;
                    R.two_ood = User_Rate_Discount.TwoOod;
                    R.two_down = User_Rate_Discount.TwoDown;
                    R.up = User_Rate_Discount.Up;
                    R.down = User_Rate_Discount.Down;
                    R.first_three = User_Rate_Discount.FirstThree;
                    R.first_three_ood = User_Rate_Discount.FirstThreeOod;
                    db.Entry(R).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    D.update_date = DateTime.Now;
                    D.three_up = User_Rate_Discount.ThreeUP_discount;
                    D.three_ood = User_Rate_Discount.ThreeOod_discount;
                    D.three_down = User_Rate_Discount.ThreeDown_discount;
                    D.two_up = User_Rate_Discount.TwoUp_discount;
                    D.two_ood = User_Rate_Discount.TwoOod_discount;
                    D.two_down = User_Rate_Discount.TwoDown_discount;
                    D.up = User_Rate_Discount.Up_discount;
                    D.down = User_Rate_Discount.Down_discount;
                    D.first_three = User_Rate_Discount.FirstThree_discount;
                    D.first_three_ood = User_Rate_Discount.FirstThreeOod_discount;
                    db.Entry(D).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("ss");
                }
                else
                {
                    return Json("fail");
                }
            }
            else
            {
                int ID = Int32.Parse(User_Rate_Discount.ID);
                Rate R = new Rate();
                R.UID = ID;
                R.update_date = DateTime.Now;
                R.create_date = DateTime.Now;
                R.three_up = User_Rate_Discount.ThreeUP;
                R.three_ood = User_Rate_Discount.ThreeOod;
                R.three_down = User_Rate_Discount.ThreeDown;
                R.two_up = User_Rate_Discount.TwoUp;
                R.two_ood = User_Rate_Discount.TwoOod;
                R.two_down = User_Rate_Discount.TwoDown;
                R.up = User_Rate_Discount.Up;
                R.down = User_Rate_Discount.Down;
                R.first_three = User_Rate_Discount.FirstThree;
                R.first_three_ood = User_Rate_Discount.FirstThreeOod;
                db.Rate.Add(R);
                db.SaveChanges();

                Discount D = new Discount();
                D.UID = ID;
                D.update_date = DateTime.Now;
                D.create_date = DateTime.Now;
                D.three_up = User_Rate_Discount.ThreeUP_discount;
                D.three_ood = User_Rate_Discount.ThreeOod_discount;
                D.three_down = User_Rate_Discount.ThreeDown_discount;
                D.two_up = User_Rate_Discount.TwoUp_discount;
                D.two_ood = User_Rate_Discount.TwoOod_discount;
                D.two_down = User_Rate_Discount.TwoDown_discount;
                D.up = User_Rate_Discount.Up_discount;
                D.down = User_Rate_Discount.Down_discount;
                D.first_three = User_Rate_Discount.FirstThree_discount;
                D.first_three_ood = User_Rate_Discount.FirstThreeOod_discount;
                db.Discount.Add(D);
                db.SaveChanges();
                return Json("ss");
            }
        }

         //-------------------------------------Add User data --------------------------------//
        [HttpPost]
        public ActionResult AddUser(Account User)
        {
            if (User != null)
            {
                
                var A = new Account();
                A.Username = User.Username;
                A.Name = User.Name;
                A.Password = ComputeHash(User.Password, null);
                A.Status = User.Status;
                A.Description = User.Description; 
                A.Create_By_UID = Convert.ToString(Session["ID"]); 
                A.Last_Login = DateTime.Now; 
                A.create_date = DateTime.Now;
                A.update_date = DateTime.Now;
                db.Account.Add(A);
                db.SaveChanges();

                var R = new Account_Role();
                int lastAccount = db.Account.Max(item => item.ID);
                R.UID = lastAccount;
                R.Role_ID = 2;
                R.create_date = DateTime.Now;
                R.update_date = DateTime.Now;
                db.Account_Role.Add(R);
                db.SaveChanges();

                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Update User data --------------------------------//
        [HttpPost]
        public ActionResult UpdateUser(Account User)
        {
            Account A = db.Account.Where(s => s.ID == User.ID).FirstOrDefault<Account>();
            if (A != null)
            {
                if (User.Password != null)
                {                   
                    A.Password = ComputeHash(User.Password, null);
                }
                A.update_date = DateTime.Now;
                A.Name = User.Name;
                A.Description = User.Description;
                A.Status = User.Status;
                db.Entry(A).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Delete User data --------------------------------//
        [HttpPost]
        public ActionResult DeleteUser(Account User)
        {
            Account A = db.Account.Where(s => s.ID == User.ID).FirstOrDefault<Account>();
            Account_Role AR = db.Account_Role.Where(s => s.UID == User.ID).First();
            if (A != null)
            {
                if (AR != null) {
                    db.Account_Role.Remove(AR);
                    db.SaveChanges();
                }
                db.Account.Remove(A);
                db.SaveChanges();
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Add Result data --------------------------------//
        [HttpPost]
        public ActionResult AddResult(Result Result)
        {
            if (Result != null)
            {

                var R = new Result();
                R.Name = Result.Name;
                R.Lotto_day = Result.Lotto_day;
                R.first_three = Result.first_three;
                R.last_three = Result.last_three;
                R.three_down_1 = Result.three_down_1;
                R.three_down_2 = Result.three_down_2;
                R.three_down_3 = Result.three_down_3;
                R.three_down_4 = Result.three_down_4;
                R.two_down = Result.two_down;
                R.create_date = DateTime.Now;
                R.update_date = DateTime.Now;
                db.Result.Add(R);
                db.SaveChanges();

                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Update Result data --------------------------------//
        [HttpPost]
        public ActionResult UpdatResult(Result Result)
        {
            Result R = db.Result.Where(s => s.ID == Result.ID).FirstOrDefault<Result>();
            if (R != null)
            {
                R.update_date = DateTime.Now;

                R.Name = Result.Name;
                R.Lotto_day = Result.Lotto_day;
                R.three_down_1 = Result.three_down_1;
                R.three_down_2 = Result.three_down_2;
                R.three_down_3 = Result.three_down_3;
                R.three_down_4 = Result.three_down_4;
                R.two_down = Result.two_down;
                db.Entry(R).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Delete Result data --------------------------------//
        [HttpPost]
        public ActionResult DeleteResult(Result Result)
        {
            Result R = db.Result.Where(s => s.ID == Result.ID).First();
            if (R != null)
            {
                db.Result.Remove(R);
                db.SaveChanges();
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Get Current Period --------------------------------//
        [HttpPost]
        public ActionResult GetCurrentPeriod()
        {
            Period current = db.Period.Where(s => s.Status == "1").SingleOrDefault();

            return Json(new { Date = current.Date.Value.ToString("dd/MM/yyyy") , Status = current.Status });
        }

        //-------------------------------------Get Setting --------------------------------//
        [HttpPost]
        public ActionResult GetSetting()
        {
            return Json(new { Setting = db.Setting }, JsonRequestBehavior.AllowGet);
        }

        //-------------------------------------Update Setting --------------------------------//
        [HttpPost]
        public ActionResult UpdateSetting(List<Setting> S)
        {
            if (S != null)
            {
                foreach (var setting in S)
                {
                    Setting x = db.Setting.Where(r => r.Name == setting.Name).First();
                    if (x != null)
                    {
                        x.Value = setting.Value;
                        x.update_date = DateTime.Now;
                        db.Entry(x).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }
        //---------------------------------- get poll header detail Admin/bet------------------------------------------//
       [HttpPost]
       public ActionResult getPoll(string UID)
        {
            int id = db.Period.Max(p => p.ID);
            if (id != 0)
            {
                string connetionString = null;
                var pollDetail = new List<Poll_Detail>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT p.[ID],p.[Receive],p.[Create_By],sum(CAST(LS.Amount as int)) as amount,sum(CAST(ls.AmountDiscount as int)) as discount,p.create_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where p.Period_ID=@period and p.UID=@UID group by p.ID,p.Receive,p.create_date,p.[Create_By]";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@UID", UID.ToString()); // user ID
                    cmd.Parameters.AddWithValue("@period", id.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            pollDetail.Add(new Poll_Detail
                            {
                                ID = Reader["ID"].ToString(),
                                Receive = Reader["Receive"].ToString(),
                                Amount = Reader["amount"].ToString(),
                                Discount = Reader["discount"].ToString(),
                                Create_By = Reader["Create_By"].ToString(),
                                create_date = Reader["create_date"].ToString()
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
                return Json(pollDetail);
            }
            return Json("fail");
        }

        //------------------------------------- Reject poll --------------------------------//
        [HttpPost]
        public ActionResult RejectPoll(string PollID)
        {
            int PID = Int32.Parse(PollID);
            Poll P = db.Poll.Find(PID);

            if (P == null)
            {
                return HttpNotFound();
            }
            else
            {
                P.update_date = DateTime.Now;
                P.Receive = "0";
                db.Entry(P).State = EntityState.Modified;
                db.SaveChanges();
                return Json("ss");
            }
        }
    }
}