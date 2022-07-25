using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private  static ModelContext _Context;
        public StudentController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        public static User SearchByAccount(decimal account)
        {
            try
            {
                //ModelContext context = new ModelContext();
                var student = _Context.Users.
                    Single(b => b.UserId == account);
                return student;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet("hh")]
        public static Student SearchByID(decimal id)
        {
            try
            {
                //ModelContext context = new ModelContext();
                var student = _Context.Students.
                    Single(b => b.StudentId == id);
                return student;
            }
            catch
            {
                return null;
            }
        }

        public static bool StudentLogin(User student, string password)
        {
            try
            {
                if (student == null)
                {
                    return false;
                }
                return student.Password == password;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("Info")]

        public string GetStudentInfo()
        {
            StudentInfoMessage StudentMessage = new StudentInfoMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                StudentMessage.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var student = SearchByID(id);
                    if (student != null)
                    {
                        StudentMessage.errorCode = 200;
                        StudentMessage.data["studentID"] = student.StudentId;
                        StudentMessage.data["studentName"] = student.Name;
                        StudentMessage.data["studentGrade"] = student.Grade;
                        StudentMessage.data["studentMajor"] = student.Major;
                        StudentMessage.data["studentCredit"] = student.Credit;
                    }

                }
            }
            return StudentMessage.ReturnJson();
        }
    }
}
