using Lotto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Lotto.Controllers
{
    public class UserController : Controller
    {
        ltfomEntities db = new ltfomEntities();
        // GET: User
        public ActionResult Index() //หน้าแรก
        {
            return View();
        }

        public ActionResult Bet() //แทงโพย
        {
            Period P = db.Period.Where(x => x.BetStatus == "1").FirstOrDefault<Period>();
            if(P != null)
            {
                string connetionString = null;
                var pollDetail = new List<Poll_Detail>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT p.[ID],p.[Receive],sum(CAST(LS.Amount as int)) as amount,sum(CAST(ls.AmountDiscount as int)) as discount FROM [dbo].[Period] pe left join(select ID,Receive,Period_ID,UID FROM [dbo].[Poll] where UID=@UID) p on pe.ID=p.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where pe.ID=@period group by p.ID,p.Receive";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@UID", "2"); // user ID
                    cmd.Parameters.AddWithValue("@period", P.ID.ToString());
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
            else
            {
                var pollDetail = new List<Poll_Detail>();
                pollDetail.Add(new Poll_Detail
                {
                    Receive = "close"
                });
                return View(pollDetail);
            }
            
        }

        public ActionResult List() //ดูโพย
        {
            //Period P = db.Period.Where(x => x.Status == "1").FirstOrDefault<Period>();
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
                    cmd.Parameters.AddWithValue("@UID", "2"); // user ID
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
                return View(pollDetail);
            }
            return View();
        }

        public ActionResult Return() //เลขคืน
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
                    string query = "SELECT p.[ID],p.[Receive],p.[Create_By],sum(CAST(LS.Amount as int)) as amount,sum(CAST(ls.AmountDiscount as int)) as discount,p.create_date,p.update_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where p.Period_ID=@period and p.UID=@UID  group by p.ID,p.Receive,p.create_date,p.[Create_By],p.update_date";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@UID", "2"); // user ID
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
                                create_date = Reader["create_date"].ToString(),
                                update_date = Reader["update_date"].ToString(),
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
        //---------------------------------------------------------------------------------------------------//
        //-------------------------------------------function------------------------------------------------//
        //---------------------------------------------------------------------------------------------------//
        //---------------------------------------- แทงโพย ---------------------------------------------------//
        [HttpPost]
        public ActionResult addPoll(PollData Data) 
        {
            dynamic discount_rate;
            Period P = db.Period.Where(s => s.Status == "1").FirstOrDefault<Period>();
            Discount D = db.Discount.Where(x => x.Account.ID == 2).FirstOrDefault<Discount>(); //-----user-----id//
            if(D!=null)
            {
                discount_rate = D;
            }
            else
            {
                Main_Discount MD = db.Main_Discount.Where(x => 1 == 1).FirstOrDefault<Main_Discount>();
                discount_rate = MD;
            }

            var poll = new Poll();
            poll.UID = 2; //----------- user id-----------//
            poll.Period_ID = P.ID;
            poll.Receive = "1";
            poll.Create_By = "user1"; //--------- username --------//
            poll.create_date = DateTime.Now;
            poll.update_date = DateTime.Now;
            db.Poll.Add(poll);
            db.SaveChanges();
            int pID = poll.ID;

            foreach (var item in Data.poll)
            {
                var lotto = new LottoMain();
                lotto.Poll_ID = pID;
                lotto.Type = item.bType;
                lotto.Number = item.Number;
                lotto.Amount = item.Amount;
                lotto.create_date = DateTime.Now;
                lotto.update_date = DateTime.Now;
                db.LottoMain.Add(lotto);
                db.SaveChanges();
                int lID = lotto.ID;

                var NumLen = item.Number.Length;
                var amount = item.Amount.Replace("X","x");
                amount = item.Amount.Replace("*", "x");
                var AmtCountX = amount.Split('x').Length - 1;
                if(AmtCountX>0)
                {
                    //------------------------2 ตัว ----------------------------//
                    if(NumLen==2)
                    {
                        char[] num = item.Number.ToCharArray();
                        string[] amt = amount.Split('x');
                        var totalDiscount = 0;
                        int iamt = 0;
                        int d = 0;
                        var typ = item.bType;
                        if (typ == "t" || typ == "b")
                        {          
                            //--------------------- เช็ค 2 ตัวโต๊ด (บ 34 = x100)-----------------------//
                            if (amt[0] == "")
                            {
                                typ = "t_";
                            }
                            //---------------------------------------------------------------------//
                            if (typ == "b")
                            {
                                d = Int32.Parse(discount_rate.two_down);
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                //---------------------- 2 เต็ง -------------------------------//
                                InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString(), amt[0], totalDiscount);

                                //-------------------- 2 โต๊ด -------------------------------//
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, typ, num[1].ToString() + num[0].ToString(), amt[1], totalDiscount);
                            }
                            else if(typ == "t")
                            {
                                d = Int32.Parse(discount_rate.two_up);
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                //---------------------- 2 เต็ง -------------------------------//
                                InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString(), amt[0], totalDiscount);


                                //-------------------- 2 โต๊ด -------------------------------//
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, typ, num[1].ToString() + num[0].ToString(), amt[1], totalDiscount);
                            }
                            else if(typ == "t_")
                            {
                                d = Int32.Parse(discount_rate.two_ood);
                                //-------------------- 2 โต๊ด -------------------------------//
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                var sort = "";
                                if(Int32.Parse(num[0].ToString())< Int32.Parse(num[1].ToString()))
                                {
                                    sort = num[0].ToString() + num[1].ToString();
                                }
                                else
                                {
                                    sort = num[1].ToString() + num[0].ToString();
                                }
                                InsertLottoSub(lID, typ, sort, amt[1], totalDiscount);
                            }
                            else { }
                        }
                        else if(item.bType == "tb")
                        {
                            //---------------- 2 บน ---------------------------//
                            d = Int32.Parse(discount_rate.two_up);
                            iamt = Int32.Parse(amt[0]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", num[0].ToString() + num[1].ToString(), amt[0], totalDiscount);

                            //------------------ 2 บน โต๊ด-----------------------//
                            iamt = Int32.Parse(amt[1]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", num[1].ToString() + num[0].ToString(), amt[1], totalDiscount);

                            //------------------- 2 ล่าง ---------------------------//
                            d = Int32.Parse(discount_rate.down);
                            iamt = Int32.Parse(amt[0]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "b", num[0].ToString() + num[1].ToString(), amt[0], totalDiscount);

                            //------------------ 2 ล่าง โต๊ด-----------------------//
                            iamt = Int32.Parse(amt[1]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "b", num[1].ToString() + num[0].ToString(), amt[1], totalDiscount);
                        }
                        else
                        {
                            return Json("Fail");
                        }
                    }
                    //---------------------------------3 ตัว------------------------------------//
                    else if(NumLen == 3)
                    {
                        char[] num = item.Number.ToCharArray();
                        string[] amt = amount.Split('x');
                        var totalDiscount = 0;
                        int iamt = 0;
                        int d = 0;
                        var typ = item.bType;
                        var sort = "";
                        var temp = 'l';
                        if (typ == "t")
                        {
                            
                            //--------------------- เช็ค 3 ตัวโต๊ด (บ 434 = x100)-----------------------//
                            if (amt[0]=="")
                            {
                                typ = "t_";
                            }
                            if(typ=="t")
                            {
                                d = Int32.Parse(discount_rate.three_up);
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                //---------------------- 3 เต็ง -------------------------------//
                                InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString() + num[2].ToString(), amt[0], totalDiscount);

                                //-------------------- 3 โต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.three_ood);
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[2];
                                    num[2] = temp;
                                }
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[1].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[1];
                                    num[1] = temp;
                                }
                                if (Int32.Parse(num[1].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[1];
                                    num[1] = num[2];
                                    num[2] = temp;
                                }
                                sort = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                InsertLottoSub(lID, "t_", sort, amt[1], totalDiscount);
                            }
                            else
                            {
                                //-------------------- 3 โต๊ด (บ 957 = x100)-------------------------------//
                                d = Int32.Parse(discount_rate.three_ood);
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[2];
                                    num[2] = temp;
                                }
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[1].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[1];
                                    num[1] = temp;
                                }
                                if (Int32.Parse(num[1].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[1];
                                    num[1] = num[2];
                                    num[2] = temp;
                                }
                                sort = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                InsertLottoSub(lID, "t_", sort, amt[1], totalDiscount);
                            }
                        }
                        else if(typ == "f")
                        {
                            //--------------------- เช็ค 3 ตัวหน้าโต๊ด (ห 434 = x100)-----------------------//
                            if (amt[0] == "")
                            {
                                typ = "f_";
                            }
                            if (typ == "f")
                            {
                                d = Int32.Parse(discount_rate.first_three);
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                //---------------------- 3 หน้าเต็ง -------------------------------//
                                InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString() + num[2].ToString(), amt[0], totalDiscount);

                                //-------------------- 3 หน้าโต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.first_three_ood);
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[2];
                                    num[2] = temp;
                                }
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[1].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[1];
                                    num[1] = temp;
                                }
                                if (Int32.Parse(num[1].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[1];
                                    num[1] = num[2];
                                    num[2] = temp;
                                }
                                sort = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                InsertLottoSub(lID, "f_", sort, amt[1], totalDiscount);
                            }
                            else
                            {
                                //-------------------- 3 หน้าโต๊ด (ห 957 = x100)-------------------------------//
                                d = Int32.Parse(discount_rate.first_three_ood);
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[2];
                                    num[2] = temp;
                                }
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[1].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[1];
                                    num[1] = temp;
                                }
                                if (Int32.Parse(num[1].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[1];
                                    num[1] = num[2];
                                    num[2] = temp;
                                }
                                sort = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                InsertLottoSub(lID, "f_", sort, amt[1], totalDiscount);
                            }
                        }
                        else if(typ == "ft")
                        {
                            //--------------------- เช็ค 3 ตัวหน้าโต๊ด (ห+ท 434 = x100)-----------------------//
                            if (amt[0] == "")
                            {
                                typ = "ft_";
                            }
                            if (typ == "ft")
                            {
                                //---------------------- 3 หน้าเต็ง -------------------------------//
                                d = Int32.Parse(discount_rate.first_three);
                                iamt = Int32.Parse(amt[0])/2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "f", num[0].ToString() + num[1].ToString() + num[2].ToString(), iamt.ToString(), totalDiscount);
                                //---------------------- 3 บนเต็ง -------------------------------//
                                d = Int32.Parse(discount_rate.three_up);
                                iamt = Int32.Parse(amt[0]) / 2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", num[0].ToString() + num[1].ToString() + num[2].ToString(), iamt.ToString(), totalDiscount);
                                //-------------------- 3 หน้าโต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.first_three_ood);
                                iamt = Int32.Parse(amt[1])/2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[2];
                                    num[2] = temp;
                                }
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[1].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[1];
                                    num[1] = temp;
                                }
                                if (Int32.Parse(num[1].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[1];
                                    num[1] = num[2];
                                    num[2] = temp;
                                }
                                sort = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                InsertLottoSub(lID, "f_", sort, iamt.ToString(), totalDiscount);                 

                                //-------------------- 3 บนโต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.three_ood);
                                iamt = Int32.Parse(amt[1]) / 2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t_", sort, iamt.ToString(), totalDiscount);
                            }
                            else
                            {
                                //-------------------- 3 หน้าโต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.first_three_ood);
                                iamt = Int32.Parse(amt[1]) / 2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[2];
                                    num[2] = temp;
                                }
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[1].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[1];
                                    num[1] = temp;
                                }
                                if (Int32.Parse(num[1].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[1];
                                    num[1] = num[2];
                                    num[2] = temp;
                                }
                                sort = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                InsertLottoSub(lID, "f_", sort, iamt.ToString(), totalDiscount);

                                //-------------------- 3 บนโต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.three_ood);
                                iamt = Int32.Parse(amt[1]) / 2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[2];
                                    num[2] = temp;
                                }
                                if (Int32.Parse(num[0].ToString()) > Int32.Parse(num[1].ToString()))
                                {
                                    temp = num[0];
                                    num[0] = num[1];
                                    num[1] = temp;
                                }
                                if (Int32.Parse(num[1].ToString()) > Int32.Parse(num[2].ToString()))
                                {
                                    temp = num[1];
                                    num[1] = num[2];
                                    num[2] = temp;
                                }
                                sort = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                InsertLottoSub(lID, "t_", sort, iamt.ToString(), totalDiscount);
                            }
                        }
                        else
                        {
                            return Json("Fail");
                        }
                    }
                    else
                    {
                        return Json("Fail");
                    }
                }
                else
                {
                    if (NumLen == 1)
                    {
                        var totalDiscount = 0;
                        int iamt = 0;
                        int d = 0;
                        var typ = item.bType;
                        if (typ == "t" || typ == "b")
                        {
                            if (typ == "t")
                            {
                                //------------------------ วิ่งบน -------------------------------//
                                d = Int32.Parse(discount_rate.up);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount);
                            }
                            else
                            {
                                //---------------------- วิ่งล่าง -------------------------------//
                                d = Int32.Parse(discount_rate.down);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100); 
                                InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount);
                            }
                        }
                        else if (typ == "tb")
                        {
                            //------------------------ วิ่งบน -------------------------------//
                            d = Int32.Parse(discount_rate.up);
                            iamt = Int32.Parse(item.Amount);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", item.Number, item.Amount, totalDiscount);
                            //---------------------- วิ่งล่าง -------------------------------//
                            d = Int32.Parse(discount_rate.down);
                            iamt = Int32.Parse(item.Amount);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "b", item.Number, item.Amount, totalDiscount);
                        }
                        else { return Json("Fail"); }
                    }
                    else if (NumLen == 2)
                    {
                        var totalDiscount = 0;
                        int iamt = 0;
                        int d = 0;
                        var typ = item.bType;
                        if (typ == "t" || typ == "b")
                        {
                            if (typ == "t")
                            {
                                //------------------------ 2 บน -------------------------------//
                                d = Int32.Parse(discount_rate.two_up);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount);

                            }
                            else
                            {
                                d = Int32.Parse(discount_rate.two_down);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                //---------------------- 2 ล่าง -------------------------------//
                                InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount);
                            }
                        }
                        else if (typ == "tb")
                        {
                            //------------------------ 2 บน -------------------------------//
                            d = Int32.Parse(discount_rate.two_up);
                            iamt = Int32.Parse(item.Amount);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", item.Number, item.Amount, totalDiscount);

                            //---------------------- 2 ล่าง -------------------------------//
                            d = Int32.Parse(discount_rate.two_down);
                            iamt = Int32.Parse(item.Amount);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "b", item.Number, item.Amount, totalDiscount);

                        }
                        else { return Json("Fail"); }
                    }
                    else if (NumLen == 3)
                    {
                        var totalDiscount = 0;
                        int iamt = 0;
                        int d = 0;
                        var typ = item.bType;
                        if (typ == "t" || typ == "b" || typ == "f")
                        {
                            if (typ == "t")
                            {
                                //------------------------ 3 บน -------------------------------//
                                d = Int32.Parse(discount_rate.three_up);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount);
                            }
                            else if(typ == "f")
                            {
                                //------------------------ 3 หน้า -------------------------------//
                                d = Int32.Parse(discount_rate.first_three);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount);
                            }
                            else
                            {
                                //---------------------- 3 ล่าง -------------------------------//
                                d = Int32.Parse(discount_rate.three_down);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount);
                            }
                        }
                        else if (typ == "tb")
                        {
                            //------------------------ 3 บน -------------------------------//
                            d = Int32.Parse(discount_rate.three_up);
                            iamt = Int32.Parse(item.Amount);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", item.Number, item.Amount, totalDiscount);

                            //---------------------- 3 ล่าง -------------------------------//
                            d = Int32.Parse(discount_rate.three_down);
                            iamt = Int32.Parse(item.Amount);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "b", item.Number, item.Amount, totalDiscount);
                        }
                        else if(typ == "ft")
                        {
                            //------------------------ 3 บน -------------------------------//
                            d = Int32.Parse(discount_rate.two_up);
                            iamt = Int32.Parse(item.Amount)/2;
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", item.Number, iamt.ToString(), totalDiscount);
                            //---------------------- 3 หน้า -------------------------------//
                            d = Int32.Parse(discount_rate.two_down);
                            iamt = Int32.Parse(item.Amount)/2;
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "f", item.Number, iamt.ToString(), totalDiscount);
                        }
                        else { return Json("Fail"); }
                    }
                    else { return Json("Fail"); }
                }
            }
            return Json("ss");
        }
        public void InsertLottoSub(int lid,string Type, string number, string amount,int totalDiscount)
        {
            var lottosub = new LottoSub();
            lottosub.Lotto_ID = lid;
            lottosub.Type = Type;
            lottosub.Number = number;
            lottosub.Amount = amount;
            lottosub.AmountDiscount = totalDiscount.ToString();
            lottosub.create_date = DateTime.Now;
            lottosub.update_date = DateTime.Now;
            db.LottoSub.Add(lottosub);
            db.SaveChanges();
        }
        [HttpPost]
        public ActionResult getPoll(string id)
        {
            string connetionString = null;
            var lottoDetail = new List<Lotto_Detail>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT p.[ID],LM.Type,LM.Number,LM.Amount FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID],[Type],[Number],[Amount] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID where p.ID=@pID";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@pID", id.ToString());
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        lottoDetail.Add(new Lotto_Detail
                        {
                            Type = Reader["Type"].ToString(),
                            Amount = Reader["Amount"].ToString(),
                            Number = Reader["Number"].ToString(),
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
            return Json(lottoDetail);
        }
        //---------------------------------------- ดึงข้อมูลเลขที่ออก ---------------------------------------------------//

        [HttpPost]
        public ActionResult getResult(string date)
        {
            Result R = db.Result.Where(s => s.Lotto_day == date).FirstOrDefault<Result>();
            return Json(R);
        }
    }
}