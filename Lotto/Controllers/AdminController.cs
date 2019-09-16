using Lotto.Models;
using MaxMind.Db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
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
            if ((string)Session["Role"] == "Administrator" || (string)Session["Role"] == "SuperAdmin")
            {
                return View();
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult AllBet() //ดูเลขทั้งหมด
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                var all = new List<All_Number>();
                var max = db.Period.Where(y=>y.UID== parentID).Select(x => (int)x.ID).DefaultIfEmpty(0).Max();                
                if (max != 0)
                {
                    int id = db.Period.Where(x => x.UID == parentID).Max(p => p.ID);
                    string connetionString = null;                    
                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "select t1.Type, t1.Number,t1.Amount,t1.nLen,t2.max,t1.Result_Status from(SELECT ls.Type, ls.Number, ls.Result_Status, sum(CAST(LS.Amount as int)) as Amount, LEN(ls.Number) as nLen FROM[dbo].[Poll] p left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm on p.ID = lm.Poll_ID left join(SELECT[ID], [Lotto_ID], [Type], [Number], [Amount], Result_Status FROM[dbo].[LottoSub]) ls on lm.ID = ls.Lotto_ID where p.Receive = '1' and p.Period_ID = @period group by ls.Type, ls.Number, ls.Result_Status) t1 left join(select MAX(tt.num) as max from(select t.Type, t.nLen, COUNT(*) as num from(SELECT ls.Type, LEN(ls.Number) as nLen, ls.number FROM[dbo].[Poll] p left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm on p.ID = lm.Poll_ID left join(SELECT[ID], [Lotto_ID], [Type], [Number], [Amount] FROM[dbo].[LottoSub]) ls on lm.ID = ls.Lotto_ID where p.Receive = '1' and p.Period_ID = @period group by ls.Type, LEN(ls.Number), ls.number) t group by t.Type, t.nLen) tt) t2 on 1 = 1 order by t1.Type , t1.Number";
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
                                    Result = Reader["Result_Status"].ToString(),
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
                return View(all);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Close() //ปิดหวย
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                Period P = db.Period.Where(y=>y.UID== parentID).Where(x => x.Status == "1").FirstOrDefault<Period>();
                var pid = 0;
                if (P != null)
                {
                    pid = P.ID;
                }
                else
                {
                    var max = db.Period.Where(y => y.UID == parentID).Select(x => (int)x.ID).DefaultIfEmpty(0).Max();
                    if (max != 0)
                    {
                        int id = db.Period.Where(x => x.UID == parentID).Max(p => p.ID);
                        pid = id;
                    }
                    else
                    {
                        pid = 0;
                    }
                }
                string connetionString = null;
                var all = new List<Close>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT pe.ID,pe.Check_Result,pe.create_date,pe.update_date,pe.Date,case when pe.BetStatus='0' then 'ปิดรับ' else 'เปิดรับ' end as BetStatus,pe.Close_BY,sum(ISNULL(CAST(ls.Amount as int), 0 )) as Amount,ISNULL(cr.crR, 0 ) as crR,ISNULL(cu.countu , 0 ) as countu FROM [dbo].[Period] pe left join(SELECT [ID],Period_ID,Receive,update_date,create_date FROM [dbo].[Poll] where Receive='1') p on p.Period_ID=pe.ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on p.ID=lm.Poll_ID left join (SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID left join(SELECT Period_ID as crPID,COUNT(Receive) as crR,Receive as crRe FROM [dbo].[Poll] where Receive='1' group by Receive,Period_ID) cr on p.Period_ID=cr.crPID and p.Receive=cr.crRe left join(SELECT COUNT(distinct UID) as countu,Period_ID as cuPID,Receive as cuR FROM [dbo].[Poll] where Receive='1' group by Receive,Period_ID,Receive) cu on p.Period_ID=cu.cuPID and p.Receive=cu.cuR where pe.ID=@period group by pe.ID,pe.create_date,pe.Date,pe.Close_BY,pe.Check_Result,pe.update_date,cr.crR,cu.countu,pe.BetStatus";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@period", pid.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            all.Add(new Close
                            {
                                PID = Reader["ID"].ToString(),
                                Date = Convert.ToDateTime(Reader["Date"]),
                                Amount = Reader["Amount"].ToString(),
                                CountReceive = Reader["crR"].ToString(),
                                CountUser = Reader["countu"].ToString(),
                                CloseBy = Reader["Close_BY"].ToString(),
                                CreateDate = Reader["create_date"].ToString(),
                                CloseDate = Reader["update_date"].ToString(),
                                BetStatus = Reader["BetStatus"].ToString(),
                                CheckResult = Reader["Check_Result"].ToString()
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

                var user_bet = new List<User_Bet_Result>();
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT po.UID,a.Name,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as AmountDiscount,sum(CAST(ls.AmountWin as int)) as AmountWin FROM [dbo].[Period] pe " +
                                    "left join(SELECT [ID],[UID],[Period_ID],[Receive] FROM [dbo].[Poll] where Receive='1') po on pe.ID=po.Period_ID " +
                                    "left join(SELECT [ID],[Name] FROM [dbo].[Account]) a on po.UID=a.ID " +
                                    "left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[AmountDiscount],[AmountWin] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where pe.ID=@period group by po.UID,a.Name";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@period", pid.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            user_bet.Add(new User_Bet_Result
                            {
                                ID = Reader["UID"].ToString(),
                                Name = Reader["Name"].ToString(),
                                Discount = Reader["AmountDiscount"].ToString(),
                                Win = Reader["AmountWin"].ToString(),
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
                if (all.Count > 0)
                {
                    if (all[0].CheckResult == "1")
                    {
                        List<Total_Amount_Result> TAR = db.Total_Amount_Result.Where(x => x.Period_ID == pid.ToString()).ToList();
                        ViewData["TAR"] = TAR;
                        ViewData["UBET"] = user_bet;
                    }
                    else if (all[0].CheckResult == "0")
                    {
                        var total_type_bet = new List<Total_Amount_Result>();
                        try
                        {
                            SqlConnection cnn = new SqlConnection(connetionString);
                            cnn.Open();
                            string query = "select t.Type,ROUND(sum(CAST(t.AmountDiscount as float)),0) as AmountDiscount,sum(CAST(t.Win as int)) as Win from(SELECT CASE WHEN ls.Type='t' and ls.NumLen='1' THEN 'Up' WHEN ls.Type='b' and ls.NumLen='1' THEN 'Down' WHEN ls.Type='f' and ls.NumLen='3' THEN 'FirstThree' WHEN ls.Type='f_' and ls.NumLen='3' THEN 'FirstThreeOod' WHEN ls.Type='t' and ls.NumLen='3' THEN 'ThreeUp' WHEN ls.Type='t_' and ls.NumLen='3' THEN 'ThreeUPOod' WHEN ls.Type='b' and ls.NumLen='3' THEN 'ThreeDown' WHEN ls.Type='t' and ls.NumLen='2' THEN 'TwoUp' WHEN ls.Type='b' and ls.NumLen='2' THEN 'TwoDown' WHEN (ls.Type='t_' or ls.Type='b_') and ls.NumLen='2' THEN 'TwoOod' ELSE null END as Type ,ls.AmountDiscount,'0' as Win FROM [dbo].[Period] pe left join(SELECT [ID],[Period_ID],[Receive] FROM [dbo].[Poll] where Receive='1') po on pe.ID=po.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[Type],[NumLen],[AmountDiscount] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where pe.ID=@period)t group by t.Type";
                            SqlCommand cmd = new SqlCommand(query, cnn);
                            cmd.Parameters.AddWithValue("@period", pid.ToString());
                            SqlDataReader Reader = cmd.ExecuteReader();
                            Console.Write(Reader);
                            try
                            {
                                while (Reader.Read())
                                {
                                    total_type_bet.Add(new Total_Amount_Result
                                    {
                                        Type = Reader["Type"].ToString(),
                                        Amount_Discount = Reader["AmountDiscount"].ToString(),
                                        Amount_Win = Reader["Win"].ToString(),
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
                        ViewData["UBET"] = user_bet;
                    }
                    else { }
                }
                else
                {
                    all.Add(new Close
                    {
                        PID = pid.ToString(),
                        Date = DateTime.Now,
                        Amount = "0",
                        CountReceive = "0",
                        CountUser = "0",
                        CloseBy = "Admin",
                        CreateDate = DateTime.Now.ToString(),
                        CloseDate = DateTime.Now.ToString(),
                        BetStatus = "0",
                        CheckResult = "1"
                    });
                    List<Total_Amount_Result> TAR = new List<Total_Amount_Result>();
                    ViewData["TAR"] = TAR;
                    ViewData["UBET"] = user_bet;
                }                
                return View(all);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult List(String UID) //ดูโพย
        {
            if ((string)Session["Role"] == "Administrator")
            {
                string param;
                if (UID != null)
                {
                    param = "and p.UID = '" + UID + "'";
                }
                else
                {
                    param = "";
                }
                var parentID = Int32.Parse((string)Session["ParentID"]);
                var pollDetail = new List<Poll_Detail>();
                var max = db.Period.Where(y => y.UID == parentID).Select(x => (int)x.ID).DefaultIfEmpty(0).Max();
                if (max != 0)
                {
                    int id = db.Period.Where(x => x.UID == parentID).Max(p => p.ID);
                    string connetionString = null;
                    
                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "select t1.ID,t1.UID,t1.Poll_Name,t1.Name,t1.Receive,t1.Check_Status,t1.Create_By,t1.amount,t1.discount,t1.AmountWin,t1.Check_Result,t1.create_date,t1.IP,Row_Number() OVER (Partition BY t1.UID ORDER BY t1.UID) as rNumber"+
                                " from("+
                                " SELECT p.[ID],p.UID,p.Poll_Name,pe.Check_Result,a.Name,p.[Receive],p.[Check_Status],p.[Create_By],p.IP,sum(CAST(LS.Amount as int)) as amount,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as discount,sum(CAST(ls.AmountWin as int)) as AmountWin,p.create_date"+
                                    " FROM [dbo].Period pe"+
                                    " left join(SELECT [ID],[UID],[Poll_Name],[Period_ID],[Receive],[Check_Status],IP, create_date, Create_By"+
                                    " FROM[dbo].[Poll]) p on pe.ID = p.Period_ID"+
                                    " left join(SELECT [ID],[Poll_ID]"+
                                    " FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID"+
                                    " left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount],[AmountWin]"+
                                    " FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID"+
                                    " left join(SELECT [ID],[Name]"+
                                    " FROM [dbo].[Account]) a on p.UID =a.ID"+
                                    " where p.Period_ID=@period " + param + " group by p.ID,p.Poll_Name,p.IP,p.Receive,p.Check_Status,p.create_date,p.[Create_By],a.Name,p.UID,pe.Check_Result"+
                                " ) t1"+
                                " order by Row_Number() OVER (Partition BY t1.Name ORDER BY t1.Name) desc";
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
                                    Check_Status = Reader["Check_Status"].ToString(),
                                    Amount = Reader["amount"].ToString(),
                                    Discount = Reader["discount"].ToString(),
                                    Create_By = Reader["Create_By"].ToString(),
                                    create_date = Reader["create_date"].ToString(),
                                    poll_number = Reader["rNumber"].ToString(),
                                    Poll_Name = Reader["Poll_Name"].ToString(),
                                    UID = Reader["UID"].ToString(),
                                    Win = Reader["AmountWin"].ToString(),
                                    Status = Reader["Check_Result"].ToString(),
                                    IP = Reader["IP"].ToString(),
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
                    if (UID != null)
                    {
                        return Json(pollDetail);
                    }
                    else
                    {
                        return View(pollDetail);
                    }
                }
                return View(pollDetail);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Bet() //แทงโพย
        {
            if ((string)Session["Role"] == "Administrator")
            {
                string connetionString = null;
                var parentID = Int32.Parse((string)Session["ParentID"]);
                var data = new List<admin_bet>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT a.[ID],a.[Name],a.[Description],a.[Status],count(p.Period_ID) as poll_count FROM [dbo].[Account] a left join(SELECT [ID],[UID],[Period_ID] FROM [dbo].[Poll]) p on a.ID=p.UID where a.Name !='administrator' and a.Status = '1' and a.Delete_Status = '0' and a.Create_By_UID=@ParentID group by a.[ID],a.[Name],a.[Description],a.[Status] order by count(p.Period_ID) desc";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            data.Add(new admin_bet
                            {
                                UID = Reader["ID"].ToString(),
                                Name = Reader["Name"].ToString(),
                                Description = Reader["Description"].ToString(),
                                Status = Reader["Status"].ToString(),
                                poll_count = Reader["poll_count"].ToString(),
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
                return View(data);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult BetTotal() //ยอดสรุปเป็นใบ
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                var all = new List<total_bet>();
                var max = db.Period.Where(y => y.UID == parentID).Select(x => (int)x.ID).DefaultIfEmpty(0).Max();
                if (max != 0)
                {
                    int id = db.Period.Where(x => x.UID == parentID).Max(p => p.ID);
                    string connetionString = null;
                    
                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "SELECT ISNULL(po.UID,0) as UID,a.Name,ISNULL(r.poll_receive, 0 ) as poll_receive,ISNULL(rr.poll_reject, 0 ) as poll_reject,sum(CAST(LS.Amount as int)) as Amount,sum(CAST(LS.AmountWin as int)) as AmountWin,sum(CASE WHEN po.Receive ='0' THEN 0WHEN po.Receive = '1' THEN CAST(LS.Amount as int)END) as AmountReceive,ROUND(sum(CASE WHEN po.Receive ='0' THEN 0 WHEN po.Receive = '1' THEN CAST(LS.AmountDiscount as float) END),0) as AmountDiscount,sum(CASE WHEN ls.Result_Status = '0' THEN 0 WHEN ls.Result_Status = '1' THEN CAST(LS.Amount as int) END) as AmountBetWin FROM [dbo].[Period] p left join(SELECT [ID],[UID],[Period_ID],[Receive] FROM [dbo].[Poll]) po on p.ID=po.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[Type],[Number],[Amount],[AmountDiscount],AmountWin,Result_Status FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID left join(SELECT [ID],[Name] FROM [dbo].[Account] where Status='1') a on po.UID=a.ID left join(SELECT Period_ID,UID,count(*) as poll_receive FROM [dbo].[Poll] where [Receive]='1' group by [UID],[Period_ID],[Receive] ) r on p.ID=r.Period_ID and po.UID=r.UID left join(SELECT UID,Period_ID,count(*) as poll_reject FROM [dbo].[Poll] where [Receive]='0' group by [UID],[Period_ID],[Receive] ) rr on p.ID=rr.Period_ID and po.UID=rr.UID where p.id=@period group by po.UID,a.Name,r.poll_receive,rr.poll_reject";
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
                                    AmountWin = Reader["AmountWin"].ToString(),
                                    AmountBetWin= Reader["AmountBetWin"].ToString(),
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
                return View(all);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult CoDealer() //แทงออก
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var adminID = Int32.Parse((string)Session["ID"]);
                List<Account_Bet_Out> abo = db.Account_Bet_Out.Where(x => x.UID == adminID).ToList();
                return View(abo);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult OutList() //แทงออก
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var user_id = Int32.Parse( Session["ID"].ToString());
                var max = db.Period.Where(y => y.UID == user_id).Select(x => (int)x.ID).DefaultIfEmpty(0).Max();
                var data = new List<Poll_Out>();
                if (max != 0)
                {
                    int id = db.Period.Where(x => x.UID == user_id).Max(p => p.ID);
                    
                    string connetionString = null;

                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "SELECT pbo.[UID], pbo.[Send_To_UID],pbo.[Period_ID],pbo.[Status],sum(CAST(lbo.Amount as int)) as Amount,ROUND(sum(CAST(lbo.AmountDiscount as float)),0) as AmountDiscount,sum(CAST(lbo.[AmountWin]as int)) as AmountWin,count(*) as count,abo.Name FROM[dbo].[Poll_Bet_Out] pbo left join(SELECT[Poll_Out_ID],sum(CAST( [Amount]as int)) as Amount, ROUND(sum(CAST([AmountDiscount]as float)),0) as AmountDiscount, sum(CAST([AmountWin] as int)) as AmountWin FROM [dbo].[Lotto_Bet_Out] group by [Poll_Out_ID]) lbo on pbo.ID=lbo.Poll_Out_ID left join(SELECT [ID],[UID],[Name] FROM[dbo].[Account_Bet_Out]) abo on pbo.Send_To_UID = abo.ID where pbo.[Period_ID]=@period group by pbo.[UID],pbo.[Send_To_UID],pbo.[Period_ID],pbo.[Status],abo.Name";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@period", id.ToString());
                        SqlDataReader Reader = cmd.ExecuteReader();
                        Console.Write(Reader);
                        try
                        {
                            while (Reader.Read())
                            {
                                data.Add(new Poll_Out
                                {
                                    UID = Reader["UID"].ToString(),
                                    Send_To_UID = Reader["Send_To_UID"].ToString(),
                                    Period_ID = Reader["Period_ID"].ToString(),
                                    Status = Reader["Status"].ToString(),
                                    Count = Reader["count"].ToString(),
                                    Amount= Reader["Amount"].ToString(),
                                    AmountDiscount = Reader["AmountDiscount"].ToString(),
                                    AmountWin = Reader["AmountWin"].ToString(),
                                    Name = Reader["Name"].ToString(),
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
                }
                    return View(data);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Out() //แทงออก
        {
            if ((string)Session["Role"] == "Administrator")
            {
                return View();
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Cut() //ตัดเลข
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                var data = new List<Rate_Discount>();
                var all = new List<All_Number>();
                var max = db.Period.Where(y => y.UID == parentID).Select(x => (int)x.ID).DefaultIfEmpty(0).Max();
                if (max != 0)
                {
                    int id = db.Period.Where(x => x.UID == parentID).Max(p => p.ID);
                    string connetionString = null;
                    
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
                    
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "SELECT [three_up],[three_ood],[three_down],[two_up],[two_ood],[two_down],[up],[down],[first_three],[first_three_ood], md.three_up_discount,md.three_ood_discount,md.three_down_discount,md.two_up_discount,md.two_ood_discount,md.two_down_discount,md.up_discount,md.down_discount,md.first_three_discount,md.first_three_ood_discount FROM[dbo].[Main_Rate] mr left join(SELECT admin_id,[three_up] as three_up_discount,[three_ood] as three_ood_discount,[three_down] as three_down_discount,[two_up] as two_up_discount,[two_ood] as two_ood_discount,[two_down] as two_down_discount,[up] as up_discount,[down] as down_discount,[first_three] as first_three_discount,[first_three_ood] as first_three_ood_discount FROM[dbo].[Main_Discount]) md on md.admin_id = mr.admin_id where mr.admin_id = @ParentID ";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
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
                    ViewBag.Rate = data;
                    return View(all);
                }
                ViewBag.Rate = data;
                return View(all);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult OutTotal() //ยอดแทงออก
        {
            if ((string)Session["Role"] == "Administrator")
            {
                return View();
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult ListBackward(int? PID) //ดูโพยย้อนหลัง
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                List<Period> pr;
                if(PID != 0)
                {
                    Session["SelectPeriod"] = PID;
                    pr = db.Period.Where(x => x.ID == PID).ToList<Period>();

                }
                else
                {
                    Session["SelectPeriod"] = 0;
                    pr = db.Period.Where(x=>x.UID== parentID).ToList<Period>();
                }
                string connetionString = null;
                var pollDetail = new List<Poll_Detail>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;

                foreach (Period poo in pr)
                {
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "select t1.ID,t1.UID,t1.Poll_Name,t1.Period_ID,t1.Name,t1.Receive,t1.Create_By,t1.amount,t1.discount,t1.AmountWin,t1.create_date,Row_Number() OVER (Partition BY t1.UID ORDER BY t1.UID) as rNumber from(SELECT p.[ID],p.UID,p.Poll_Name,a.Name,p.[Receive],p.[Create_By],p.Period_ID,sum(CAST(LS.Amount as int)) as amount,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as discount,sum(CAST(LS.AmountWin as int)) as AmountWin,p.create_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID  left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount],AmountWin FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID  left join(SELECT [ID],[Name] FROM [dbo].[Account]) a on p.UID =a.ID where p.Period_ID=@period group by p.ID,p.Poll_Name,p.Receive,p.create_date,p.[Create_By],a.Name,p.UID,p.Period_ID) t1 order by Row_Number() OVER (Partition BY t1.Name ORDER BY t1.Name) desc";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@period", poo.ID.ToString());
                        SqlDataReader Reader = cmd.ExecuteReader();
                        Console.Write(Reader);
                        try
                        {
                            while (Reader.Read())
                            {
                                pollDetail.Add(new Poll_Detail
                                {
                                    ID = Reader["ID"].ToString(),
                                    PeriodDate = String.Format("{0:dd/MM/yyyy}", poo.Date),
                                    name = Reader["Name"].ToString(),
                                    Receive = Reader["Receive"].ToString(),
                                    Amount = Reader["amount"].ToString(),
                                    Discount = Reader["discount"].ToString(),
                                    Create_By = Reader["Create_By"].ToString(),
                                    create_date = Reader["create_date"].ToString(),
                                    poll_number = Reader["rNumber"].ToString(),
                                    Poll_Name = Reader["Poll_Name"].ToString(),
                                    UID = Reader["UID"].ToString(),
                                    Win = Reader["AmountWin"].ToString(),
                                    PeroidID = Reader["Period_ID"].ToString(),
                                }); ;
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
                }
                return View(pollDetail);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult LottoBackward() //ดูหวยย้อนหลัง
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                return View(db.Period.Where(y=>y.UID== parentID).OrderByDescending(x => x.ID).ToList());
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Result() //ดูเลขหวย
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                string connetionString = null;
                var r = new List<Result>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    var user_id = Session["ID"].ToString();
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT p.[ID],p.[UID],p.[Date] ,r.[Name] ,r.[Lotto_day],r.[two_down],r.[first_three],r.[last_three],r.[three_down_1],r.[three_down_2],r.[three_down_3],r.[three_down_4] FROM[dbo].[Period] p left join(SELECT[ID], [Period_ID], [Name], [Lotto_day] , [two_down] , [first_three], [last_three],[three_down_1] , [three_down_2] , [three_down_3], [three_down_4] FROM [dbo].[Result]) r on p.ID=r.Period_ID where r.Name is not null and p.[UID]=@ParentID order by r.Lotto_day desc";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@ParentID", parentID);
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            r.Add(new Result
                            {
                                ID = Int32.Parse(Reader["ID"].ToString()),
                                Name = Reader["Name"].ToString(),
                                Lotto_day = DateTime.Parse(Reader["Lotto_day"].ToString()),
                                two_down = Reader["two_down"].ToString(),
                                first_three = Reader["first_three"].ToString(),
                                last_three = Reader["last_three"].ToString(),
                                three_down_1 = Reader["three_down_1"].ToString(),
                                three_down_2 = Reader["three_down_2"].ToString(),
                                three_down_3 = Reader["three_down_3"].ToString(),
                                three_down_4 = Reader["three_down_4"].ToString(),
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
                return View(r);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Setting() //ตั้งค่า
        {
            if ((string)Session["Role"] == "Administrator")
            {
                return View();
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Member() //สมาชิก
        {
            if ((string)Session["Role"] == "Administrator" || (string)Session["Role"] == "SuperAdmin")
            {
                string connetionString = null;
                var user = new List<User_Role>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    SqlDataReader Reader = null;
                    cnn.Open();
                    string query = null; 
                    if((string)Session["Role"] == "Administrator"){
                        var parentID = Int32.Parse((string)Session["ParentID"]);
                        query = "SELECT a.[ID], a.[Username],a.[Name],a.[Description],a.[Status],a.[create_date],a.Create_By_UID, a.Last_Login,a.update_date,r.Role FROM[dbo].[Account] a left join(SELECT [ID],[UID],[Role_ID] FROM[dbo].[Account_Role]) ar on a.ID=ar.UID left join(SELECT[ID], [Role] FROM [dbo].[Role]) r on ar.Role_ID=r.ID where a.Status = '1' and a.Delete_Status = '0' and a.Create_By_UID=@ParentID";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
                        Reader = cmd.ExecuteReader();
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
                                    Role = Reader["Role"].ToString(),
                                    //Descript = Reader["Descript"].ToString()
                                });
                            }
                            cnn.Close();
                        }
                        catch
                        {

                        }
                    }
                    if((string)Session["Role"] == "SuperAdmin"){
                        query = "SELECT a.[ID], a.[Username],a.[Name],a.[Description],a.[Status],a.[create_date],a.Create_By_UID, a.Last_Login,a.update_date,r.Role,r.Descript FROM[dbo].[Account] a left join(SELECT TOP (1000) [ID],[UID],[Role_ID] FROM [dbo].[Account_Role]) ar on a.ID=ar.UID left join(SELECT [ID], [Role], [Descript] FROM [dbo].[Role]) r on ar.Role_ID=r.ID where r.Descript = 'admin'";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        Reader = cmd.ExecuteReader();
                    }
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
                                Role = Reader["Role"].ToString(),
                                Descript = Reader["Descript"].ToString()
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
                    if((string)Session["Role"] == "Administrator"){
                        for (int i = 0; i < user.Count; i++)
                        {
                            bool role = Check_Role("user", user[i].Role);
                            if (!role)
                            {
                                user.RemoveAt(i);
                            }
                        }
                    }
                }
                return View(user);
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult MemberPrice() //ราคาสมาชิก
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                string connetionString = null;
                var user = new List<User_Rate_Discount>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT a.[ID], a.[Username],a.[Name],r.Role ,ra.[ID] as rateID,ra.[three_up],ra.[three_ood],ra.[three_down],ra.[two_up],ra.[two_ood],ra.[two_down],ra.[up],ra.[down],ra.[first_three],ra.[first_three_ood],d.[ID] as discountID,d.[three_up] as three_up_d,d.[three_ood] as three_ood_d,d.[three_down] as three_down_d,d.[two_up] as two_up_d,d.[two_ood] as two_ood_d,d.[two_down] as two_down_d,d.[up] as up_d,d.[down] as down_d,d.[first_three] as first_three_d,d.[first_three_ood] as first_three_ood_d FROM[dbo].[Account] a left join(SELECT[ID],[UID],[Role_ID] FROM[dbo].[Account_Role]) ar on a.ID = ar.UID left join(SELECT[ID], [Role] FROM[dbo].[Role]) r on ar.Role_ID = r.ID left join(SELECT [ID], [UID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Rate]) ra on a.ID = ra.UID left join(SELECT [ID], [UID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Discount]) d on a.ID = d.UID where a.Status = '1' and a.Delete_Status = '0' and ra.UID is not null and a.[Create_By_UID]=@ParentID";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    //Console.Write(Reader);
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
                    string query = "SELECT a.[ID], a.[Username],a.[Name],r.Role ,ra.[three_up],ra.[three_ood],ra.[three_down],ra.[two_up],ra.[two_ood],ra.[two_down],ra.[up],ra.[down],ra.[first_three],ra.first_three_ood,d.[three_up] as three_up_d,d.[three_ood] as three_ood_d,d.[three_down] as three_down_d,d.[two_up] as two_up_d,d.[two_ood] as two_ood_d,d.[two_down] as two_down_d,d.[up] as up_d,d.[down] as down_d,d.[first_three] as first_three_d,d.first_three_ood as first_three_ood_d FROM[dbo].[Account] a left join(SELECT[ID],[UID],[Role_ID] FROM[dbo].[Account_Role]) ar on a.ID = ar.UID left join(SELECT[ID], [Role] FROM[dbo].[Role]) r on ar.Role_ID = r.ID left join(SELECT[ID],admin_id, [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Main_Rate]) ra on ra.admin_id = @ParentID left join(SELECT[ID],admin_id, [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three],[first_three_ood] FROM[dbo].[Main_Discount]) d on ra.admin_id = d.admin_id left join(SELECT[ID], [UID], [three_up], [three_ood], [three_down], [two_up], [two_ood], [two_down], [up], [down], [first_three] FROM[dbo].[Rate]) rate on a.ID = rate.UID where a.Status = '1' and a.Delete_Status = '0' and rate.UID is null and a.[Create_By_UID]=@ParentID";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    //Console.Write(Reader);
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
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult DealerInfo() //ข้อมูลเจ้ามือ
        {
            if ((string)Session["Role"] == "Administrator")
            {
                return View();
            }
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult AdminBet(int id) //แทงโพย
        {
            if ((string)Session["Role"] == "Administrator")
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                var max = db.Period.Where(y => y.UID == parentID).Select(x => (int)x.ID).DefaultIfEmpty(0).Max();
                
                if (max != 0)
                {
                    int maxpid = db.Period.Where(x => x.UID == parentID).Max(p => p.ID);
                    Period P = db.Period.Where(x => x.ID == maxpid).Where(y => y.Check_Result == "0").FirstOrDefault<Period>();
                    string connetionString = null;
                    var pollDetail = new List<Poll_Detail>();
                    connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                    try
                    {
                        SqlConnection cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        string query = "SELECT p.[ID],p.[Receive],sum(CAST(LS.Amount as int)) as amount,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as discount FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where p.Period_ID=@period and p.UID=@UID group by p.ID,p.Receive";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@UID", id.ToString()); // user ID
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
                    ViewBag.UID = id;
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
            else if ((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        //---------------------------------------------------------------------------------------------------//
        //-------------------------------------------function------------------------------------------------//
        //---------------------------------------------------------------------------------------------------//
        //---------------------------------------- อัพโหลดรูป ---------------------------------------------------//
        public JsonResult UploadIMG()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[12];
            var random = new Random();

            for (int j = 0; j < stringChars.Length; j++)
            {
                stringChars[j] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            for (int i = 0; i < Request.Files.Count; i++)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    //Use the following properties to get file's name, size and MIMEType
                    int fileSize = file.ContentLength;
                    //(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
                    string fileName = finalString + ".jpg";
                    //string fileName = Request.Form["PID"].ToString()+".jpg";
                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;
                    //To save file, use SaveAs method
                    file.SaveAs(Server.MapPath("~/PollIMG/") + fileName); //File will be saved in application root

                    var img = new Poll_Image();
                    img.Poll_ID = Request.Form["PID"].ToString(); //----------- user id-----------//
                    img.Name = fileName;
                    img.create_date = DateTime.Now;
                    img.update_date = DateTime.Now;
                    db.Poll_Image.Add(img);
                    db.SaveChanges();
                }
                catch
                {
                    return Json("fail");
                }
            }
            return Json("ss");
        }
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
            var uid = Int32.Parse((string)Session["ID"]);
            Rate R = db.Rate.Where(s => s.UID == uid).FirstOrDefault<Rate>();
            var data = new List<Rate_Discount>();
            if (R != null)
            {
                string connetionString = null;               
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "select [three_up],[three_ood],[three_down],[two_up],[two_ood],[two_down],[up],[down],[first_three],[first_three_ood], md.three_up_discount,md.three_ood_discount,md.three_down_discount,md.two_up_discount,md.two_ood_discount,md.two_down_discount,md.up_discount,md.down_discount,md.first_three_discount,md.first_three_ood_discount FROM[dbo].Rate mr join(SELECT[three_up] as three_up_discount,[three_ood] as three_ood_discount,[three_down] as three_down_discount,[two_up] as two_up_discount,[two_ood] as two_ood_discount,[two_down] as two_down_discount,[up] as up_discount,[down] as down_discount,[first_three] as first_three_discount,[first_three_ood] as first_three_ood_discount ,UID FROM[dbo].Discount) md on mr.UID = md.UID where mr.UID=@UID";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@UID", uid.ToString());
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
            }
            else
            {
                string connetionString = null;
                var parentID = Int32.Parse((string)Session["ParentID"]);
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT [three_up],[three_ood],[three_down],[two_up],[two_ood],[two_down],[up],[down],[first_three],[first_three_ood], md.three_up_discount,md.three_ood_discount,md.three_down_discount,md.two_up_discount,md.two_ood_discount,md.two_down_discount,md.up_discount,md.down_discount,md.first_three_discount,md.first_three_ood_discount FROM[dbo].[Main_Rate] mr join(SELECT admin_id,[three_up] as three_up_discount,[three_ood] as three_ood_discount,[three_down] as three_down_discount,[two_up] as two_up_discount,[two_ood] as two_ood_discount,[two_down] as two_down_discount,[up] as up_discount,[down] as down_discount,[first_three] as first_three_discount,[first_three_ood] as first_three_ood_discount FROM[dbo].[Main_Discount]) md on md.admin_id = mr.admin_id where mr.admin_id=@ParentID";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
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
            }
            return Json(data);
        }

        //-------------------------------------update rate and discount--------------------------------//
        [HttpPost]
        public ActionResult UpdateRateDiscount(Rate_Discount RateDiscountArr)
        {
            var parentID = (string)Session["ParentID"];
            Main_Rate MR = db.Main_Rate.Where(s => s.admin_id == parentID).FirstOrDefault<Main_Rate>();
            Main_Discount MD = db.Main_Discount.Where(s => s.admin_id == parentID).FirstOrDefault<Main_Discount>();
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
                Main_Rate R = new Main_Rate();
                R.update_date = DateTime.Now;
                R.admin_id = parentID;
                R.three_up = RateDiscountArr.ThreeUP;
                R.three_ood = RateDiscountArr.ThreeOod;
                R.three_down = RateDiscountArr.ThreeDown;
                R.two_up = RateDiscountArr.TwoUp;
                R.two_ood = RateDiscountArr.TwoOod;
                R.two_down = RateDiscountArr.TwoDown;
                R.up = RateDiscountArr.Up;
                R.down = RateDiscountArr.Down;
                R.first_three = RateDiscountArr.FirstThree;
                R.first_three_ood = RateDiscountArr.FirstThreeOod;
                db.Main_Rate.Add(R);
                db.SaveChanges();

                Main_Discount D = new Main_Discount();
                D.update_date = DateTime.Now;
                D.admin_id = parentID;
                D.three_up = RateDiscountArr.ThreeUP_discount;
                D.three_ood = RateDiscountArr.ThreeOod_discount;
                D.three_down = RateDiscountArr.ThreeDown_discount;
                D.two_up = RateDiscountArr.TwoUp_discount;
                D.two_ood = RateDiscountArr.TwoOod_discount;
                D.two_down = RateDiscountArr.TwoDown_discount;
                D.up = RateDiscountArr.Up_discount;
                D.down = RateDiscountArr.Down_discount;
                D.first_three = RateDiscountArr.FirstThree_discount;
                D.first_three_ood = RateDiscountArr.FirstThreeOod_discount;
                db.Main_Discount.Add(D);
                db.SaveChanges();
                return Json("ss");
            }
        }
        //-------------------------------------update user rate and discount--------------------------------//
        [HttpPost]
        public ActionResult UpdateUserRateDiscount(User_Rate_Discount User_Rate_Discount)
        {
            if (User_Rate_Discount.rateID != "null")
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
                Account u = db.Account.Where(s => s.Username == User.Username).FirstOrDefault<Account>();
                if(u==null)
                {
                    var A = new Account();
                    A.Username = User.Username;
                    A.Name = User.Name;
                    A.Password = ComputeHash(User.Password, null);
                    A.Status = User.Status;
                    A.Delete_Status = "0";
                    A.Description = User.Description;
                    A.Create_By_UID = Convert.ToString(Session["ID"]);
                    A.Last_Login = DateTime.Now;
                    A.create_date = DateTime.Now;
                    A.update_date = DateTime.Now;
                    db.Account.Add(A);
                    db.SaveChanges();
                    int AID = A.ID;
                    var R = new Account_Role();
                    var mr = new Main_Rate();
                    var md = new Main_Discount();
                    //int lastAccount = db.Account.Max(item => item.ID);
                    R.UID = AID;
                    if ((string)Session["Role"] == "SuperAdmin"){
                        R.Role_ID = 1;
                        var s = new Setting();
                        s.UID = AID;
                        s.auto_poll_accept = 0;
                        s.auto_close_time = "15:00";
                        s.create_date = DateTime.Now;
                        s.update_date = DateTime.Now;
                        db.Setting.Add(s);

                        mr.admin_id = AID.ToString();
                        mr.three_up = "550";
                        mr.three_ood = "100";
                        mr.three_down = "100";
                        mr.two_up = "65";
                        mr.two_ood = "65";
                        mr.two_down = "65";
                        mr.up = "3";
                        mr.down = "4";
                        mr.first_three = "550";
                        mr.first_three_ood = "100";
                        mr.create_date= DateTime.Now;
                        mr.update_date= DateTime.Now;
                        db.Main_Rate.Add(mr);

                        md.admin_id = AID.ToString();
                        md.three_up = "33";
                        md.three_ood = "33";
                        md.three_down = "33";
                        md.two_up = "33";
                        md.two_ood = "0";
                        md.two_down = "33";
                        md.up = "10";
                        md.down = "10";
                        md.first_three = "33";
                        md.first_three_ood = "33";
                        md.create_date= DateTime.Now;
                        md.update_date= DateTime.Now;
                        db.Main_Discount.Add(md);
                        db.SaveChanges();
                    }
                    else {
                        R.Role_ID = 2;
                    }
                    R.create_date = DateTime.Now;
                    R.update_date = DateTime.Now;
                    db.Account_Role.Add(R);
                    db.SaveChanges();

                    return Json("ss");
                }
                else
                {
                    return Json("dup");
                }
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
            if (A != null)
            {   
                if((string)Session["Role"] == "SuperAdmin"){
                    Account_Role AR = db.Account_Role.Where(s => s.UID == A.ID).FirstOrDefault<Account_Role>();
                    db.Account_Role.Remove(AR);
                    db.Account.Remove(A);
                    db.SaveChanges();
                    return Json("ss");
                }
                else{
                    A.update_date = DateTime.Now;
                    A.Status = "0";
                    A.Delete_Status = "1";
                    db.Entry(A).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("ss");
                }
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Add CoDealer data --------------------------------//
        [HttpPost]
        public ActionResult AddCoDealer(Account_Bet_Out CoDealer)
        {           
            if (CoDealer != null)
            {
                Account_Bet_Out A = new Account_Bet_Out();
                A.UID = CoDealer.UID;
                A.Name = CoDealer.Name;
                A.SendToUsername = CoDealer.SendToUsername;
                A.Username = CoDealer.Username;
                A.Password = CoDealer.Password;
                db. Account_Bet_Out.Add(A);
                db.SaveChanges();

                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Update CoDealer data --------------------------------//
        [HttpPost]
        public ActionResult UpdateCoDealer(Account_Bet_Out CoDealer)
        {
            Account_Bet_Out A = db.Account_Bet_Out.Where(s => s.ID == CoDealer.ID).FirstOrDefault<Account_Bet_Out>();
            if (A != null)
            {
                A.Name = CoDealer.Name;
                A.SendToUsername = CoDealer.SendToUsername;
                A.Username = CoDealer.Username;
                A.Password = CoDealer.Password;
                db.Entry(A).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        //-------------------------------------Delete CoDealer data --------------------------------//
        [HttpPost]
        public ActionResult DeleteCoDealer(Account_Bet_Out CoDealer)
        {
            Account_Bet_Out A = db.Account_Bet_Out.Where(s => s.ID == CoDealer.ID).FirstOrDefault<Account_Bet_Out>();
            if (A != null)
            {   
                db.Account_Bet_Out.Remove(A);
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
            var parentID = Int32.Parse((string)Session["ParentID"]);
            Period current = db.Period.Where(s => s.Status == "1").Where(x=>x.UID==parentID).SingleOrDefault();
            if (current != null)
            {
                return Json(new { Date = current.Date.Value.ToString("dd/MM/yyyy"), Status = current.Status });
            }
            try
            {
                int id = db.Period.Where(y => y.UID == parentID).Max(p => p.ID);
                current = db.Period.Where(s => s.ID == id).First();
            }
            catch
            {
                return Json(new { Date = "--/--/----", Status = 0 });
            }
            
            return Json(new { Date = current.Date.Value.ToString("dd/MM/yyyy"), Status = current.Status });
        }

        //-------------------------------------Get Setting --------------------------------//
        [HttpPost]
        public ActionResult GetSetting()
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            return Json(db.Setting.Where(x=>x.UID== parentID), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAllPeriod()
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            System.Collections.ArrayList period = new System.Collections.ArrayList();
            string connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT ID, Date FROM [dbo].[Period] where UID=@ParentID";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {

                        period.Add(new {
                            ID = Reader["ID"].ToString(),
                            Date = Reader["Date"].ToString()
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
            return Json(new { Period = period }, JsonRequestBehavior.AllowGet);
        }

        //-------------------------------------Update Setting --------------------------------//
        [HttpPost]
        public ActionResult UpdateSetting(int Accept, string Close)
        {
            int u = Int32.Parse((string)Session["ID"]);
            Setting s = db.Setting.Where(x => x.UID == u).First();
            if (s != null)
            {   
                s.auto_poll_accept = Accept;
                s.auto_close_time = Close;
                s.update_date = DateTime.Now;
                db.Entry(s).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                Session["auto_poll_accept"] = s.auto_poll_accept;
                Session["auto_close_time"] = s.auto_close_time;
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }

        [HttpPost]
        public ActionResult UpdateInfo(string Tel, string Line)
        {
            int u = Int32.Parse((string)Session["ID"]);
            Setting s = db.Setting.Where(x => x.UID == u).First();
            if (s != null)
            {  
                s.dealer_phone = Tel;
                s.dealer_line_id = Line;
                s.update_date = DateTime.Now;
                db.Entry(s).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                Session["dealer_phone"] = s.dealer_phone;
                Session["dealer_line_id"] = s.dealer_line_id;
                return Json("ss");
            }
            else
            {
                return Json("fail");
            }
        }
        //---------------------------------- get poll header detail Admin/bet------------------------------------------//
        [HttpPost]
        public ActionResult getPoll(string UID ,string PID)
        {
            int id = 0;
            if (PID!=null)
            {
                id = Int32.Parse(PID);
            }
            else
            {
                var parentID = Int32.Parse((string)Session["ParentID"]);
                id = db.Period.Where(x=>x.UID== parentID).Max(p => p.ID);
            }            
            if (id != 0)
            {
                string connetionString = null;
                var pollDetail = new List<Poll_Detail>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT p.[ID],p.[Receive],p.[Create_By],sum(CAST(LS.Amount as int)) as amount,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as discount,p.create_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID where p.Period_ID=@period and p.UID=@UID group by p.ID,p.Receive,p.create_date,p.[Create_By]";
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

        //------------------------------------- Reject poll --------------------------------//
        [HttpPost]
        public ActionResult ConfirmCheckPoll(string PollID)
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
                P.Check_Status = "1";
                db.Entry(P).State = EntityState.Modified;
                db.SaveChanges();
                return Json("ss");
            }
        }

        [HttpPost]
        public ActionResult addPoll(String PollName,string IPAddress, PollData Data, string UID, string PID)
        {
            dynamic discount_rate;
            var user_id = Session["ID"].ToString();
            var id = Int32.Parse(user_id);
            var returnPollID = 0;
            var parentID = Int32.Parse((string)Session["ParentID"]);
            var strparentID = (string)Session["ParentID"];
            Period P = db.Period.Where(x=>x.UID== parentID).Where(s => s.Status == "1").FirstOrDefault<Period>();
            Discount D = db.Discount.Where(x => x.Account.ID == id).FirstOrDefault<Discount>(); //-----user-----id//
            if (D != null)
            {
                discount_rate = D;
            }
            else
            {
                Main_Discount MD = db.Main_Discount.Where(y=>y.admin_id== strparentID).Where(x => 1 == 1).FirstOrDefault<Main_Discount>();
                discount_rate = MD;
            }

            int uid = Int32.Parse(UID);
            int pollID;

            if (PID != "")
            {
                pollID = Int32.Parse(PID);
                if (PollName != "")
                {
                    Poll poll = db.Poll.Find(pollID);
                    if (poll != null)
                    {
                        poll.update_date = DateTime.Now;
                        poll.Poll_Name = PollName;
                        db.Entry(poll).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                List<LottoMain> main = db.LottoMain.Where(m => m.Poll_ID == pollID).ToList<LottoMain>();
                foreach (LottoMain moo in main)
                {
                    db.LottoSub.RemoveRange(db.LottoSub.Where(x => x.Lotto_ID == moo.ID));
                    //db.SaveChanges();
                }
                db.LottoMain.RemoveRange(db.LottoMain.Where(m => m.Poll_ID == pollID));
                db.SaveChanges();
            }
            else
            {
                //Period Last = db.Period.OrderByDescending(u => u.ID).FirstOrDefault();
                var poll = new Poll();
                poll.UID = uid; //----------- user id-----------//
                poll.Poll_Name = PollName;
                poll.Period_ID = P.ID;
                poll.Receive = "1";
                poll.Create_By = "Admin"; //--------- Admin --------//
                poll.IP = IPAddress;
                poll.Check_Status = "0";
                poll.create_date = DateTime.Now;
                poll.update_date = DateTime.Now;
                db.Poll.Add(poll);
                db.SaveChanges();

                pollID = poll.ID;
                returnPollID = pollID;
            }

            foreach (var item in Data.poll)
            {
                var lotto = new LottoMain();
                lotto.Poll_ID = pollID;
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
                var numx = item.Number.Replace("X", "x");
                numx = item.Number.Replace("*", "x");
                var NumCountX = numx.Split('x').Length - 1;
                var AmtCountX = amount.Split('x').Length - 1;
                var AmtCountSlash = amount.Split('/').Length - 1;
                if (AmtCountX > 0 && AmtCountSlash > 0)
                {
                    char[] num = item.Number.ToCharArray();
                    string[] amt = amount.Split(new char[] { 'x', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var totalDiscount = 0.00;
                    var iamt = 0.00;
                    var d = 0.00;
                    var typ = item.bType;
                    var sort = "";
                    var temp = 'l';


                    d = Int32.Parse(discount_rate.three_up);
                    iamt = Int32.Parse(amt[0]);
                    totalDiscount = (iamt - (iamt * d) / 100);
                    //---------------------- 3 เต็ง -------------------------------//
                    InsertLottoSub(lID, "t", item.Number, amt[0], totalDiscount, NumLen);

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
                    //----------------------3ล่าง--------------------------------------//
                    d = Int32.Parse(discount_rate.three_down);
                    iamt = Int32.Parse(amt[2]);
                    totalDiscount = (iamt - (iamt * d) / 100);
                    InsertLottoSub(lID, "b", item.Number, amt[2], totalDiscount, NumLen);
                }
                else if (AmtCountSlash > 0)
                {
                    char[] num = item.Number.ToCharArray();
                    string[] amt = amount.Split('/');
                    var totalDiscount = 0.00;
                    var iamt = 0.00;
                    var d = 0.00;
                    var typ = item.bType;
                    //----------------------2 ตัว-------------------------//
                    if (NumLen == 2)
                    {
                        //------------------------ 2 บน -------------------------------//
                        d = Int32.Parse(discount_rate.two_up);
                        iamt = Int32.Parse(amt[0]);
                        totalDiscount = (iamt - (iamt * d) / 100);
                        InsertLottoSub(lID, "t", item.Number, amt[0], totalDiscount, NumLen);

                        //---------------------- 2 ล่าง -------------------------------//
                        d = Int32.Parse(discount_rate.two_down);
                        iamt = Int32.Parse(amt[1]);
                        totalDiscount = (iamt - (iamt * d) / 100);
                        InsertLottoSub(lID, "b", item.Number, amt[1], totalDiscount, NumLen);
                    }
                    //----------------------3 ตัว-------------------------//
                    else
                    {
                        if (typ == "tb")
                        {
                            //------------------------ 3 บน -------------------------------//
                            d = Int32.Parse(discount_rate.three_up);
                            iamt = Int32.Parse(amt[0]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", item.Number, amt[0], totalDiscount, NumLen);

                            //---------------------- 3 ล่าง -------------------------------//
                            d = Int32.Parse(discount_rate.three_down);
                            iamt = Int32.Parse(amt[1]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "b", item.Number, amt[1], totalDiscount, NumLen);
                        }
                        else if (typ == "ft")
                        {
                            //------------------------ 3 บน -------------------------------//
                            d = Int32.Parse(discount_rate.three_up);
                            iamt = Int32.Parse(amt[1]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", item.Number, amt[1], totalDiscount, NumLen);
                            //---------------------- 3 หน้า -------------------------------//
                            d = Int32.Parse(discount_rate.first_three);
                            iamt = Int32.Parse(amt[0]);
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "f", item.Number, amt[0], totalDiscount, NumLen);
                        }
                        else
                        {
                            return Json("Fail");
                        }
                    }
                }
                else if (AmtCountX > 0)
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
                            d = Int32.Parse(discount_rate.two_down);
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
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "f", num[0].ToString() + num[1].ToString() + num[2].ToString(), iamt.ToString(), totalDiscount, NumLen);
                                //---------------------- 3 บนเต็ง -------------------------------//
                                d = Int32.Parse(discount_rate.three_up);
                                iamt = Int32.Parse(amt[0]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t", num[0].ToString() + num[1].ToString() + num[2].ToString(), iamt.ToString(), totalDiscount, NumLen);
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
                                InsertLottoSub(lID, "f_", sort, iamt.ToString(), totalDiscount, NumLen);

                                //-------------------- 3 บนโต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.three_ood);
                                iamt = Int32.Parse(amt[1]);
                                totalDiscount = (iamt - (iamt * d) / 100);
                                InsertLottoSub(lID, "t_", sort, iamt.ToString(), totalDiscount, NumLen);
                            }
                            else
                            {
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
                                InsertLottoSub(lID, "f_", sort, iamt.ToString(), totalDiscount, NumLen);

                                //-------------------- 3 บนโต๊ด -------------------------------//
                                d = Int32.Parse(discount_rate.three_ood);
                                iamt = Int32.Parse(amt[1]) ;
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
                            d = Int32.Parse(discount_rate.three_up);
                            iamt = Int32.Parse(item.Amount) ;
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "t", item.Number, iamt.ToString(), totalDiscount, NumLen);
                            //---------------------- 3 หน้า -------------------------------//
                            d = Int32.Parse(discount_rate.first_three);
                            iamt = Int32.Parse(item.Amount) ;
                            totalDiscount = (iamt - (iamt * d) / 100);
                            InsertLottoSub(lID, "f", item.Number, iamt.ToString(), totalDiscount, NumLen);
                        }
                        else { return Json("Fail"); }
                    }
                    else if (NumLen == 4)
                    {
                        if (NumCountX > 0)
                        {
                            char[] num = item.Number.ToCharArray();
                            var totalDiscount = 0.00;
                            var iamt = 0.00;
                            var d = 0.00;
                            var typ = item.bType;
                            var swapNumber = "";
                            var len = 3;
                            if (typ == "t" || typ == "b" || typ == "f")
                            {
                                if (num[0] == num[1] && num[0] == num[2])
                                {
                                    if (typ == "t")
                                    {
                                        //------------------------ 3 บน -------------------------------//
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else if (typ == "f")
                                    {
                                        //------------------------ 3 หน้า -------------------------------//
                                        d = Int32.Parse(discount_rate.first_three);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else
                                    {
                                        //---------------------- 3 ล่าง -------------------------------//
                                        d = Int32.Parse(discount_rate.three_down);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                    }
                                }
                                else if (num[0] != num[1] && num[0] != num[2] && num[1] != num[2])
                                {
                                    if (typ == "t")
                                    {
                                        //------------------------ 3 บน -------------------------------//
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[1].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);

                                    }
                                    else if (typ == "f")
                                    {
                                        //------------------------ 3 หน้า -------------------------------//
                                        d = Int32.Parse(discount_rate.first_three);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[1].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else
                                    {
                                        //---------------------- 3 ล่าง -------------------------------//
                                        d = Int32.Parse(discount_rate.three_down);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[1].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                    }
                                }
                                else if (num[0] == num[1] || num[0] == num[2] || num[1] == num[2])
                                {
                                    if (typ == "t")
                                    {
                                        //------------------------ 3 บน -------------------------------//
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        if (num[0] == num[1])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else if (num[0] == num[2])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else if (num[1] == num[2])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else { }
                                    }
                                    else if (typ == "f")
                                    {
                                        //------------------------ 3 หน้า -------------------------------//
                                        d = Int32.Parse(discount_rate.first_three);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        if (num[0] == num[1])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else if (num[0] == num[2])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else if (num[1] == num[2])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else { }
                                    }
                                    else
                                    {
                                        //---------------------- 3 ล่าง -------------------------------//
                                        d = Int32.Parse(discount_rate.three_down);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        if (num[0] == num[1])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else if (num[0] == num[2])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else if (num[1] == num[2])
                                        {
                                            swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                            swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                            InsertLottoSub(lID, typ, swapNumber, item.Amount, totalDiscount, len);
                                        }
                                        else { }
                                    }
                                }
                                else { return Json("Fail"); }
                            }
                            else if (typ == "tb")
                            {
                                if (num[0] == num[1] && num[0] == num[2])
                                {
                                    //------------------------ 3 บน -------------------------------//
                                    d = Int32.Parse(discount_rate.three_up);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                    //---------------------- 3 ล่าง -------------------------------//
                                    d = Int32.Parse(discount_rate.three_down);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                }
                                else if (num[0] != num[1] && num[0] != num[2] && num[1] != num[2])
                                {
                                    d = Int32.Parse(discount_rate.three_up);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[1].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                    d = Int32.Parse(discount_rate.three_down);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[1].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                }
                                else if (num[0] == num[1] || num[0] == num[2] || num[1] == num[2])
                                {
                                    if (num[0] == num[1])
                                    {
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                        d = Int32.Parse(discount_rate.three_down);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else if (num[0] == num[2])
                                    {
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                        d = Int32.Parse(discount_rate.three_down);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else if (num[1] == num[2])
                                    {
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                        d = Int32.Parse(discount_rate.three_down);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, "b", swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else { }
                                }
                                else { return Json("Fail"); }
                            }
                            else if (typ == "ft")
                            {
                                if (num[0] == num[1] && num[0] == num[2])
                                {
                                    //------------------------ 3 บน -------------------------------//
                                    d = Int32.Parse(discount_rate.three_up);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                    //---------------------- 3 ล่าง -------------------------------//
                                    d = Int32.Parse(discount_rate.first_three);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                }
                                else if (num[0] != num[1] && num[0] != num[2] && num[1] != num[2])
                                {
                                    d = Int32.Parse(discount_rate.three_up);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[1].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                    d = Int32.Parse(discount_rate.first_three);
                                    iamt = Int32.Parse(item.Amount);
                                    totalDiscount = (iamt - (iamt * d) / 100);
                                    swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                    InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                    InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    swapNumber = num[2].ToString() + num[1].ToString() + num[0].ToString();
                                    InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                }
                                else if (num[0] == num[1] || num[0] == num[2] || num[1] == num[2])
                                {
                                    if (num[0] == num[1])
                                    {
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                        d = Int32.Parse(discount_rate.first_three);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[2].ToString() + num[0].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else if (num[0] == num[2])
                                    {
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                        d = Int32.Parse(discount_rate.first_three);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[0].ToString() + num[2].ToString() + num[1].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else if (num[1] == num[2])
                                    {
                                        d = Int32.Parse(discount_rate.three_up);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, "t", swapNumber, item.Amount, totalDiscount, len);

                                        d = Int32.Parse(discount_rate.first_three);
                                        iamt = Int32.Parse(item.Amount);
                                        totalDiscount = (iamt - (iamt * d) / 100);
                                        swapNumber = num[0].ToString() + num[1].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[0].ToString() + num[2].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                        swapNumber = num[1].ToString() + num[2].ToString() + num[0].ToString();
                                        InsertLottoSub(lID, "f", swapNumber, item.Amount, totalDiscount, len);
                                    }
                                    else { }
                                }
                                else { return Json("Fail"); }
                            }
                            else { return Json("Fail"); }
                        }
                    }
                    else { return Json("Fail"); }
                }
            }
            db.SaveChanges();
            return Json(new { PollID = returnPollID });
        }
        public void InsertLottoSub(int lid, string Type, string number, string amount, double totalDiscount, int NumLen)
        {
            var lottosub = new LottoSub();
            lottosub.Lotto_ID = lid;
            lottosub.Type = Type;
            lottosub.NumLen = NumLen.ToString();
            lottosub.Number = number;
            lottosub.Amount = amount;
            lottosub.AmountDiscount = totalDiscount.ToString();
            lottosub.AmountWin = "0";
            lottosub.Result_Status = "0";
            lottosub.create_date = DateTime.Now;
            lottosub.update_date = DateTime.Now;
            db.LottoSub.Add(lottosub);
            //db.SaveChanges();
        }
        [HttpPost]
        public ActionResult addPeroid(DateTime Date)
        {
            var pDate = Date.ToString("yyyy-MM-dd");
            //List<Period> oldPeroid = db.Period.Where(s => s.Status == "1").ToList();
            //oldPeroid.ForEach(a => a.Status = "0");
            //db.Entry(oldPeroid).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();
            var ID = Int32.Parse((string)Session["ID"]);
            var p = new Period();
            p.UID = ID; //----------- admin id-----------//
            p.Date = Date;
            p.Status = "1";
            p.BetStatus = "0";
            p.Check_Result = "0";
            p.Close_BY = "";
            p.create_date = DateTime.Now;
            db.Period.Add(p);
            db.SaveChanges();
            return Json("ss");
        }

        [HttpPost]
        public ActionResult UpdateBetStatus(string Status, string PID)
        {
            int id = Int32.Parse(PID);
            if (Status == "close")
            {
                Status = "0";
            }
            else if (Status == "open")
            {
                Status = "1";
            }
            else { }
            Period p = db.Period.Where(s => s.ID == id).FirstOrDefault<Period>();
            p.BetStatus = Status;
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json("ss");
        }

        [HttpPost]
        public ActionResult CloseLotto(string PID)
        {
            int id = Int32.Parse(PID);
            Period p = db.Period.Where(s => s.ID == id).FirstOrDefault<Period>();
            p.BetStatus = "0";
            p.Status = "0";
            p.Close_BY = "Admin";
            p.update_date = DateTime.Now;
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json("ss");
        }
        [HttpPost]
        public ActionResult CheckResult(string PID, DateTime DAY, string FT, string FTO1, string FTO2, string FTO3, string FTO4, string FTO5, string TU, string TUO1, string TUO2, string TUO3, string TUO4, string TUO5, string TD, string TDO1, string ThDown1, string ThDown2, string ThDown3, string ThDown4, string TwUP, string Tw_up_ood, string UP, string DOWN)
        {
            int perID = Int32.Parse(PID);
            Period pe = db.Period.Where(s => s.Check_Result == "1").Where(x => x.ID == perID).FirstOrDefault<Period>();
            if (pe != null)
            {
                string connetionString1 = null;
                var update = new List<LottoSub>();
                connetionString1 = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString1);
                    cnn.Open();
                    string query = "SELECT ls.ID FROM [dbo].[Period] pe left join(SELECT [ID],[Period_ID] FROM [dbo].[Poll] where Receive='1') po on pe.ID=po.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where pe.ID=@period";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@period", PID.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            update.Add(new LottoSub
                            {
                                ID = Int32.Parse(Reader["ID"].ToString()),
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
                foreach (var item in update)
                {
                    LottoSub ls = db.LottoSub.Where(s => s.ID == item.ID).FirstOrDefault<LottoSub>();
                    ls.AmountWin = "0";
                    ls.Result_Status = "0";
                    ls.update_date = DateTime.Now;
                    db.Entry(ls).State = System.Data.Entity.EntityState.Modified;
                    
                }
                db.SaveChanges();
                var tar = db.Total_Amount_Result.Where(q => q.Period_ID == PID).ToList();
                foreach (var item in tar)
                {
                    db.Total_Amount_Result.Remove(item);
                }
                var re = db.Result.Where(q => q.Period_ID == perID).ToList();
                foreach (var item in re)
                {
                    db.Result.Remove(item);
                }
            }
            string connetionString = null;
            var data = new List<Poll_Result>();
            var parentID = Int32.Parse((string)Session["ParentID"]);
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT ls.[ID],ls.Type,ls.NumLen,ls.Number,ls.Amount,ls.AmountDiscount,isnull(r.[three_up],mr.three_up) as three_up,isnull(r.[three_ood],mr.[three_ood]) as three_ood,isnull(r.[three_down],mr.[three_down]) as three_down,isnull(r.[two_up],mr.[two_up]) as two_up,isnull(r.[two_ood],mr.[two_ood]) as two_ood,isnull(r.[two_down],mr.[two_down]) as two_down,isnull(r.[up],mr.[up]) as up,isnull(r.[down],mr.[down]) as down,isnull(r.[first_three],mr.[first_three]) as first_three,isnull(r.[first_three_ood],mr.[first_three_ood]) as first_three_ood FROM [dbo].[Period] pe left join(SELECT [ID],UID,[Period_ID],[Receive] FROM [dbo].[Poll] where Receive='1') po on pe.ID=po.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[Type],NumLen,[Number],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID left join(SELECT [ID],[UID],[three_up],[three_ood],[three_down],[two_up],[two_ood],[two_down],[up],[down],[first_three],[first_three_ood] FROM [dbo].[Rate]) r on po.UID=r.UID left join(SELECT [ID],admin_id,[three_up],[three_ood],[three_down],[two_up],[two_ood],[two_down],[up],[down],[first_three],[first_three_ood] FROM [dbo].[Main_Rate]) mr on mr.admin_id=@ParentID where pe.ID=@period";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@period", PID.ToString());
                cmd.Parameters.AddWithValue("@ParentID", parentID.ToString());
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        data.Add(new Poll_Result
                        {
                            ID = Reader["ID"].ToString(),
                            Type = Reader["Type"].ToString(),
                            NumLen = Reader["NumLen"].ToString(),
                            Number = Reader["Number"].ToString(),
                            Amount = Reader["Amount"].ToString(),
                            AmountDiscount = Reader["AmountDiscount"].ToString(),
                            three_up = Reader["three_up"].ToString(),
                            three_ood = Reader["three_ood"].ToString(),
                            three_down = Reader["three_down"].ToString(),
                            two_up = Reader["two_up"].ToString(),
                            two_ood = Reader["two_ood"].ToString(),
                            two_down = Reader["two_down"].ToString(),
                            up = Reader["up"].ToString(),
                            down = Reader["down"].ToString(),
                            first_three = Reader["first_three"].ToString(),
                            first_three_ood = Reader["first_three_ood"].ToString(),
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
            var list = new List<totalResult>
            {
                new totalResult { Type = "Up", AmountDiscount = 0.00, Win = 0,total=0 },
                new totalResult { Type = "Down", AmountDiscount = 0.00, Win = 0 ,total=0},
                new totalResult { Type = "FirstThree", AmountDiscount = 0.00, Win = 0 ,total=0},
                new totalResult { Type = "FirstThreeOod", AmountDiscount = 0.00, Win = 0 ,total=0},
                new totalResult { Type = "ThreeUp", AmountDiscount = 0.00, Win = 0 ,total=0},
                new totalResult { Type = "ThreeUPOod", AmountDiscount = 0.00, Win =0 ,total=0},
                new totalResult { Type = "ThreeDown", AmountDiscount = 0.00, Win = 0 ,total=0},
                new totalResult { Type = "TwoUp", AmountDiscount = 0.00, Win = 0 ,total=0},
                new totalResult { Type = "TwoDown", AmountDiscount = 0.00, Win = 0 ,total=0},
                new totalResult { Type = "TwoOod", AmountDiscount = 0.00, Win = 0 ,total=0}
           };
            foreach (var idata in data)
            {
                if (idata.ID != "")
                {
                    int amount_ = Int32.Parse(idata.Amount);
                    float amountDis = float.Parse(idata.AmountDiscount, CultureInfo.InvariantCulture);
                    //int amountDis = Int32.Parse(idata.AmountDiscount);
                    if (idata.NumLen == "1")
                    {
                        if (idata.Type == "t" && idata.Number == UP)
                        {
                            UpdateLottoSub(idata.Amount, idata.up, idata.ID);
                            int rate = Int32.Parse(idata.up);
                            list[0].Win = list[0].Win + (amount_ * rate);
                            list[0].AmountDiscount = list[0].AmountDiscount + amountDis;
                            list[0].total = Convert.ToInt32(list[0].AmountDiscount) + list[0].Win;
                        }
                        else if (idata.Type == "b" && idata.Number == DOWN)
                        {
                            UpdateLottoSub(idata.Amount, idata.down, idata.ID);
                            int rate = Int32.Parse(idata.down);
                            list[1].Win = list[1].Win + (amount_ * rate);
                            list[1].AmountDiscount = list[1].AmountDiscount + amountDis;
                            list[1].total = Convert.ToInt32(list[1].AmountDiscount) + list[1].Win;
                        }
                        else if (idata.Type == "t")
                        {
                            list[0].AmountDiscount = list[0].AmountDiscount + amountDis;
                            list[0].total = Convert.ToInt32(list[0].AmountDiscount) + list[0].Win;
                        }
                        else if (idata.Type == "b")
                        {
                            list[1].AmountDiscount = list[1].AmountDiscount + amountDis;
                            list[1].total = Convert.ToInt32(list[1].AmountDiscount) + list[1].Win;
                        }
                        else { }
                    }
                    else if (idata.NumLen == "2")
                    {
                        if (idata.Type == "t" && idata.Number == TwUP)
                        {
                            UpdateLottoSub(idata.Amount, idata.two_up, idata.ID);
                            int rate = Int32.Parse(idata.two_up);
                            list[7].Win = list[7].Win + (amount_ * rate);
                            list[7].AmountDiscount = list[7].AmountDiscount + amountDis;
                            list[7].total = Convert.ToInt32(list[7].AmountDiscount) + list[7].Win;
                        }
                        else if (idata.Type == "b" && idata.Number == TD)
                        {
                            UpdateLottoSub(idata.Amount, idata.two_down, idata.ID);
                            int rate = Int32.Parse(idata.two_down);
                            list[8].Win = list[8].Win + (amount_ * rate);
                            list[8].AmountDiscount = list[8].AmountDiscount + amountDis;
                            list[8].total = Convert.ToInt32(list[8].AmountDiscount) + list[8].Win;
                        }
                        else if (idata.Type == "t_" && (idata.Number == Tw_up_ood || idata.Number == TwUP))
                        {
                            UpdateLottoSub(idata.Amount, idata.two_ood, idata.ID);
                            int rate = Int32.Parse(idata.two_ood);
                            list[9].Win = list[9].Win + (amount_ * rate);
                            list[9].AmountDiscount = list[9].AmountDiscount + amountDis;
                            list[9].total = Convert.ToInt32(list[9].AmountDiscount) + list[9].Win;
                        }
                        else if (idata.Type == "b_" && (idata.Number == TDO1 || idata.Number == TD))
                        {
                            UpdateLottoSub(idata.Amount, idata.two_ood, idata.ID);
                            int rate = Int32.Parse(idata.two_ood);
                            list[9].Win = list[9].Win + (amount_ * rate);
                            list[9].AmountDiscount = list[9].AmountDiscount + amountDis;
                            list[9].total = Convert.ToInt32(list[9].AmountDiscount) + list[9].Win;
                        }
                        else if (idata.Type == "t")
                        {
                            list[7].AmountDiscount = list[7].AmountDiscount + amountDis;
                            list[7].total = Convert.ToInt32(list[7].AmountDiscount) + list[7].Win;
                        }
                        else if (idata.Type == "b")
                        {
                            list[8].AmountDiscount = list[8].AmountDiscount + amountDis;
                            list[8].total = Convert.ToInt32(list[8].AmountDiscount) + list[8].Win;
                        }
                        else if (idata.Type == "t_")
                        {
                            list[9].AmountDiscount = list[9].AmountDiscount + amountDis;
                            list[9].total = Convert.ToInt32(list[9].AmountDiscount) + list[9].Win;
                        }
                        else if (idata.Type == "b_")
                        {
                            list[9].AmountDiscount = list[9].AmountDiscount + amountDis;
                            list[9].total = Convert.ToInt32(list[9].AmountDiscount) + list[9].Win;
                        }
                        else { }
                    }
                    else if (idata.NumLen == "3")
                    {
                        if (idata.Type == "t" && idata.Number == TU)
                        {
                            UpdateLottoSub(idata.Amount, idata.three_up, idata.ID);
                            int rate = Int32.Parse(idata.three_up);
                            list[4].Win = list[4].Win + (amount_ * rate);
                            list[4].AmountDiscount = list[4].AmountDiscount + amountDis;
                            list[4].total = Convert.ToInt32(list[4].AmountDiscount) + list[4].Win;
                        }
                        else if (idata.Type == "b" && (idata.Number == ThDown1 || idata.Number == ThDown2 || idata.Number == ThDown3 || idata.Number == ThDown4))
                        {
                            UpdateLottoSub(idata.Amount, idata.three_down, idata.ID);
                            int rate = Int32.Parse(idata.three_down);
                            list[6].Win = list[6].Win + (amount_ * rate);
                            list[6].AmountDiscount = list[6].AmountDiscount + amountDis;
                            list[6].total = Convert.ToInt32(list[6].AmountDiscount) + list[6].Win;
                        }
                        else if (idata.Type == "t_" && (idata.Number == TU || idata.Number == TUO1 || idata.Number == TUO2 || idata.Number == TUO3 || idata.Number == TUO4 || idata.Number == TUO5))
                        {
                            UpdateLottoSub(idata.Amount, idata.three_ood, idata.ID);
                            int rate = Int32.Parse(idata.three_ood);
                            list[5].Win = list[5].Win + (amount_ * rate);
                            list[5].AmountDiscount = list[5].AmountDiscount + amountDis;
                            list[5].total = Convert.ToInt32(list[5].AmountDiscount) + list[5].Win;
                        }
                        else if (idata.Type == "f" && idata.Number == FT)
                        {
                            UpdateLottoSub(idata.Amount, idata.first_three, idata.ID);
                            int rate = Int32.Parse(idata.first_three);
                            list[2].Win = list[2].Win + (amount_ * rate);
                            list[2].AmountDiscount = list[2].AmountDiscount + amountDis;
                            list[2].total = Convert.ToInt32(list[2].AmountDiscount) + list[2].Win;
                        }
                        else if (idata.Type == "f_" && (idata.Number == FT || idata.Number == FTO1 || idata.Number == FTO2 || idata.Number == FTO3 || idata.Number == FTO4 || idata.Number == FTO5))
                        {
                            UpdateLottoSub(idata.Amount, idata.first_three_ood, idata.ID);
                            int rate = Int32.Parse(idata.first_three_ood);
                            list[3].Win = list[3].Win + (amount_ * rate);
                            list[3].AmountDiscount = list[3].AmountDiscount + amountDis;
                            list[3].total = Convert.ToInt32(list[3].AmountDiscount) + list[3].Win;
                        }
                        else if (idata.Type == "t")
                        {
                            list[4].AmountDiscount = list[4].AmountDiscount + amountDis;
                            list[4].total = Convert.ToInt32(list[4].AmountDiscount) + list[4].Win;
                        }
                        else if (idata.Type == "b")
                        {
                            list[6].AmountDiscount = list[6].AmountDiscount + amountDis;
                            list[6].total = Convert.ToInt32(list[6].AmountDiscount) + list[6].Win;
                        }
                        else if (idata.Type == "t_")
                        {
                            list[5].AmountDiscount = list[5].AmountDiscount + amountDis;
                            list[5].total = Convert.ToInt32(list[5].AmountDiscount) + list[5].Win;
                        }
                        else if (idata.Type == "f")
                        {
                            list[2].AmountDiscount = list[2].AmountDiscount + amountDis;
                            list[2].total = Convert.ToInt32(list[2].AmountDiscount) + list[2].Win;
                        }
                        else if (idata.Type == "f_")
                        {
                            list[3].AmountDiscount = list[3].AmountDiscount + amountDis;
                            list[3].total = Convert.ToInt32(list[3].AmountDiscount) + list[3].Win;
                        }
                        else { }
                    }
                    else { }
                }
            }
            int id = Int32.Parse(PID);
            Period p = db.Period.Where(s => s.ID == id).FirstOrDefault<Period>();
            p.Check_Result = "1";
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var r = new Result();
            r.Period_ID = id;
            r.Name = "หวยรัฐบาล";
            r.Lotto_day = DAY;
            r.Up = UP;
            r.Down = DOWN;
            r.two_up = TwUP;
            r.two_up_ood = Tw_up_ood;
            r.two_down = TD;
            r.two_down_ood = TDO1;
            r.first_three = FT;
            r.first_3_ood_1 = FTO1;
            r.first_3_ood_2 = FTO2;
            r.first_3_ood_3 = FTO3;
            r.first_3_ood_4 = FTO4;
            r.first_3_ood_5 = FTO5;
            r.last_three = TU;
            r.last_3_ood_1 = TUO1;
            r.last_3_ood_2 = TUO2;
            r.last_3_ood_3 = TUO3;
            r.last_3_ood_4 = TUO4;
            r.last_3_ood_5 = TUO5;
            r.three_down_1 = ThDown1;
            r.three_down_2 = ThDown2;
            r.three_down_3 = ThDown3;
            r.three_down_4 = ThDown4;
            r.update_date = DateTime.Now;
            r.create_date = DateTime.Now;
            db.Result.Add(r);
            db.SaveChanges();

            foreach (var ilist in list)
            {
                var discount = Convert.ToInt32(ilist.AmountDiscount);
                var tr = new Total_Amount_Result();
                tr.Type = ilist.Type;
                tr.Amount_Discount = discount.ToString();
                tr.Period_ID = PID;
                tr.Amount_Win = ilist.Win.ToString();
                tr.create_date = DateTime.Now;
                tr.update_date = DateTime.Now;
                db.Total_Amount_Result.Add(tr);
                
            }
            db.SaveChanges();
            return Json("ss");
        }

        public void UpdateLottoSub(string Amount, string Rate, string ID)
        {
            int id = Int32.Parse(ID);
            var amount = Int32.Parse(Amount);
            var rate = Int32.Parse(Rate);
            var win = amount * rate;
            LottoSub ls = db.LottoSub.Where(s => s.ID == id).FirstOrDefault<LottoSub>();
            ls.update_date = DateTime.Now;
            ls.AmountWin = win.ToString();
            ls.Result_Status = "1";
            db.Entry(ls).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult GetUserTotalBalance(string uID)
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            Period P = db.Period.Where(y=>y.UID== parentID).Where(x => x.Status == "1").FirstOrDefault<Period>();
            var pid = 0;
            if (P != null)
            {
                pid = P.ID;
            }
            else
            {
                int id = db.Period.Where(x=>x.UID== parentID).Max(p => p.ID);
                pid = id;
            }
            string connetionString = null;
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            var total_type_bet = new List<Total_Amount_Result>();
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "select t.Type,ROUND(sum(CAST(t.AmountDiscount as float)),0) as AmountDiscount,sum(CAST(t.Win as int)) as Amount_Win from(SELECT CASE WHEN ls.Type='t' and ls.NumLen='1' THEN 'Up' WHEN ls.Type='b' and ls.NumLen='1' THEN 'Down' WHEN ls.Type='f' and ls.NumLen='3' THEN 'FirstThree' WHEN ls.Type='f_' and ls.NumLen='3' THEN 'FirstThreeOod' WHEN ls.Type='t' and ls.NumLen='3' THEN 'ThreeUp' WHEN ls.Type='t_' and ls.NumLen='3' THEN 'ThreeUPOod' WHEN ls.Type='b' and ls.NumLen='3' THEN 'ThreeDown' WHEN ls.Type='t' and ls.NumLen='2' THEN 'TwoUp' WHEN ls.Type='b' and ls.NumLen='2' THEN 'TwoDown' WHEN (ls.Type='t_' or ls.Type='b_') and ls.NumLen='2' THEN 'TwoOod' ELSE null END as Type ,ls.AmountDiscount,ls.AmountWin as Win FROM [dbo].[Period] pe " +
                "left join(SELECT [ID],[Period_ID],[Receive] FROM [dbo].[Poll] where Receive='1' and UID=" + uID + ") po on pe.ID=po.Period_ID " +
                "left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID " +
                "left join(SELECT [ID],[Lotto_ID],[Type],[NumLen],[AmountDiscount],AmountWin FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID " +
                "where pe.ID=" + pid + ")t group by t.Type";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        total_type_bet.Add(new Total_Amount_Result
                        {
                            Type = Reader["Type"].ToString(),
                            Amount_Discount = Reader["AmountDiscount"].ToString(),
                            Amount_Win = Reader["Amount_Win"].ToString(),
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
            return Json(total_type_bet);
        }

        [HttpPost]
        public ActionResult GetUserBetResult(string uID)
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            Period P = db.Period.Where(y=>y.UID== parentID).Where(x => x.Status == "1").FirstOrDefault<Period>();
            var pid = 0;
            if (P != null)
            {
                pid = P.ID;
            }
            else
            {
                int id = db.Period.Where(x=>x.UID==parentID).Max(p => p.ID);
                pid = id;
            }
            string connetionString = null;
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            var user_bet = new List<User_Bet_Result>();
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT po.UID,a.Name,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as AmountDiscount,sum(CAST(ls.AmountWin as int)) as AmountWin FROM [dbo].[Period] pe " +
                            "left join(SELECT [ID],[UID],[Period_ID],[Receive] FROM [dbo].[Poll] where Receive='1' and UID=" + uID + ") po on pe.ID=po.Period_ID " +
                            "left join(SELECT [ID],[Name] FROM [dbo].[Account]) a on po.UID=a.ID " +
                            "left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID " +
                            "left join(SELECT [ID],[Lotto_ID],[AmountDiscount],[AmountWin] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where pe.ID=" + pid + " group by po.UID,a.Name";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        user_bet.Add(new User_Bet_Result
                        {
                            ID = Reader["UID"].ToString(),
                            Name = Reader["Name"].ToString(),
                            Discount = Reader["AmountDiscount"].ToString(),
                            Win = Reader["AmountWin"].ToString(),
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
            return Json(user_bet);
        }

        [HttpPost]
        public ActionResult RemoveRate(int RID)
        {
            Rate R = db.Rate.Where(s => s.UID == RID).First();
            if (R != null)
            {
                db.Rate.RemoveRange(db.Rate.Where(x => x.UID == RID));
                db.SaveChanges();
                
            }
            Discount d = db.Discount.Where(s => s.UID == RID).First();
            if (d != null)
            {
                db.Discount.RemoveRange(db.Discount.Where(x => x.UID == RID));
                db.SaveChanges();

            }
            return Json("ss");
        }

        [HttpPost]
        public ActionResult GetTotalBet()
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            Period P = db.Period.Where(y=>y.UID== parentID).Where(x => x.Status == "1").FirstOrDefault<Period>();
            var pid = 0;
            var totalbet = 0;
            if (P != null)
            {
                pid = P.ID;
            }
            else
            {
                int id = db.Period.Where(x=>x.UID== parentID).Max(p => p.ID);
                pid = id;
            }
            string connetionString = null;
            //var pollDetail = new List<Poll_Detail>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "select t1.amount from(SELECT p.[ID],p.UID,a.Name,p.[Receive],p.[Create_By],sum(CAST(LS.Amount as int)) as amount,sum(CAST(ls.AmountDiscount as int)) as discount,p.create_date FROM [dbo].[Poll] p left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) LM on p.ID=LM.Poll_ID  left join(SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) LS on LM.ID=LS.Lotto_ID  left join(SELECT [ID],[Name] FROM [dbo].[Account]) a on p.UID =a.ID where p.Period_ID=" + pid + " and p.[Receive] = 1 group by p.ID,p.Receive,p.create_date,p.[Create_By],a.Name,p.UID) t1 order by Row_Number() OVER (Partition BY t1.Name ORDER BY t1.Name) desc";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        totalbet += Int32.Parse(Reader["amount"].ToString());
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
            return Json(totalbet);
        }
        [HttpPost]
        public JsonResult LottoBackwardTotalBalance(string PID)
        {
            int id = Int32.Parse(PID);
            var tar = new List<Total_Amount_Result>();
            Period P = db.Period.Where(x => x.ID == id).FirstOrDefault<Period>();
            string connetionString = null;
            var all = new List<Close>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT pe.ID,pe.Check_Result,pe.create_date,pe.update_date,pe.Date,case when pe.BetStatus='0' then 'ปิดรับ' else 'เปิดรับ' end as BetStatus,pe.Close_BY,sum(ISNULL(CAST(ls.Amount as int), 0 )) as Amount,ISNULL(cr.crR, 0 ) as crR,ISNULL(cu.countu , 0 ) as countu FROM [dbo].[Period] pe left join(SELECT [ID],Period_ID,Receive,update_date,create_date FROM [dbo].[Poll] where Receive='1') p on p.Period_ID=pe.ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on p.ID=lm.Poll_ID left join (SELECT [ID],[Lotto_ID],[Amount],[AmountDiscount] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID left join(SELECT Period_ID as crPID,COUNT(Receive) as crR,Receive as crRe FROM [dbo].[Poll] where Receive='1' group by Receive,Period_ID) cr on p.Period_ID=cr.crPID and p.Receive=cr.crRe left join(SELECT COUNT(distinct UID) as countu,Period_ID as cuPID,Receive as cuR FROM [dbo].[Poll] where Receive='1' group by Receive,Period_ID,Receive) cu on p.Period_ID=cu.cuPID and p.Receive=cu.cuR where pe.ID=@period group by pe.ID,pe.create_date,pe.Date,pe.Close_BY,pe.Check_Result,pe.update_date,cr.crR,cu.countu,pe.BetStatus";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@period", id.ToString());
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        all.Add(new Close
                        {
                            PID = Reader["ID"].ToString(),
                            Date = Convert.ToDateTime(Reader["Date"]),
                            Amount = Reader["Amount"].ToString(),
                            CountReceive = Reader["crR"].ToString(),
                            CountUser = Reader["countu"].ToString(),
                            CloseBy = Reader["Close_BY"].ToString(),
                            CreateDate = Reader["create_date"].ToString(),
                            CloseDate = Reader["update_date"].ToString(),
                            BetStatus = Reader["BetStatus"].ToString(),
                            CheckResult= Reader["Check_Result"].ToString()
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

            var user_bet = new List<User_Bet_Result>();
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "SELECT po.UID,a.Name,ROUND(sum(CAST(ls.AmountDiscount as float)),0) as AmountDiscount,sum(CAST(ls.AmountWin as int)) as AmountWin FROM [dbo].[Period] pe " +
                                "left join(SELECT [ID],[UID],[Period_ID],[Receive] FROM [dbo].[Poll] where Receive='1') po on pe.ID=po.Period_ID "+
                                "left join(SELECT [ID],[Name] FROM [dbo].[Account]) a on po.UID=a.ID "+
                                "left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[AmountDiscount],[AmountWin] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where pe.ID=@period group by po.UID,a.Name";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@period", id.ToString());
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        user_bet.Add(new User_Bet_Result
                        {
                            ID = Reader["UID"].ToString(),
                            Name = Reader["Name"].ToString(),
                            Discount = Reader["AmountDiscount"].ToString(),
                            Win = Reader["AmountWin"].ToString(),
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
            if (all[0].CheckResult=="1")
            {
                List<Total_Amount_Result> TAR = db.Total_Amount_Result.Where(x => x.Period_ID == id.ToString()).ToList();
                ViewData["TAR"] = TAR;
                ViewData["UBET"] = user_bet;
                tar = TAR;
            }
            else if(all[0].CheckResult == "0")
            {
                var total_type_bet = new List<Total_Amount_Result>();
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "select t.Type,ROUND(sum(CAST(t.AmountDiscount as float)),0) as AmountDiscount,sum(CAST(t.Win as int)) as Win from(SELECT CASE WHEN ls.Type='t' and ls.NumLen='1' THEN 'Up' WHEN ls.Type='b' and ls.NumLen='1' THEN 'Down' WHEN ls.Type='f' and ls.NumLen='3' THEN 'FirstThree' WHEN ls.Type='f_' and ls.NumLen='3' THEN 'FirstThreeOod' WHEN ls.Type='t' and ls.NumLen='3' THEN 'ThreeUp' WHEN ls.Type='t_' and ls.NumLen='3' THEN 'ThreeUPOod' WHEN ls.Type='b' and ls.NumLen='3' THEN 'ThreeDown' WHEN ls.Type='t' and ls.NumLen='2' THEN 'TwoUp' WHEN ls.Type='b' and ls.NumLen='2' THEN 'TwoDown' WHEN (ls.Type='t_' or ls.Type='b_') and ls.NumLen='2' THEN 'TwoOod' ELSE null END as Type ,ls.AmountDiscount,'0' as Win FROM [dbo].[Period] pe left join(SELECT [ID],[Period_ID],[Receive] FROM [dbo].[Poll] where Receive='1') po on pe.ID=po.Period_ID left join(SELECT [ID],[Poll_ID] FROM [dbo].[LottoMain]) lm on po.ID=lm.Poll_ID left join(SELECT [ID],[Lotto_ID],[Type],[NumLen],[AmountDiscount] FROM [dbo].[LottoSub]) ls on lm.ID=ls.Lotto_ID where pe.ID=@period)t group by t.Type";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@period", id.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            total_type_bet.Add(new Total_Amount_Result
                            {
                                Type = Reader["Type"].ToString(),
                                Amount_Discount = Reader["AmountDiscount"].ToString(),
                                Amount_Win = Reader["Win"].ToString(),
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
                ViewData["UBET"] = user_bet;
                tar = total_type_bet;
            }
            else { }
            return Json(new { all, TAR = tar, UBET = user_bet });
        }
        //--------------------------------ดึงข้อมูลไปแสดงใน moddal หน้า ดูเลขทั้งหมด------------------------------------------///
        [HttpPost]
        public ActionResult getUserPoll(string Number, string Type, string NumLen)
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            int id = db.Period.Where(x=>x.UID==parentID).Max(p => p.ID);

            string connetionString = null;
            var data = new List<UserAllBet>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "select a.ID, a.Username,a.Name,sum(CAST(ls.Amount as int)) as Amount,COUNT(DISTINCT t1.ID) as CPoll from(SELECT [ID],[UID],[Period_ID],[Receive] FROM[dbo].[Poll] where Period_ID = @period and Receive = '1') t1 left join(SELECT[ID],[Username],[Name] FROM[dbo].[Account]) a on t1.UID = a.ID left join(SELECT[ID], [Poll_ID] FROM [dbo].[LottoMain]) lm on t1.ID = lm.Poll_ID left join(SELECT[ID], [Lotto_ID], [Type], [NumLen], [Number], [Amount] FROM [dbo].[LottoSub] where Number=@number and Type = @type and NumLen = @nlen) ls on lm.ID = ls.Lotto_ID where ls.Number is not null group by a.ID,a.Username,a.Name";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@period", id.ToString());
                cmd.Parameters.AddWithValue("@number", Number);
                cmd.Parameters.AddWithValue("@type", Type);
                cmd.Parameters.AddWithValue("@nLen", NumLen);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        data.Add(new UserAllBet
                        {
                            UID = Reader["ID"].ToString(),
                            Username = Reader["Username"].ToString(),
                            Name = Reader["Name"].ToString(),
                            Amount = Reader["Amount"].ToString(),
                            poll_count = Reader["CPoll"].ToString(),
                            Number = Number,
                            Type = Type,
                            nLen = NumLen
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
        //--------------------------------ดึงข้อมูลไปแสดงใน moddal หน้า ดูเลขทั้งหมด------------------------------------------///
        [HttpPost]
        public ActionResult getUserPollDetail(string UID,string Number, string Type, string NumLen)
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            int id = db.Period.Where(x=>x.UID== parentID).Max(p => p.ID);

            string connetionString = null;
            var data = new List<Poll_Detail>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "select t.ID,t.poll_id,t.Username, t.Poll_Name,t.Receive,sum(CAST(LS.Amount as int)) as Amount,ROUND(sum(CAST(ls.AmountDiscount as float)), 0) as AmountDiscount " +
                               ",sum(CAST(LS.AmountWin as int)) as AmountWin,t.IP,t.create_date,t.Create_By,rn.rNumber from(select a.ID, a.Username, a.Name, ls.Amount, t1.Poll_Name, t1.Receive, t1.ID as poll_id " +
                               ", t1.IP, t1.create_date,t1.Create_By from(SELECT[ID],[UID], Poll_Name,[Period_ID],[Receive], IP, create_date,Create_By FROM[dbo].[Poll] where Period_ID = @period and Receive = '1') t1 " +
                               "left join(SELECT[ID],[Username],[Name] FROM[dbo].[Account] where ID = @uid) a on t1.UID = a.ID left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm "+
                               "on t1.ID = lm.Poll_ID left join(SELECT[ID],[Lotto_ID],[Type],[NumLen],[Number],[Amount] FROM[dbo].[LottoSub] where Number = @number and Type = @type "+
                               "and NumLen = @nLen) ls on lm.ID = ls.Lotto_ID where ls.Number is not null and a.ID is not null) t left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm "+
                               "on t.poll_id = lm.Poll_ID left join(SELECT[ID], [Lotto_ID], [Amount], [AmountDiscount], [AmountWin] FROM [dbo].[LottoSub]) ls on lm.ID = ls.Lotto_ID "+
                               "left join(SELECT [ID], Row_Number() OVER(Partition BY UID ORDER BY UID) as rNumber FROM[dbo].[Poll] where Period_ID = @period and UID = @uid) rn " +
                                "on t.poll_id = rn.ID group by t.ID,t.poll_id,t.Username,t.Poll_Name,t.Receive,t.IP,t.create_date,rn.rNumber,t.Create_By";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@period", id.ToString());
                cmd.Parameters.AddWithValue("@uid", UID);
                cmd.Parameters.AddWithValue("@number", Number);
                cmd.Parameters.AddWithValue("@type", Type);
                cmd.Parameters.AddWithValue("@nLen", NumLen);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        data.Add(new Poll_Detail
                        {
                            UID = Reader["ID"].ToString(),
                            ID = Reader["poll_id"].ToString(),
                            name = Reader["Username"].ToString(),
                            Poll_Name = Reader["Poll_Name"].ToString(),
                            Receive = Reader["Receive"].ToString(),
                            Amount = Reader["Amount"].ToString(),
                            Discount = Reader["AmountDiscount"].ToString(),
                            Win = Reader["AmountWin"].ToString(),
                            IP = Reader["IP"].ToString(),
                            create_date= Reader["create_date"].ToString(),
                            poll_number = Reader["rNumber"].ToString(),
                            Create_By = Reader["Create_By"].ToString(),
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
        //--------------------------------ดึงข้อมูลไปแสดงใน moddal หน้า สรุปยอดเป็นใบ------------------------------------------///
        [HttpPost]
        public ActionResult getUserPoll2(string uid)
        {
            var parentID = Int32.Parse((string)Session["ParentID"]);
            int id = db.Period.Where(x=>x.UID== parentID).Max(p => p.ID);

            string connetionString = null;
            var data = new List<Poll_Detail>();
            connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
            try
            {
                SqlConnection cnn = new SqlConnection(connetionString);
                cnn.Open();
                string query = "select * from(select p.poll_id, p.id, a.Username, p.[Poll_Name], p.[Period_ID], p.[Receive], p.[Create_By], p.[IP], p.[create_date], rn.rNumber, sum(CAST(LS.Amount as int)) as Amount , ROUND(sum(CAST(ls.AmountDiscount as float)), 0) as AmountDiscount, sum(CAST(LS.AmountWin as int)) as AmountWin, sum(CAST(LS.Result_Status as int)) as result from(SELECT [ID] as poll_id,[UID] as id,[Poll_Name],[Period_ID],[Receive],[Create_By],[IP],[create_date] FROM[dbo].[Poll] where Period_ID = @period and UID = @uid and Receive = '1') p left join(SELECT[ID],[Poll_ID] FROM[dbo].[LottoMain]) lm on p.poll_id = lm.Poll_ID left join(SELECT[ID],[Lotto_ID],[Amount],[AmountDiscount],[AmountWin],[Result_Status] FROM[dbo].[LottoSub]) ls on lm.ID = ls.Lotto_ID left join(SELECT[ID],[Username] FROM[dbo].[Account]) a on p.id = a.ID left join(SELECT[ID], Row_Number() OVER(Partition BY UID ORDER BY UID) as rNumber FROM[dbo].[Poll] where Period_ID = @period and UID = @uid) rn on p.poll_id = rn.ID group by p.poll_id, p.id, a.Username, p.[Poll_Name], p.[Period_ID], p.[Receive], p.[Create_By], p.[IP], p.[create_date], rn.rNumber) t where result > 0";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@period", id.ToString());
                cmd.Parameters.AddWithValue("@uid", uid);
                SqlDataReader Reader = cmd.ExecuteReader();
                Console.Write(Reader);
                try
                {
                    while (Reader.Read())
                    {
                        data.Add(new Poll_Detail
                        {
                            UID = Reader["ID"].ToString(),
                            ID = Reader["poll_id"].ToString(),
                            name = Reader["Username"].ToString(),
                            Poll_Name = Reader["Poll_Name"].ToString(),
                            Receive = Reader["Receive"].ToString(),
                            Amount = Reader["Amount"].ToString(),
                            Discount = Reader["AmountDiscount"].ToString(),
                            Win = Reader["AmountWin"].ToString(),
                            IP = Reader["IP"].ToString(),
                            create_date= Reader["create_date"].ToString(),
                            poll_number = Reader["rNumber"].ToString(),
                            Create_By = Reader["Create_By"].ToString(),
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
        //--------------------------------บันทึกข้อมูลลงตาราแทงออก และรับไว้------------------------------------------///
        [HttpPost]
        public ActionResult addPollBetOut(PollData Data)
        {
            var pOut = new Poll_Bet_Out();
            
            int user_id = Int32.Parse( Session["ID"].ToString());
            int pid = db.Period.Where(x => x.UID == user_id).Max(p => p.ID);

            pOut.UID = user_id;
            pOut.Period_ID = pid;
            pOut.Status = "0";
            pOut.create_date = DateTime.Now;
            db.Poll_Bet_Out.Add(pOut);
            db.SaveChanges();
            int pollOut_id = pOut.ID;
            foreach (var item in Data.poll)
            {
                var lOut = new Lotto_Bet_Out();
                lOut.Poll_Out_ID = pollOut_id;
                lOut.Type = item.bType;
                lOut.NumLen = item.NumLen;
                lOut.Number = item.Number;
                lOut.Amount = item.Amount;
                lOut.AmountWin = "0";
                lOut.AmountDiscount = "0";
                lOut.Result_Status = "0";
                lOut.create_date = DateTime.Now;
                lOut.update_date = DateTime.Now;
                db.Lotto_Bet_Out.Add(lOut);
            }
            foreach(var item in Data.pollReceive)
            {
                var lReceive = new Lotto_Bet_Receive();
                lReceive.UID = user_id;
                lReceive.Period_ID = pid;
                lReceive.Type = item.bType;
                lReceive.NumLen = item.NumLen;
                lReceive.Number = item.Number;
                lReceive.Amount = item.Amount;
                lReceive.AmountWin = "0";
                lReceive.AmountDiscount = "0";
                lReceive.Result_Status = "0";
                lReceive.Status = "0";
                lReceive.create_date = DateTime.Now;
                lReceive.update_date = DateTime.Now;
                db.Lotto_Bet_Receive.Add(lReceive);
            }
            db.SaveChanges();
            return Json(pollOut_id);
        }
    }
}