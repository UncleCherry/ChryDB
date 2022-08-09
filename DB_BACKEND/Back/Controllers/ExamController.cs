using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly ModelContext _Context;
        public ExamController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        private class ExamInfo
        {
            public decimal ExamId { get; set; }
            public decimal? CourseId { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public int? MeetingId { get; set; }
            public string CourseName { get; set; }
        }
        // 获取所有考试信息
        [HttpGet("all")]
        public string GetAllExams()
        {
            Message message = new Message();
            message.data.Add("ExamsList", new List<ExamInfo>());
            var exams = from e in _Context.Exams
                        join c in _Context.Courses
                        on e.CourseId equals c.CourseId
                        select new ExamInfo{
                            ExamId = e.ExamId,
                            CourseId = e.CourseId,
                            StartTime = e.StartTime,
                            EndTime = e.EndTime,
                            MeetingId = e.MeetingId,
                            CourseName=c.CourseName
                        };
            /*
            message.data["ExamsList"] = _Context.Exams.Select(e => new ExamInfo
            {
                ExamId = e.ExamId,
                CourseId =e.CourseId,
                StartTime=e.StartTime,
                EndTime=e.EndTime,
                MeetingId=e.MeetingId
            }).ToList();*/
            message.data["ExamsList"] = exams.ToList();
            return message.ReturnJson();
        }

        // 教务创建添加考试
        [HttpPost("create")]
        public string PostExam()
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
                        //添加考试信息
                        decimal courseid= Decimal.Parse(Request.Form["courseid"]);
                        int meetingid = int.Parse(Request.Form["meetingid"]);
                        // 格式：2022-08-08T21:00:00
                        DateTime starttime = DateTime.Parse(Request.Form["starttime"]);
                        DateTime endtime = DateTime.Parse(Request.Form["endtime"]);
                        Exam exam = new Exam();
                        exam.CourseId = courseid;
                        exam.StartTime = starttime;
                        exam.EndTime = endtime;
                        exam.MeetingId = meetingid;
                        try
                        {
                            _Context.Exams.Add(exam);
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
