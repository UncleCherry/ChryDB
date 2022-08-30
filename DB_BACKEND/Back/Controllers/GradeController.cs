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
        // 老师修改成绩
        [HttpPut("alt")]
        public string AltGrade()
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
                        //修改成绩信息
                        decimal examid = Decimal.Parse(Request.Form["examid"]);
                        decimal studentid = Decimal.Parse(Request.Form["studentid"]);
                        byte examgrade = byte.Parse(Request.Form["grade"]);
                        //查找有无对应成绩记录
                        Grade grade = _Context.Grades.Find(examid,studentid);
                        if (grade != null)
                        {
                            //对grade表改
                            grade.ExamId = examid;
                            grade.StudentId = studentid;
                            grade.Grade_ = examgrade;
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
        // 老师删除成绩
        [HttpDelete("del")]
        public string DelGrade()
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
                        //删除成绩记录
                        decimal examid = Decimal.Parse(Request.Form["examid"]);
                        decimal studentid = Decimal.Parse(Request.Form["studentid"]);
                        //查找有无对应成绩记录
                        Grade grade = _Context.Grades.Find(examid, studentid);
                        if (grade != null)
                        {
                            //对grade表删
                            try
                            {
                                _Context.Remove(grade);
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

        //带课程名的成绩信息
        private class GradeInfo
        {
            public decimal? CourseId { get; set; }
            public string CourseName { get; set; }
            public decimal ExamId { get; set; }
            public decimal StudentId { get; set; }
            public string StudentName { get; set; }
            public string Year { get; set; }
            public string Semester { get; set; }
            public byte? Grade { get; set; }
            public byte? Credit { get; set; }
        }
        //考试信息
        private class ExamInfo
        {
            public decimal ExamId { get; set; }
            public decimal? CourseId { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public int? MeetingId { get; set; }
            public string Year { get; set; }
            public string Semester { get; set; }
            public string CourseName { get; set; }
        }
        //每场考试考生信息，包含考试id,课名，课id
        private class StudentInfo
        {
            public decimal StudentId { get; set; }
            public string Name { get; set; }
            public decimal ExamId { get; set; }
            public decimal? CourseId { get; set; }
            public string CourseName { get; set; }
            public string Year { get; set; }
            public string Semester { get; set; }
        }
        //每场考试考生信息，包含成绩
        private class StudentInfoWithGrade
        {
            public decimal StudentId { get; set; }
            public string Name { get; set; }
            public decimal ExamId { get; set; }
            public decimal? CourseId { get; set; }
            public string CourseName { get; set; }
            public string Year { get; set; }
            public string Semester { get; set; }
            public byte? Grade { get; set; }
        }
        //学生获取所有成绩信息
        [HttpGet("student")]
        public string StudentGrades()
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
                        if (stu == null) {
                            message.errorCode = 201;//身份验证失败
                            return message.ReturnJson();
                        }
                        //验证学生身份成功
                        var grades = _Context.Grades.Where(g => g.StudentId == id);//查出该学生的所有成绩
                        var gradeswithname = from g in grades
                                             join e in _Context.Exams
                                             on g.ExamId equals e.ExamId
                                             join c in _Context.Courses
                                             on e.CourseId equals c.CourseId
                                             select new GradeInfo
                                             {
                                                 CourseId = c.CourseId,
                                                 CourseName=c.CourseName,
                                                 ExamId=e.ExamId,
                                                 StudentId=stu.StudentId,
                                                 StudentName=stu.Name,
                                                 Grade=g.Grade_,
                                                 Year=c.Year,
                                                 Semester=c.Semester,
                                                 Credit=c.Credit
                                             };
                        message.errorCode = 200;
                        message.data["GradesList"] = gradeswithname.ToList();
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
        //老师获取所有自己课程考试信息
        [HttpGet("ins_exam")]
        public string InsCourseExam()
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
                        //查出该老师课程的所有考试
                        var exams = from e in _Context.Exams
                                    join i in _Context.Instructs
                                    on e.CourseId equals i.CourseId
                                    where i.InstructorId == ins.UserId
                                    select e;
                        var examswithname=from e in exams
                                          join c in _Context.Courses
                                          on e.CourseId equals c.CourseId
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

                        message.errorCode = 200;
                        message.data["ExamsList"] = examswithname.ToList();
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
        //获取某一考试所有考生信息
        [HttpGet("studentsinexam")]
        public string StudentsInCourse(decimal examid=-1)
        {
            Message message = new Message();
            message.errorCode = 300;
            var exam = _Context.Exams.Find(examid);
            if(exam==null)
            {
                message.errorCode = 202;//课程考试不存在
                return message.ReturnJson();
            }
            var course = _Context.Courses.Find(exam.CourseId);
            if(course==null)
            {
                message.errorCode = 202;//课程考试不存在
                return message.ReturnJson();
            }
            var students = from s in _Context.Students
                           join t in _Context.Takes on s.StudentId equals t.StudentId
                           where t.CourseId==course.CourseId
                           select new StudentInfo 
                           { 
                               StudentId=s.StudentId,
                               Name=s.Name,
                               ExamId=exam.ExamId,
                               CourseId=course.CourseId,
                               CourseName=course.CourseName,
                               Year = course.Year,
                               Semester = course.Semester
                           };

            var studentswithgrade = from s in students
                                    join g in _Context.Grades on s.StudentId equals g.StudentId into grouping //left join 写法 in linq
                                    from p in grouping.DefaultIfEmpty()
                                    select new StudentInfoWithGrade
                                    {
                                        StudentId = s.StudentId,
                                        Name = s.Name,
                                        ExamId = s.ExamId,
                                        CourseId = s.CourseId,
                                        CourseName = s.CourseName,
                                        Year = s.Year,
                                        Semester = s.Semester,
                                        Grade =p.Grade_
                                    };
            message.errorCode = 200;
            message.data["StudentsList"] = studentswithgrade.ToList();
            return message.ReturnJson();
        }
    }
}