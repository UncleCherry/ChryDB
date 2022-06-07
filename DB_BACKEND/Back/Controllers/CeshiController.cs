using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Back.Entity;
using Microsoft.AspNetCore.Cors;

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CeshiController : ControllerBase
    {
        private readonly ModelContext _Context;
        public CeshiController(ModelContext modelContext)
        {
            _Context = modelContext;
            //_Context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
        }
        [HttpGet]
        //[HttpPost]
        public List<User> GetValue()
        {
            List<User> us = new List<User>();
            us=_Context.Users.ToList();
           // us = _Context.Users.Where(x => x.UserId == {id}).ToList();
            //us[0].UserName = "ori";
            //_Context.Users.Update(us[0]);
           // _Context.SaveChanges();
            return us;
        }
    }
}
