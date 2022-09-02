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

    public class LeaveController : ControllerBase
    {
        private readonly ModelContext _Context;

        public LeaveController(ModelContext modelContext)
        {
            _Context = modelContext;
        }

        public class ApplicationInfo
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
                        if (admin == null)
                        {
                            message.errorCode = 205;//教务不存在
                            return message.ReturnJson();
                        }
                        //对Application表增
                        string reason = Request.Form["reason"];
                        string leavereason = Request.Form["courseid"] + "-" + Request.Form["number"] + "-" + reason;
                        DateTime time = DateTime.Now;
                        Application application = new Application();
                        application.UserId = student.UserId;
                        application.AdminId = admin.AdminId;
                        application.Reason = leavereason;
                        application.Type = 5;   //请假
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

        [HttpDelete("DeleteInfo")]
        public string DeleteLeaveInfo()
        {
            Message message = new Message();
            decimal leaveid = decimal.Parse(Request.Form["applicationid"]);
            Application app = _Context.Applications.Find(leaveid);
            if (app != null)
            {
                try
                {
                    _Context.Applications.Remove(app);
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
                message.errorCode = 203;//无对应考勤信息
                return message.ReturnJson();
            }
        }

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
                        var apps = _Context.Applications.Where(x => x.UserId == student.UserId && x.Type==5);//寻找请假信息
                        message.data["ApplicationsList"] = apps.Select(a => new ApplicationInfo
                        {
                            ApplicationId = a.ApplicationId,
                            UserId = student.UserId,
                            StudentName = stu.Name,
                            AdminId = a.AdminId,
                            Reason = a.Reason,
                            Time = a.Time,
                            Type = a.Type,
                            State = a.State
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
                        var apps = _Context.Applications.Where(x => x.AdminId == admin.UserId && x.Type==5);//查询请假信息
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

                        message.data["ApplicationsList"] = appswithname.ToList();
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

        [HttpGet("getApplicationInfo")]
        public string getApplicationInfo()
        {
            Message message = new Message();
            message.data.Add("ApplicationsList", new List<Application>());
            var app = from a in _Context.Applications
                      where decimal.Parse(Request.Query["leaveid"]) == a.ApplicationId
                      select a;
            message.data["ApplicationsList"] = app.ToList();
            message.errorCode = 200;
            return message.ReturnJson();
        }
    }

}
