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
            try
            {
                var user = _Context.Users.Single(b => b.UserId == logininfo.UserId && b.Password ==logininfo.Password);
                else
                {
                    loginMessage.data.Add("token", 12345);
                    loginMessage.data.Add("userid", user.UserId);
                    loginMessage.data.Add("username", user.UserName);
                    loginMessage.data.Add("usertype", user.UserType);
                }
            }
            catch
            {
                loginMessage.errorCode = 11111;
                
            }
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
