using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ModelContext _Context;
        public RegisterController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        [HttpPost]
        public string UserRegister()
        {
            Message message = new Message();
            message.errorCode = 200;
            decimal id = decimal.Parse(Request.Form["id"]);
            string password = Request.Form["password"];
            string username = Request.Form["username"];
            if (_Context.Users.Find(id)!=null)//id已存在 =====这里需求不太清楚
            {
                message.errorCode = 400;
                return message.ReturnJson();
            }
            User user = new User();
            user.UserId = id;
            user.Password = password;
            user.UserName = username;
            _Context.Users.Add(user);
            _Context.SaveChanges();
            return message.ReturnJson();
        }
        // GET: api/<RegisterController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RegisterController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RegisterController>
       

        // PUT api/<RegisterController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RegisterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
