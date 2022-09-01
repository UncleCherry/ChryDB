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
    public class AdminController : ControllerBase
    {
        private  static ModelContext _Context;
        public AdminController(ModelContext modelContext)
        {
            _Context = modelContext;
        }

        [HttpGet("Info")]

        public string GetAdminInfo()
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
                    var aUser = from a in _Context.Admins
                                    where a.AdminId == id
                                    select a;
                    Admin admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        message.data["adminID"] = admin.AdminId;
                        message.data["adminDepartment"] = admin.Department;
                        message.data["adminName"] = admin.Name;
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
