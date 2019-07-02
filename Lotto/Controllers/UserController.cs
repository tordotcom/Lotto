using Lotto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            if ((string)Session["Role"] == "User")
            {
                return View();
            }
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Bet() //แทงโพย
        {
            if ((string)Session["Role"] == "User")
            {
                Period P = db.Period.Where(x => x.BetStatus == "1").FirstOrDefault<Period>();
                if(P != null)
                {
                    string connetionString = null;
                    var pollDetail = new List<Poll_Detail>();
                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        var user_id = Session["ID"].ToString();
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "SELECT p.[ID],p.[Receive],sum(CAST(LS.Amount as int)) as amount,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as discount FROM [dbo].[Period] pe left join(select ID,Receive,Period_ID,UID FROM [dbo].[Poll] where UID=@UID) p on pe.ID=p.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where pe.ID=@period group by p.ID,p.Receive";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@UID", user_id); // user ID
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
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult List() //ดูโพย
        {
            if ((string)Session["Role"] == "User")
            {
                //Period P = db.Period.Where(x => x.Status == "1").FirstOrDefault<Period>();
                int id = db.Period.Max(p => p.ID);
                var user_id = Session["ID"].ToString();
                if (id != 0)
                {
                    string connetionString = null;
                    var pollDetail = new List<Poll_Detail>();
                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "SELECT p.[ID],p.[Receive],p.[Create_By],sum(CAST(LS.Amount as int)) as amount,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as discount,sum(CAST(ls.AmountWin as int)) as Win,p.create_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount],[AmountWin] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where p.Period_ID=@period and p.UID=@UID group by p.ID,p.Receive,p.create_date,p.[Create_By]";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@UID", user_id); // user ID
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
                                    Win = Reader["Win"].ToString(),
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
                    var total_type_bet = new List<user_total>();
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "select SUM(CAST(t.Amount as int)) as Amount,ROUND(sum(CAST(t.AmountDiscount as float)),0) as AmountDiscount,SUM(CAST(t.AmountWin as int)) as AmountWin,t.Type from (SELECT ls.Amount,ls.AmountDiscount,ls.AmountWin,CASE WHEN ls.Type='t' and ls.NumLen='1' THEN 'Up' WHEN ls.Type='b' and ls.NumLen='1' THEN 'Down' WHEN ls.Type='f' and ls.NumLen='3' THEN 'FirstThree' WHEN ls.Type='f_' and ls.NumLen='3' THEN 'FirstThreeOod' WHEN ls.Type='t' and ls.NumLen='3' THEN 'ThreeUp' WHEN ls.Type='t_' and ls.NumLen='3' THEN 'ThreeUPOod' WHEN ls.Type='b' and ls.NumLen='3' THEN 'ThreeDown' WHEN ls.Type='t' and ls.NumLen='2' THEN 'TwoUp' WHEN ls.Type='b' and ls.NumLen='2' THEN 'TwoDown' WHEN (ls.Type='t_' or ls.Type='b_') and ls.NumLen='2' THEN 'TwoOod' ELSE null END as Type FROM [dbo].[Period] pe left join(SELECT [ID],[UID],[Receive],[Period_ID] FROM [dbo].[Poll]) po on pe.ID=po.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[Type],[NumLen],[Amount],[AmountDiscount],[AmountWin] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where pe.ID=@period and po.UID=@uid and po.Receive='1') t group by t.Type";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@period", id.ToString());
                        cmd.Parameters.AddWithValue("@uid", user_id);
                        SqlDataReader Reader = cmd.ExecuteReader();
                        Console.Write(Reader);
                        try
                        {
                            while (Reader.Read())
                            {
                                total_type_bet.Add(new user_total
                                {
                                    Type = Reader["Type"].ToString(),
                                    Amount= Reader["Amount"].ToString(),
                                    Amount_Discount = Reader["AmountDiscount"].ToString(),
                                    Amount_Win = Reader["AmountWin"].ToString(),

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
                    ViewData["TAR"] = total_type_bet;
                    return View(pollDetail);
                }
                return View();
            }
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Return() //เลขคืน
        {
            if ((string)Session["Role"] == "User")
            {
                int id = db.Period.Max(p => p.ID);
                if (id != 0)
                {
                    string connetionString = null;
                    var pollDetail = new List<Poll_Detail>();
                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        var user_id = Session["ID"].ToString();
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "SELECT p.[ID],p.[Receive],p.[Create_By],sum(CAST(LS.Amount as int)) as amount,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as discount,p.create_date,p.update_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where p.Period_ID=@period and p.UID=@UID  group by p.ID,p.Receive,p.create_date,p.[Create_By],p.update_date";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@UID", user_id); // user ID
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
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Result() //ดูผล
        {
            if ((string)Session["Role"] == "User")
            {
                List<Result> r = db.Result.OrderByDescending(x=>x.ID).ToList();
                return View(r);
            }
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Password() //รหัสผ่าน
        {
            if ((string)Session["Role"] == "User")
            {
                var id = Int32.Parse(Session["ID"].ToString());
                Account a = db.Account.Where(x => x.ID == id).FirstOrDefault<Account>();
                return View(a);
            }
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Howto() //วิธีแทง
        {
            if ((string)Session["Role"] == "User")
            {
                return View();
            }
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Contact() //ติดต่อเจ้ามือ
        {
            if ((string)Session["Role"] == "User")
            {
                return View();
            }
            else if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        //---------------------------------------------------------------------------------------------------//
        //-------------------------------------------function------------------------------------------------//
        //---------------------------------------------------------------------------------------------------//
        //---------------------------------------- แทงโพย ---------------------------------------------------//
        [HttpPost]
        public ActionResult addPoll(PollData Data) 
        {
            var user_id = Session["ID"].ToString();
            var id = Int32.Parse(user_id);
            var username = Session["Username"].ToString();
            dynamic discount_rate;
            Period P = db.Period.Where(s => s.Status == "1").Where(x=> x.BetStatus=="1").FirstOrDefault<Period>();
            Discount D = db.Discount.Where(x => x.Account.ID == id).FirstOrDefault<Discount>(); //-----user-----id//
            if(D!=null)
            {
                discount_rate = D;
            }
            else
            {
                Main_Discount MD = db.Main_Discount.Where(x => 1 == 1).FirstOrDefault<Main_Discount>();
                discount_rate = MD;
            }
            if (P != null)
            {
                var poll = new Poll();
                poll.UID = id; //----------- user id-----------//
                poll.Period_ID = P.ID;
                poll.Receive = "1";
                poll.Create_By = username; //--------- username --------//
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
                    var amount = item.Amount.Replace("X", "x");
                    amount = item.Amount.Replace("*", "x");
                    var AmtCountX = amount.Split('x').Length - 1;
                    if (AmtCountX > 0)
                    {
                        //------------------------2 ตัว ----------------------------//
                        if (NumLen == 2)
                        {
                            char[] num = item.Number.ToCharArray();
                            string[] amt = amount.Split('x');
                            var totalDiscount = 0.00;
                            var iamt = 0.00;
                            var d = 0.00;
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
                                    InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString(), amt[0], totalDiscount, NumLen);

                                    //-------------------- 2 โต๊ด -------------------------------//
                                    iamt = Int32.Parse(amt[1]);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, num[1].ToString() + num[0].ToString(), amt[1], totalDiscount, NumLen);
                                }
                                else if (typ == "t")
                                {
                                    d = Int32.Parse(discount_rate.two_up);
                                    iamt = Int32.Parse(amt[0]);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    //---------------------- 2 เต็ง -------------------------------//
                                    InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString(), amt[0], totalDiscount, NumLen);


                                    //-------------------- 2 โต๊ด -------------------------------//
                                    iamt = Int32.Parse(amt[1]);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, num[1].ToString() + num[0].ToString(), amt[1], totalDiscount, NumLen);
                                }
                                else if (typ == "t_")
                                {
                                    d = Int32.Parse(discount_rate.two_ood);
                                    //-------------------- 2 โต๊ด -------------------------------//
                                    iamt = Int32.Parse(amt[1]);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    var sort = "";
                                    if (Int32.Parse(num[0].ToString()) < Int32.Parse(num[1].ToString()))
                                    {
                                        sort = num[0].ToString() + num[1].ToString();
                                    }
                                    else
                                    {
                                        sort = num[1].ToString() + num[0].ToString();
                                    }
                                    InsertLottoSub(lID, typ, sort, amt[1], totalDiscount, NumLen);
                                }
                                else { }
                            }
                            else if (item.bType == "tb")
                            {
                                //---------------- 2 บน ---------------------------//
                                d = Int32.Parse(discount_rate.two_up);
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", num[0].ToString() + num[1].ToString(), amt[0], totalDiscount, NumLen);

                                //------------------ 2 บน โต๊ด-----------------------//
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", num[1].ToString() + num[0].ToString(), amt[1], totalDiscount, NumLen);

                                //------------------- 2 ล่าง ---------------------------//
                                d = Int32.Parse(discount_rate.down);
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "b", num[0].ToString() + num[1].ToString(), amt[0], totalDiscount, NumLen);

                                //------------------ 2 ล่าง โต๊ด-----------------------//
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "b", num[1].ToString() + num[0].ToString(), amt[1], totalDiscount, NumLen);
                            }
                            else
                            {
                                return Json("Fail");
                            }
                        }
                        //---------------------------------3 ตัว------------------------------------//
                        else if (NumLen == 3)
                        {
                            char[] num = item.Number.ToCharArray();
                            string[] amt = amount.Split('x');
                            var totalDiscount = 0.00;
                            var iamt = 0.00;
                            var d = 0.00;
                            var typ = item.bType;
                            var sort = "";
                            var temp = 'l';
                            if (typ == "t")
                            {

                                //--------------------- เช็ค 3 ตัวโต๊ด (บ 434 = x100)-----------------------//
                                if (amt[0] == "")
                                {
                                    typ = "t_";
                                }
                                if (typ == "t")
                                {
                                    d = Int32.Parse(discount_rate.three_up);
                                    iamt = Int32.Parse(amt[0]);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    //---------------------- 3 เต็ง -------------------------------//
                                    InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString() + num[2].ToString(), amt[0], totalDiscount, NumLen);

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
                                    InsertLottoSub(lID, "t_", sort, amt[1], totalDiscount, NumLen);
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
                                    InsertLottoSub(lID, "t_", sort, amt[1], totalDiscount, NumLen);
                                }
                            }
                            else if (typ == "f")
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
                                    InsertLottoSub(lID, typ, num[0].ToString() + num[1].ToString() + num[2].ToString(), amt[0], totalDiscount, NumLen);

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
                                    InsertLottoSub(lID, "f_", sort, amt[1], totalDiscount, NumLen);
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
                                    InsertLottoSub(lID, "f_", sort, amt[1], totalDiscount, NumLen);
                                }
                            }
                            else if (typ == "ft")
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
                                    iamt = Int32.Parse(amt[0]) / 2;
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, "f", num[0].ToString() + num[1].ToString() + num[2].ToString(), iamt.ToString(), totalDiscount, NumLen);
                                    //---------------------- 3 บนเต็ง -------------------------------//
                                    d = Int32.Parse(discount_rate.three_up);
                                    iamt = Int32.Parse(amt[0]) / 2;
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, "t", num[0].ToString() + num[1].ToString() + num[2].ToString(), iamt.ToString(), totalDiscount, NumLen);
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
                                    InsertLottoSub(lID, "f_", sort, iamt.ToString(), totalDiscount, NumLen);

                                    //-------------------- 3 บนโต๊ด -------------------------------//
                                    d = Int32.Parse(discount_rate.three_ood);
                                    iamt = Int32.Parse(amt[1]) / 2;
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, "t_", sort, iamt.ToString(), totalDiscount, NumLen);
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
                                    InsertLottoSub(lID, "f_", sort, iamt.ToString(), totalDiscount, NumLen);

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
                                    InsertLottoSub(lID, "t_", sort, iamt.ToString(), totalDiscount, NumLen);
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
                            var totalDiscount = 0.00;
                            var iamt = 0.00;
                            var d = 0.00;
                            var typ = item.bType;
                            if (typ == "t" || typ == "b")
                            {
                                if (typ == "t")
                                {
                                    //------------------------ วิ่งบน -------------------------------//
                                    d = Int32.Parse(discount_rate.up);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount, NumLen);
                                }
                                else
                                {
                                    //---------------------- วิ่งล่าง -------------------------------//
                                    d = Int32.Parse(discount_rate.down);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount, NumLen);
                                }
                            }
                            else if (typ == "tb")
                            {
                                //------------------------ วิ่งบน -------------------------------//
                                d = Int32.Parse(discount_rate.up);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", item.Number, item.Amount, totalDiscount, NumLen);
                                //---------------------- วิ่งล่าง -------------------------------//
                                d = Int32.Parse(discount_rate.down);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "b", item.Number, item.Amount, totalDiscount, NumLen);
                            }
                            else { return Json("Fail"); }
                        }
                        else if (NumLen == 2)
                        {
                            var totalDiscount = 0.00;
                            var iamt = 0.00;
                            var d = 0.00;
                            var typ = item.bType;
                            if (typ == "t" || typ == "b")
                            {
                                if (typ == "t")
                                {
                                    //------------------------ 2 บน -------------------------------//
                                    d = Int32.Parse(discount_rate.two_up);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount, NumLen);

                                }
                                else
                                {
                                    d = Int32.Parse(discount_rate.two_down);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    //---------------------- 2 ล่าง -------------------------------//
                                    InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount, NumLen);
                                }
                            }
                            else if (typ == "tb")
                            {
                                //------------------------ 2 บน -------------------------------//
                                d = Int32.Parse(discount_rate.two_up);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", item.Number, item.Amount, totalDiscount, NumLen);

                                //---------------------- 2 ล่าง -------------------------------//
                                d = Int32.Parse(discount_rate.two_down);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "b", item.Number, item.Amount, totalDiscount, NumLen);

                            }
                            else { return Json("Fail"); }
                        }
                        else if (NumLen == 3)
                        {
                            var totalDiscount = 0.00;
                            var iamt = 0.00;
                            var d = 0.00;
                            var typ = item.bType;
                            if (typ == "t" || typ == "b" || typ == "f")
                            {
                                if (typ == "t")
                                {
                                    //------------------------ 3 บน -------------------------------//
                                    d = Int32.Parse(discount_rate.three_up);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount, NumLen);
                                }
                                else if (typ == "f")
                                {
                                    //------------------------ 3 หน้า -------------------------------//
                                    d = Int32.Parse(discount_rate.first_three);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount, NumLen);
                                }
                                else
                                {
                                    //---------------------- 3 ล่าง -------------------------------//
                                    d = Int32.Parse(discount_rate.three_down);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    InsertLottoSub(lID, typ, item.Number, item.Amount, totalDiscount, NumLen);
                                }
                            }
                            else if (typ == "tb")
                            {
                                //------------------------ 3 บน -------------------------------//
                                d = Int32.Parse(discount_rate.three_up);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", item.Number, item.Amount, totalDiscount, NumLen);

                                //---------------------- 3 ล่าง -------------------------------//
                                d = Int32.Parse(discount_rate.three_down);
                                iamt = Int32.Parse(item.Amount);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "b", item.Number, item.Amount, totalDiscount, NumLen);
                            }
                            else if (typ == "ft")
                            {
                                //------------------------ 3 บน -------------------------------//
                                d = Int32.Parse(discount_rate.two_up);
                                iamt = Int32.Parse(item.Amount) / 2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", item.Number, iamt.ToString(), totalDiscount, NumLen);
                                //---------------------- 3 หน้า -------------------------------//
                                d = Int32.Parse(discount_rate.two_down);
                                iamt = Int32.Parse(item.Amount) / 2;
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "f", item.Number, iamt.ToString(), totalDiscount, NumLen);
                            }
                            else { return Json("Fail"); }
                        }
                        else { return Json("Fail"); }
                    }
                }
            }
            else
            {
                return Json("timeout");
            }
            return Json("ss");
        }
        public void InsertLottoSub(int lid,string Type, string number, string amount,double totalDiscount,int NumLen)
        {
            var lottosub = new LottoSub();
            lottosub.Lotto_ID = lid;
            lottosub.Type = Type;
            lottosub.NumLen = NumLen.ToString();
            lottosub.Number = number;
            lottosub.Amount = amount;
            lottosub.AmountWin = "0";
            lottosub.Result_Status = "0";
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
                string query = "SELECT p.[ID],LM.Type,LM.Number,LM.Amount,sum(CAST(ls.Result_Status as int)) as Result_Status FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID],[Type],[Number],[Amount] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID  left join(SELECT [ID],[Lotto_ID],[Result_Status] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where p.ID=@pID group by p.ID,lm.ID,lm.Type,lm.Number,lm.Amount";
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
                            Result_Status = Reader["Result_Status"].ToString(),
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
        public ActionResult getResult(DateTime date)
        {
            Result R = db.Result.Where(s => s.Lotto_day == date).FirstOrDefault<Result>();
            return Json(R);
        }
        //---------------------------------------- เปลี่ยนรหัสผ่าน ---------------------------------------------------//
        [HttpPost]
        public ActionResult ChangePassword(string OldPass, string NewPass)
        {
            var id = Int32.Parse(Session["ID"].ToString());
            Account a = db.Account.Where(x => x.ID == id).FirstOrDefault<Account>();
            bool verify = VerifyHash(OldPass, a.Password);
            if (verify)
            {
                Account ac = db.Account.Where(r => r.ID == id).First();
                if (ac != null)
                {
                    ac.Password = ComputeHash(NewPass, null);
                    ac.update_date = DateTime.Now;
                    db.Entry(ac).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }
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
    }
}