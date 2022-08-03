using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ModelContext _Context;
        public UserController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete]
        public string Delete()
        {
            Message message = new Message();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    User user = _Context.Users.SingleOrDefault(x => x.UserId == id);
                    if (user == null)
                    {
                        message.errorCode = 201;//没有对应id的用户
                        return message.ReturnJson();
                    }
                    else {
                        _Context.Users.Remove(user);
                        _Context.SaveChanges();
                        message.errorCode = 200;
                        return message.ReturnJson();
                    }
                }
            }
            return message.ReturnJson();
        }
    }
}
