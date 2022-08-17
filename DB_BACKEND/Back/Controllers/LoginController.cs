using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ModelContext _Context;
        public LoginController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        [HttpPost]
        public string Login()
        {
            LoginMessage loginMessage = new LoginMessage();
            string account = Request.Form["account"];
            decimal uid;
            string password = Request.Form["password"];
            if (account != null && password != null)
            {
                try
                {
                    uid = decimal.Parse(account);
                }
                catch
                {
                    loginMessage.errorCode = 201;//有account参数但其为空
                    return loginMessage.ReturnJson();
                }
                loginMessage.errorCode = 200;
            }
            else
            {
                loginMessage.errorCode = 202;//有参数为空
                return loginMessage.ReturnJson();
            }
            var users = from u in _Context.Users
                        where u.UserId == uid && u.Password == password
                        select u;
            var user = users.FirstOrDefault();
            if (user != null)
            {
                loginMessage.data["loginState"] = true;
                loginMessage.data["userName"] = user.UserName;
                loginMessage.data["userType"] = user.UserType;
                var token = Token.GetToken(new TokenInfo
                {
                    id = uid,
                    password = password
                });
                loginMessage.data.Add("token", token);
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = false;
                cookieOptions.SameSite = SameSiteMode.None;
                cookieOptions.Secure = true;
                cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
                Response.Cookies.Append("Token", token, cookieOptions);
            }
            return loginMessage.ReturnJson();
        }
        // GET: api/<LoginController>
        [HttpGet]
        public User Get()
        {
            String userid;
            Request.Cookies.TryGetValue("user_id", out userid);

            return _Context.Users.Single(x => x.UserId == int.Parse(userid));
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost("student")]
        public string StudentLogin()
        {
            LoginMessage loginMessage = new LoginMessage();
            string account = Request.Form["account"];
            decimal uid;
            string password = Request.Form["password"];
            if (account != null && password != null)
            {
                try
                {
                    uid = decimal.Parse(account);
                }
                catch
                {
                    loginMessage.errorCode = 201;//有account参数但其为空
                    return loginMessage.ReturnJson();
                }
                loginMessage.errorCode = 200;
            }else
            {
                loginMessage.errorCode = 202;//有参数为空
                return loginMessage.ReturnJson();
            }
            var sUser = from s in _Context.Students
                        join u in _Context.Users
                        on s.StudentId equals u.UserId
                        where s.StudentId == uid && u.Password == password
                        select u;
            var student = sUser.FirstOrDefault();
            if (student != null)
            {
                loginMessage.data["loginState"] = true;
                loginMessage.data["UserName"] = student.UserName;
                var token = Token.GetToken(new TokenInfo
                {
                    id = uid,
                    password = password
                });
                loginMessage.data.Add("token", token);
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = false;
                cookieOptions.SameSite = SameSiteMode.None;
                cookieOptions.Secure = true;
                cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
                Response.Cookies.Append("Token", token, cookieOptions);
            }
            //var request = Request;
            return loginMessage.ReturnJson();
        }
        [HttpPost("instructor")]
        public string InstructorLogin()
        {
            LoginMessage loginMessage = new LoginMessage();
            string account = Request.Form["account"];
            decimal uid;
            string password = Request.Form["password"];
            if (account != null && password != null)
            {
                try
                {
                    uid = decimal.Parse(account);
                }
                catch
                {
                    loginMessage.errorCode = 201;//有account参数但其为空
                    return loginMessage.ReturnJson();
                }
                loginMessage.errorCode = 200;
            }
            else
            {
                loginMessage.errorCode = 202;//有参数为空
                return loginMessage.ReturnJson();
            }
            var iUser = from i in _Context.Instructors
                        join u in _Context.Users
                        on i.InstructorId equals u.UserId
                        where i.InstructorId == uid && u.Password == password
                        select u;
            var instructor = iUser.FirstOrDefault();
            if (instructor != null)
            {
                loginMessage.data["loginState"] = true;
                loginMessage.data["UserName"] = instructor.UserName;
                var token = Token.GetToken(new TokenInfo
                {
                    id = uid,
                    password = password
                });
                loginMessage.data.Add("token", token);
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = false;
                cookieOptions.SameSite = SameSiteMode.None;
                cookieOptions.Secure = true;
                cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
                Response.Cookies.Append("Token", token, cookieOptions);
            }
            //var request = Request;
            return loginMessage.ReturnJson();
        }
    }

}
