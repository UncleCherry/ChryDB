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

        [HttpGet("hh")]
        public static Student SearchByID(decimal id)
        {
            try
            {
                ModelContext context = new ModelContext();
                var student = context.Students.
                    Single(b => b.StudentId == id);
                //var student = _Context.Students.
                //    Single(b => b.StudentId == id);
                return student;
            }
            catch
            {
                return null;
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
                    var sUser = from s in _Context.Students
                                    where s.StudentId == id
                                    select s;
                    Student student = sUser.FirstOrDefault();
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
