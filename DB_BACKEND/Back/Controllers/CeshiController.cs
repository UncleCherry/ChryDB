using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Back.Entity;
namespace Back.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class CeshiController : ControllerBase
    {
        [HttpGet]
        public List<User> GetValue()
        {
            List<User> us = new List<User>(); 
            using (ModelContext context = new ModelContext())
            {
                us=context.Users.ToList();
            }
                return us;
        }
    }
}
