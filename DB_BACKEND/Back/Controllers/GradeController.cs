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
    public class GradeController : ControllerBase
    {
        private readonly ModelContext _Context;
        public GradeController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        // 老师创建添加成绩
        [HttpPost("create")]
        public string PostGrade()
        {

            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token老师身份
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
                        //验证老师身份成功
                        //添加成绩信息
                        decimal examid = Decimal.Parse(Request.Form["examid"]);
                        decimal studentid = Decimal.Parse(Request.Form["studentid"]);
                        byte examgrade = byte.Parse(Request.Form["grade"]);
                        Grade grade = new Grade();
                        grade.ExamId = examid;
                        grade.StudentId = studentid;
                        grade.Grade_ = examgrade;
                        try
                        {
                            _Context.Grades.Add(grade);
                            _Context.SaveChanges();
                        }
                        catch
                        {
                            message.errorCode = 202;//数据库更新失败
                            return message.ReturnJson();
                        }
                        message.errorCode = 200;
                        return message.ReturnJson();
                    }
                    else
                    {
                        message.errorCode = 201;//身份验证失败
                        return message.ReturnJson();

                    }
                }
            }
            return message.ReturnJson();
        }
    }
}
