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
        [HttpPost("student")]
        public string StudentRegister()
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
            Student student = new Student();
            user.UserId = id;
            user.Password = password;
            user.UserName = username;
            user.UserType = 0;//0为学生
            student.StudentId = id;
            student.Name = username;
            _Context.Users.Add(user);
            _Context.Students.Add(student);
            _Context.Users.Add(user);
            _Context.SaveChanges();
            return message.ReturnJson();
        }
        [HttpPost("instructor")]
        public string InstructorRegister()
        {
            Message message = new Message();
            message.errorCode = 200;
            decimal id = decimal.Parse(Request.Form["id"]);
            string password = Request.Form["password"];
            string username = Request.Form["username"];
            string department = Request.Form["department"];
            if (_Context.Users.Find(id) != null)//id已存在 =====这里需求不太清楚
            {
                message.errorCode = 400;
                return message.ReturnJson();
            }
            User user = new User();
            Instructor instructor = new Instructor();
            user.UserId = id;
            user.Password = password;
            user.UserName = username;
            user.UserType = 1;//1为教师
            instructor.InstructorId = id;
            instructor.Name = username;
            instructor.Department = department;
            _Context.Users.Add(user);
            _Context.Instructors.Add(instructor);
            _Context.SaveChanges();
            return message.ReturnJson();
        }
    }
}
