using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ModelContext _Context;
        public CourseController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        // GET: api/<CourseController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CourseController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // 教师添加课程
        [HttpPost]
        public string PostCourse()
        {

            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师身份
            {

                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var iUser = from i in _Context.Users
                                where i.UserId == id && i.UserType == 1
                                select i;
                    User ins = iUser.FirstOrDefault();
                    if (ins != null)
                    {
                        //验证教师身份成功
                        //添加课程信息
                        string coursename = Request.Form["coursename"];
                        string timeslot = Request.Form["timeslot"];
                        byte credit = byte.Parse(Request.Form["credit"]);
                        bool isrequired = bool.Parse(Request.Form["isrequired"]);
                        string year = Request.Form["year"];
                        string semester = Request.Form["semester"];
                        Course course = new Course();
                        course.CourseName = coursename;
                        course.TimeSlot = timeslot;
                        course.Credit = credit;
                        course.IsRequired = isrequired;
                        course.Year = year;
                        course.Semester = semester;
                        _Context.Courses.Add(course);
                        _Context.SaveChanges();
                        message.errorCode = 200;
                        return message.ReturnJson();
                    }
                    else
                    {
                        message.errorCode = 201;
                        return message.ReturnJson();

                    }
                }
            }
            return message.ReturnJson();
        }

        // PUT api/<CourseController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CourseController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}
