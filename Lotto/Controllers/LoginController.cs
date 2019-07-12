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
    public class LoginController : Controller
    {
        ltfomEntities db = new ltfomEntities();
        // GET: Login
        public ActionResult Login()
        {
            if ((string)Session["Role"] == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if((string)Session["Role"] == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Login(UserLogin objUser)
        {
            if (ModelState.IsValid)
            {
                string connetionString = null;
                var user = new List<UserLogin>();
                connetionString = WebConfigurationManager.ConnectionStrings["LottoDB"].ConnectionString;
                try
                {
                    SqlConnection cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    string query = "SELECT u.ID,u.Username,u.Password,u.Last_Login,r.Role FROM[dbo].[Account] u left join(SELECT[UID],[Role_ID] FROM [dbo].[Account_Role]) ar on u.ID=ar.UID left join(select ID, Role from [dbo].[Role]) r on ar.Role_ID=r.ID where u.Username=@username and u.Status=1";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@username", objUser.Username.ToString());
                    SqlDataReader Reader = cmd.ExecuteReader();
                    Console.Write(Reader);
                    try
                    {
                        while (Reader.Read())
                        {
                            user.Add(new UserLogin
                            {
                                ID = Reader["ID"].ToString(),
                                Username = Reader["Username"].ToString(),
                                Password = Reader["Password"].ToString(),
                                Last_Login = Reader["Last_Login"].ToString(),
                                Role = Reader["Role"].ToString(),
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
                    bool verify = VerifyHash(objUser.Password, user[0].Password);
                    if (verify)
                    {                      
                        bool role = Check_Role("admin", user[0].Role);
                        if (role)
                        {
                            Session["ID"] = user[0].ID;
                            Session["Username"] = user[0].Username;
                            Session["Role"] = "Administrator";
                            Session["Last_Login"] = user[0].Last_Login;
                            //Session["sessionid"] = System.Web.HttpContext.Current.Session.SessionID;
                            return RedirectToAction("Index", "Admin");                            
                        }
                        else
                        {
                            role = Check_Role("user", user[0].Role);
                            if (role)
                            {
                                Session["ID"] = user[0].ID;
                                Session["Username"] = user[0].Username;
                                Session["Role"] = "User";
                                Session["Last_Login"] = user[0].Last_Login;
                                Session["sessionid"] = System.Web.HttpContext.Current.Session.SessionID;
                                int id = Int32.Parse(user[0].ID);
                                Account A = db.Account.Where(s => s.ID == id).FirstOrDefault<Account>();
                                if (A != null)
                                {
                                    A.Last_Login = DateTime.Now;      
                                    A.SessionID= System.Web.HttpContext.Current.Session.SessionID;
                                    db.Entry(A).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }

                                return RedirectToAction("Index", "User");
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
            ViewBag.Message = "Fail";
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
        public bool Check_Role(string plainText, string hashValue)
        {
            bool verify = VerifyHash(plainText, hashValue);
            return verify;
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
    }
}