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
            public string Year { get; set; }
            public string Semester { get; set; }
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
                            CourseName=c.CourseName,
                            Year=c.Year,
                            Semester=c.Semester
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
            message.errorCode = 200;
            return message.ReturnJson();
        }
        //根据id获取单个考试信息
        [HttpGet("getinfo")]
        public string GetExamInfo(decimal examid=-1)
        {

            Message message = new Message();
            ExamInfo einfo = new ExamInfo();
            var exam = _Context.Exams.Find(examid);
            //查找有无对应考试
            if (exam != null)
            {
                var examinfo = from c in _Context.Courses
                            select new ExamInfo
                            {
                                ExamId = exam.ExamId,
                                CourseId = exam.CourseId,
                                StartTime = exam.StartTime,
                                EndTime = exam.EndTime,
                                MeetingId = exam.MeetingId,
                                CourseName = c.CourseName,
                                Year = c.Year,
                                Semester = c.Semester
                            };
                einfo = examinfo.FirstOrDefault();
                message.data.Add("exam", einfo);
                message.errorCode = 200;
                return message.ReturnJson();
            }
            else
            {
                message.errorCode = 203;//无对应考试
                return message.ReturnJson();
            }
        }
        //获取学生考试信息
        [HttpGet("student")]
        public string GetStudentExams()
        {
            Message message = new Message();
            message.errorCode = 300;
            message.data.Add("ExamsList", new List<ExamInfo>());
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
                        //验证学生身份成功
                        //搜索考试表
                        var exams= from t in _Context.Takes
                                   join e in _Context.Exams
                                   on t.CourseId equals e.CourseId
                                   join c in _Context.Courses
                                   on e.CourseId equals c.CourseId
                                   where t.StudentId == student.UserId
                                   select new ExamInfo
                                   {
                                       ExamId = e.ExamId,
                                       CourseId = e.CourseId,
                                       StartTime = e.StartTime,
                                       EndTime = e.EndTime,
                                       MeetingId = e.MeetingId,
                                       CourseName = c.CourseName,
                                       Year = c.Year,
                                       Semester = c.Semester
                                   };
                        message.data["ExamsList"] = exams.ToList();
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
        // 教务修改考试
        [HttpPut("alt")]
        public string AltExam()
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
                        //修改考试信息
                        decimal examid = decimal.Parse(Request.Form["examid"]);

                        //查找有无对应考试
                        Exam exam = _Context.Exams.Find(examid);
                        if (exam!= null)
                        {
                            //对Exam表改
                            //decimal courseid = Decimal.Parse(Request.Form["courseid"]);
                            int meetingid = int.Parse(Request.Form["meetingid"]);
                            // 格式：2022-08-08T21:00:00
                            DateTime starttime = DateTime.Parse(Request.Form["starttime"]);
                            DateTime endtime = DateTime.Parse(Request.Form["endtime"]);
                            //exam.CourseId = courseid;
                            exam.StartTime = starttime;
                            exam.EndTime = endtime;
                            exam.MeetingId = meetingid;
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
                            message.errorCode = 203;//无对应考试
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

        // 教务删除考试
        [HttpDelete("del")]
        public string DelExam()
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
                        //删除考试信息
                        decimal examid = decimal.Parse(Request.Form["examid"]);

                        //查找有无对应考试
                        Exam exam = _Context.Exams.Find(examid);
                        if (exam != null)
                        {
                            try
                            {
                                _Context.Exams.Remove(exam);
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
                            message.errorCode = 203;//无对应考试
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
