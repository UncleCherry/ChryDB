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
                                                 Grade=g.Grade_
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
    }
}
