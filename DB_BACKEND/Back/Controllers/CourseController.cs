using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ModelContext _Context;
        public CourseController(ModelContext modelContext)
        {
            _Context = modelContext;
        }
        private class CourseInfo
        {
            public decimal CourseId { get; set; }
            public string CourseName { get; set; }
            public string TimeSlot { get; set; }
            public byte? Credit { get; set; }
            public bool? IsRequired { get; set; }
            public string Year { get; set; }
            public string Semester { get; set; }
            public int? MeetingId { get; set; }
        }
        // 获取所有课程信息
        [HttpGet("all")]
        public string GetAllCourses()
        {
            Message message = new Message();
            message.data.Add("CoursesList", new List<Course>());
            message.data["CoursesList"]=_Context.Courses.Select(c=>new CourseInfo
            {
                CourseId=c.CourseId,
                CourseName=c.CourseName,
                TimeSlot=c.TimeSlot,
                Credit=c.Credit,
                IsRequired=c.IsRequired,
                Year=c.Year,
                Semester=c.Semester,
                MeetingId=c.MeetingId
            }).ToList();
            message.errorCode = 200;
            return message.ReturnJson();
        }
        //根据id获取单个课程信息
        [HttpGet("getinfo")]
        public string GetCourseInfo()
        {

            Message message = new Message();
            CourseInfo cinfo = new CourseInfo();
            decimal courseid = decimal.Parse(Request.Form["courseid"]);
            var course=_Context.Courses.Find(courseid);
            //查找有无对应课程
            if (course != null)
            {
                cinfo.CourseId = course.CourseId;
                cinfo.CourseName = course.CourseName;
                cinfo.TimeSlot = course.TimeSlot;
                cinfo.Credit = course.Credit;
                cinfo.IsRequired = course.IsRequired;
                cinfo.Year = course.Year;
                cinfo.Semester = course.Semester;
                cinfo.MeetingId = course.MeetingId;
                message.data.Add("course", cinfo);
                message.errorCode = 200;
                return message.ReturnJson();
            }
            else
            {
                message.errorCode = 203;//无对应课程
                return message.ReturnJson();
            }
            return message.ReturnJson();
        }
        // 获取学生选课信息
        [HttpGet("student")]
        public string GetStudentCourses()
        {
            Message message = new Message();
            message.errorCode = 300;
            message.data.Add("CoursesList", new List<Course>());
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
                        //搜索课表
                        var courses = from t in _Context.Takes
                                      join c in _Context.Courses
                                      on t.CourseId equals c.CourseId
                                      where t.StudentId == student.UserId
                                      select c;
                        message.data["CoursesList"] = courses.Select(c => new CourseInfo
                        {
                            CourseId = c.CourseId,
                            CourseName = c.CourseName,
                            TimeSlot = c.TimeSlot,
                            Credit = c.Credit,
                            IsRequired = c.IsRequired,
                            Year = c.Year,
                            Semester = c.Semester,
                            MeetingId = c.MeetingId
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
        // 获取教师授课信息
        [HttpGet("instructor")]
        public string GetInstructorCourses()
        {
            Message message = new Message();
            message.errorCode = 300;
            message.data.Add("CoursesList", new List<Course>());
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师身份
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
                        //验证教师身份成功
                        //搜索课表
                        var courses = from i in _Context.Instructs
                                      join c in _Context.Courses
                                      on i.CourseId equals c.CourseId
                                      where i.InstructorId == ins.UserId
                                      select c;
                        message.data["CoursesList"] = courses.Select(c => new CourseInfo
                        {
                            CourseId = c.CourseId,
                            CourseName = c.CourseName,
                            TimeSlot = c.TimeSlot,
                            Credit = c.Credit,
                            IsRequired = c.IsRequired,
                            Year = c.Year,
                            Semester = c.Semester,
                            MeetingId = c.MeetingId
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
        // 教务创建添加课程
        [HttpPost("create")]
        public string PostCourse()
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
                        //添加课程信息
                        string coursename = Request.Form["coursename"];
                        string timeslot = Request.Form["timeslot"];
                        byte credit = byte.Parse(Request.Form["credit"]);
                        bool isrequired = bool.Parse(Request.Form["isrequired"]);
                        string year = Request.Form["year"];
                        string semester = Request.Form["semester"];
                        Course course = new Course();
                        course.CourseName = coursename;
                        course.TimeSlot = timeslot;
                        course.Credit = credit;
                        course.IsRequired = isrequired;
                        course.Year = year;
                        course.Semester = semester;
                        try
                        {
                            _Context.Courses.Add(course);
                            _Context.SaveChanges();
                        }catch
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
        //老师选择课程来教授
        [HttpPost("instruct")]
        public string InstructCourse()
        {
            ModelContext context = new ModelContext();
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师身份
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
                        //验证教师身份成功

                        decimal courseid = decimal.Parse(Request.Form["courseid"]);

                        //查找有无对应课程
                        if (_Context.Courses.Find(courseid)!=null)
                        {
                            //对instruct表增
                            Instruct instruct = new Instruct();
                            instruct.CourseId = courseid;
                            instruct.InstructorId = ins.UserId;
                            try {
                                _Context.Instructs.Add(instruct);
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
                            message.errorCode = 203;//无对应课程
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

        //老师不教课程
        [HttpDelete("uninstruct")]
        public string UnInstructCourse()
        {
            ModelContext context = new ModelContext();
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师身份
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
                        //验证教师身份成功

                        decimal courseid = decimal.Parse(Request.Form["courseid"]);

                        //查找Instruct对应记录
                        Instruct instruct = _Context.Instructs.Find(courseid, ins.UserId);
                        if ( instruct!= null)
                        {
                            //对instruct表删
                            try
                            {
                                _Context.Instructs.Remove(instruct);
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
                            message.errorCode = 203;//无对应记录
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
        //学生选择课程
        [HttpPost("take")]
        public string TakeCourse()
        {
            ModelContext context = new ModelContext();
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
                        //验证学生身份成功

                        decimal courseid = decimal.Parse(Request.Form["courseid"]);

                        //查找有无对应课程
                        if (_Context.Courses.Find(courseid) != null)
                        {
                            //对take表增
                            Take take = new Take();
                            take.CourseId = courseid;
                            take.StudentId = student.UserId;
                            try
                            {
                                _Context.Takes.Add(take);
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
                            message.errorCode = 203;//无对应课程
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
        //学生退课程
        [HttpDelete("drop")]
        public string DropCourse()
        {
            ModelContext context = new ModelContext();
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
                        //验证学生身份成功

                        decimal courseid = decimal.Parse(Request.Form["courseid"]);

                        //查找take对应记录
                        Take take = _Context.Takes.Find(courseid, student.UserId);
                        if (take != null)
                        {
                            //对take表删
                            try
                            {
                                _Context.Takes.Remove(take);
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
                            message.errorCode = 203;//无对应记录
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
