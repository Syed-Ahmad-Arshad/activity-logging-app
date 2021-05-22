using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActivityLoggingProject.Controllers
{
        
    public class HomeController : Controller
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Test;User ID=sa;Password=abc";
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult UserHome()
        {
            return View();
        }

        public ActionResult Authorize() //Function to authorize a user login
        {
            string Login = Request["UserName"];
            string Password = Request["Password"];
            //DataBase Logic
            string connString = connectionString;
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            string sqlQuery = "Select Password from dbo.Users where Name = '" + Login + "'";
            SqlCommand command = new SqlCommand(sqlQuery, conn);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read() == true)
            {
                string password1 = (string)reader.GetValue(0);
                if (password1 == Password)
                {
                    conn.Close();
                    TempData["username"] = Login;
                    Session["id"] = Login;
                    return Redirect("~/Home/UserHome");
                }
                else
                {
                    conn.Close();
                    ViewBag.ErrorMsg = "Invalid Password";
                    return View("Login");
                }
            }
            ViewBag.ErrorMsg = "Login does not exist";
            conn.Close();
            return View("Login");
        }

        public ActionResult Signup() //Function to return signup view
        {
            return View();
        }
        public ActionResult SaveUser() //Function to save user
        {
            string Name = Request["UserName"];
            string Email = Request["Email"];
            string Password = Request["Password"];
            string ConfrimPassword = Request["ConfirmPassword"];
            //DataBase Logic
            string connString = connectionString;
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();


            //
            SqlConnection conn2 = new SqlConnection(connString);
            conn2.Open();
            string sqlQuery2 = String.Format(@"SELECT * from dbo.Users WHERE Name = '{0}'", Name);
            SqlCommand command2 = new SqlCommand(sqlQuery2, conn2);
            SqlDataReader dr = command2.ExecuteReader();
            if (dr.Read())
            {
                ViewBag.status = "User name already exists.";
                return View("Signup");
            }


            //
            string sqlQuery = String.Format(@"INSERT INTO dbo.Users(Name,Password, Email) VALUES('{0}','{1}','{2}')", Name, Password, Email);
            SqlCommand command = new SqlCommand(sqlQuery, conn);
            int recAff = command.ExecuteNonQuery();
            if (recAff == 1)
            {
                TempData["username"] = Name;
                return View("UserHome");
            }

            return Redirect("~/Home/SignUp");
        }
        public JsonResult getActivities(string username) //Function that returns all activities
        {
            string connString = connectionString;
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            string sqlQuery = "Select ID,ActivityName, UserName, Priority, Labels from dbo.ActivityTable where UserName='" + username + "'";
            SqlCommand command = new SqlCommand(sqlQuery, conn);
            SqlDataReader reader = command.ExecuteReader();
            List<ActivityLoggingProject.Models.Activity> activities = new List<ActivityLoggingProject.Models.Activity>();
            while (reader.Read() == true)
            {
                ActivityLoggingProject.Models.Activity dto = new ActivityLoggingProject.Models.Activity();
                dto.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                dto.usrname = reader.GetString(reader.GetOrdinal("username"));
                dto.activityname = reader.GetString(reader.GetOrdinal("activityname"));
                dto.priority = reader.GetString(reader.GetOrdinal("priority"));
                dto.labels = reader.GetString(reader.GetOrdinal("labels"));


                activities.Add(dto);
            }

            return Json(activities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddActivity(string username, string activityname, string priority, string labels = "") //Function to add an activity
        {
            String connString = connectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                //Step 2: Open connection.
                conn.Open();

                //Step 3" Build the query.
                String sqlQuery = String.Format(@"INSERT INTO dbo.ActivityTable(ActivityName,UserName, Priority, Labels) VALUES('{0}','{1}', '{2}', '{3}')", activityname, username, priority, labels);

                //Step 4: Build command object. 
                SqlCommand command = new SqlCommand(sqlQuery, conn);
                int recAff = command.ExecuteNonQuery();
                //Step 5: Execute non-query.
                SqlConnection conn2 = new SqlConnection(connString);
                conn2.Open();
                ActivityLoggingProject.Models.Activity dto = new ActivityLoggingProject.Models.Activity();
                sqlQuery = "Select ID, ActivityName, UserName, Priority, Labels from dbo.ActivityTable where UserName ='" + username + "' and ActivityName='" + activityname + "'";
                SqlCommand command2 = new SqlCommand(sqlQuery, conn2);
                SqlDataReader reader = command2.ExecuteReader();
                if (reader.Read())
                {
                    dto.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                    dto.usrname = reader.GetString(reader.GetOrdinal("username"));
                    dto.activityname = reader.GetString(reader.GetOrdinal("activityname"));
                    dto.priority = reader.GetString(reader.GetOrdinal("priority"));
                    dto.labels = reader.GetString(reader.GetOrdinal("labels"));
                }
                else
                {
                    
                }
                return Json(dto, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getActivity(int ID) //Function to return a single activity
        {
            String connString = connectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                //Step 2: Open connection.
                conn.Open();

                //Step 3" Build the query.
                String sqlQuery = "Select ActivityName, UserName from dbo.ActivityTable where ID ='" + ID;

                //Step 4: Build command object. 
                SqlCommand command = new SqlCommand(sqlQuery, conn);
                SqlDataReader reader = command.ExecuteReader();
                ActivityLoggingProject.Models.Activity activ = new ActivityLoggingProject.Models.Activity();
                if (reader.Read())
                {
                    activ.activityname = (string)reader.GetValue(0);
                    activ.usrname = (string)reader.GetValue(0);

                }
                return Json(activ, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult deleteActivity(int ID) //Function to delete an activity
        {
            //Step 1: Build connection using connection string.
            String connString = connectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                //Step 2: Open connection.
                conn.Open();

                //Step 3" Build the query.
                String sqlQuery = String.Format(@"Delete from dbo.ActivityTable where ID = {0}", ID);

                //Step 4: Build command object. 
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, conn);
                bool flag;
                //Step 5: Execute non-query.
                int recAff = sqlCommand.ExecuteNonQuery();
                if (recAff == 1)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
        }
       
        public JsonResult EditActivityData(string activityName, string priority, string labels, int ID, string username) //Function to edit an activity
        {
            //Step 1: Build connection using connection string.
            String connString = connectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                //Step 2: Open connection.
                conn.Open();

                //Step 3" Build the query.
                String sqlQuery = String.Format(@"Update dbo.ActivityTable Set ActivityName ='{0}', Priority = '{1}', Labels = '{2}', UserName='{3}' where ID = '{4}'", activityName, priority, labels, username, ID);

                //Step 4: Build command object. 
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, conn);

                //Step 5: Execute non-query.
                int recAff = sqlCommand.ExecuteNonQuery();
                bool flag;
                if (recAff == 1)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);

            }
        }
    }
}