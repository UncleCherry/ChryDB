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
    public class PlanController : ControllerBase
    {
        private readonly ModelContext _Context;
        public PlanController(ModelContext modelContext)
        {
            _Context = modelContext;
        }

        public class PlanInfo {
            public string Major { get; set; }
            public decimal Grade { get; set; }
            public string Info { get; set; }
            public decimal? AdminId { get; set; }
        }
        // 获取学生专业培养计划信息
        [HttpGet("student")]
        public string GetPlanInfo()
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
                        var plan = _Context.TrainingPlans.Find(stu.Major,stu.Grade);//查出该学生的培养计划
                        if (plan != null)
                        {
                            PlanInfo planinfo = new PlanInfo();
                            planinfo.Major = plan.Major;
                            planinfo.Grade = plan.Grade;
                            planinfo.Info = plan.Info;
                            planinfo.AdminId = plan.AdminId;
                            message.errorCode = 200;
                            message.data["TrainingPlan"] = planinfo;
                            return message.ReturnJson();
                        }
                        else {
                            message.errorCode = 203;//没有培养计划
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
        // 教务创建添加培养计划
        [HttpPost("create")]
        public string PostPlan()
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
                        //添加培养计划信息
                        decimal grade = Decimal.Parse(Request.Form["grade"]);
                        string major = Request.Form["major"];
                        string info= Request.Form["info"];
                        TrainingPlan plan = new TrainingPlan();
                        plan.Major = major;
                        plan.Grade = grade;
                        plan.Info = info;
                        plan.AdminId = admin.UserId;
                        try
                        {
                            _Context.TrainingPlans.Add(plan);
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
        // 教务修改培养计划
        [HttpPut("alt")]
        public string AltPlan()
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
                        //修改培养计划信息
                        decimal grade = Decimal.Parse(Request.Form["grade"]);
                        string major = Request.Form["major"];
                        //查找有无对应培养计划
                        TrainingPlan plan = _Context.TrainingPlans.Find(major,grade);
                        if (plan != null)
                        {
                            string info = Request.Form["info"];
                            plan.Info = info;
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
                            message.errorCode = 203;//无对应培养计划
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
