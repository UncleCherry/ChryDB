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

        // POST api/<LoginController>
        [HttpPost]
        public String Post([FromBody] LoginInfo logininfo)
        {
            String a;
            LoginMessage loginMessage = new LoginMessage();
            if (logininfo.UserId != null && logininfo.Password != null)
            {
                loginMessage.errorCode = 200;
            }
            else
            {
                try
                {
                    var user = _Context.Users.Single(b => b.UserId == logininfo.UserId && b.Password == logininfo.Password);

                        loginMessage.data.Add("token", 12345);
                        loginMessage.data.Add("userid", user.UserId);
                        loginMessage.data.Add("username", user.UserName);
                        loginMessage.data.Add("usertype", user.UserType);
                    
                }
                catch
                {
                    loginMessage.errorCode = 11111;

                }
            }
               
            return loginMessage.ReturnJson();
        }

        [HttpPost("student")]
        public string studentLogin()
        {
            LoginMessage loginMessage = new LoginMessage();
            decimal account = decimal.Parse(Request.Form["account"]);
            string password = Request.Form["password"];
            if (account != null && password != null)
            {
                loginMessage.errorCode = 200;
            }
            User student = StudentController.SearchByAccount(account);
            if (StudentController.StudentLogin(student, password))
            {
                loginMessage.data["loginState"] = true;
                loginMessage.data["UserName"] = student.UserName;
                var token = Token.GetToken(new TokenInfo
                {
                    id = account,
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
            var request = Request;
            return loginMessage.ReturnJson();
        }
        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        public class LoginInfo
        {
            public decimal UserId { get; set; }
            public string Password { get; set; }
        }
    }
}
