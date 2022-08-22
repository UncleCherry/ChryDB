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
    public class ApplicationController : ControllerBase
    {
        private readonly ModelContext _Context;
        public ApplicationController(ModelContext modelContext)
        {
            _Context = modelContext;
        }

        public class  ApplicationInfo
        {
            public decimal ApplicationId { get; set; }
            public decimal? UserId { get; set; }
            public string StudentName { get; set; }
            public decimal? AdminId { get; set; }
            public string Reason { get; set; }
            public DateTime? Time { get; set; }
            public int? Type { get; set; }
            public int? State { get; set; }
        }
        // 获取学生申请信息
        [HttpGet("student")]
        public string GetStudentApplication()
        {
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token学生身份
            {

                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var sUser = from s in _Context.Users
                                where s.UserId == id && s.UserType == 0
                                select s;
                    User student = sUser.FirstOrDefault();
                    if (student != null)
                    {
                        Student stu = _Context.Students.Find(id);
                        if (stu == null)
                        {
                            message.errorCode = 201;//身份验证失败
                            return message.ReturnJson();
                        }
                        //验证学生身份成功
                        //搜索申请
                        var apps = _Context.Applications.Where(x => x.UserId == student.UserId);
                        message.data["ApplicaitionsList"] = apps.Select(a => new ApplicationInfo
                        {
                            ApplicationId=a.ApplicationId,
                            UserId=student.UserId,
                            StudentName=stu.Name,
                            AdminId=a.AdminId,
                            Reason=a.Reason,
                            Time=a.Time,
                            Type=a.Type,
                            State=a.State
                        }).ToList();
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
        // 获取教务负责的申请信息
        [HttpGet("admin")]
        public string GetAdminApplication()
        {
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教务身份
            {

                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var aUser = from a in _Context.Users
                                where a.UserId == id && a.UserType == 2
                                select a;
                    User admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        //验证教务身份成功
                        //搜索申请
                        var apps = _Context.Applications.Where(x => x.AdminId == admin.UserId);
                        var appswithname = from a in apps
                                           join s in _Context.Students on a.UserId equals s.StudentId
                                           select new ApplicationInfo
                                           {
                                               ApplicationId = a.ApplicationId,
                                               UserId = a.UserId,
                                               StudentName = s.Name,
                                               AdminId = a.AdminId,
                                               Reason = a.Reason,
                                               Time = a.Time,
                                               Type = a.Type,
                                               State = a.State
                                           };

                        message.data["ApplicaitionsList"] = appswithname.ToList();
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
        // 学生创建添加申请
        [HttpPost("create")]
        public string PostApplication()
        {

            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token学生身份
            {

                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var sUser = from s in _Context.Users
                                where s.UserId == id && s.UserType == 0
                                select s;
                    User student = sUser.FirstOrDefault();
                    if (student != null)
                    {
                        Student stu = _Context.Students.Find(id);
                        if (stu == null)
                        {
                            message.errorCode = 201;//身份验证失败
                            return message.ReturnJson();
                        }
                        //验证学生身份成功
                        //查找学生对应教务
                        var plan = _Context.TrainingPlans.Find(stu.Major, stu.Grade);//查出该学生的培养计划
                        if (plan == null)
                        {
                            message.errorCode = 204;//培养计划不存在
                            return message.ReturnJson();
                        }
                        var admin = _Context.Admins.Find(plan.AdminId);//查出培养计划对应的教务
                        if(admin==null)
                        {
                            message.errorCode = 205;//教务不存在
                            return message.ReturnJson();
                        }
                        //对Application表增
                        string reason= Request.Form["reason"];
                        int type = int.Parse(Request.Form["type"]);//申请类型
                        
                        DateTime time = DateTime.Now;
                        Application application = new Application();
                        application.UserId = student.UserId;
                        application.AdminId = admin.AdminId;
                        application.Reason = reason;
                        application.Type = type;
                        application.Time = time;
                        application.State = 0;//默认为0，待审核状态
                        try
                        {
                            _Context.Applications.Add(application);
                            _Context.SaveChanges();
                        }
                        catch
                        {
                            message.errorCode = 202;//更新数据库失败
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
        //教务通过申请
        [HttpPut("pass")]
        public string Pass()
        {

            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教务身份
            {

                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var aUser = from a in _Context.Users
                                where a.UserId == id && a.UserType == 2
                                select a;
                    User admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        //验证教务身份成功
                        //修改申请信息
                        decimal applicationid = decimal.Parse(Request.Form["applicationid"]);

                        //查找有无对应申请
                        Application app = _Context.Applications.Find(applicationid);
                        if (app != null)
                        {
                            //对App表改
                            app.State = 1;//状态改为申请成功
                            try
                            {
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
                            message.errorCode = 203;//无对应申请
                            return message.ReturnJson();
                        }
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
        //教务拒绝申请
        [HttpPut("reject")]
        public string Reject()
        {

            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教务身份
            {

                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var aUser = from a in _Context.Users
                                where a.UserId == id && a.UserType == 2
                                select a;
                    User admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        //验证教务身份成功
                        //修改申请信息
                        decimal applicationid = decimal.Parse(Request.Form["applicationid"]);

                        //查找有无对应申请
                        Application app = _Context.Applications.Find(applicationid);
                        if (app != null)
                        {
                            //对App表改
                            app.State = 2;//状态改为申请失败
                            try
                            {
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
                            message.errorCode = 203;//无对应申请
                            return message.ReturnJson();
                        }
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
