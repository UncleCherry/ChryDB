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
    public class InstructorController : ControllerBase
    {
        private  static ModelContext _Context;
        public InstructorController(ModelContext modelContext)
        {
            _Context = modelContext;
        }

        [HttpGet("Info")]

        public string GetInstructorInfo()
        {
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var iUser = from i in _Context.Instructors
                                    where i.InstructorId == id
                                    select i;
                    Instructor ins = iUser.FirstOrDefault();
                    if (ins != null)
                    {
                        message.errorCode = 200;
                        message.data["instructorID"] = ins.InstructorId;
                        message.data["instructorDepartment"] = ins.Department;
                        message.data["instructorName"] = ins.Name;
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
    }
}
