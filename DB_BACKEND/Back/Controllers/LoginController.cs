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
        public String Post()
        {
            String a;
            CookieOptions cookieOptions = new CookieOptions();
            //cookieOptions.Path = "/";
           // cookieOptions.HttpOnly = false;
            cookieOptions.SameSite = SameSiteMode.None;
            cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
            cookieOptions.Secure = true;
            Request.Cookies.TryGetValue("user_id", out a);
            //if (Request.Headers.TryGetValue("Cookies", out a))
            //    return "aaaa";
            //else
            //    return "bbbb";
            Response.Cookies.Append("user_id","123",cookieOptions);
            return a;
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
    }
}
