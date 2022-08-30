using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;



namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ModelContext _Context;
        public AttendanceController(ModelContext modelContext)
        {
            _Context = modelContext;
        }

        private class AttendanceInfo
        {
            public decimal? CourseId { get; set; }
            public string CourseName { get; set; }
            public int CourseNumber { get; set; }
            public string Name { get; set; }
            public decimal? StudentId { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            //public string StartTime { get; set; }
            //public string EndTime { get; set; }
            public bool? IsEffective { get; set; }

        }

        private class AttendanceGrade
        {
            public decimal? CourseId { get; set; }
            public string CourseName { get; set; }
            public string Name { get; set; }
            public decimal? StudentId { get; set; }
            public decimal grade { get; set; }
            public decimal absence { get; set; }

        }



        [HttpPost("insertInfo")]

        public string PostAttendance([FromBody] List<Attendance> list)
        {
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师身份
            {
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var aUser = from a in _Context.Users
                                where a.UserId == id && a.UserType == 1  //1为教师
                                select a;
                    User admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        //验证教师身份成功
                        //添加考勤信息
                        Attendance att = new Attendance();
                        for (int i = 0; i<list.Count; i++)
                        {
                            att.CourseId = list[i].CourseId;
                            att.StudentId = list[i].StudentId;
                            att.StartTime = list[i].StartTime;
                            att.EndTime = list[i].EndTime;
                            att.IsEffective = list[i].IsEffective;
                            att.CourseNumber = list[i].CourseNumber;
                            try
                            {
                                _Context.Attendances.Add(att);
                                _Context.SaveChanges();
                            }
                            catch
                            {
                                message.errorCode = 202;//数据库更新失败
                                return message.ReturnJson();
                            }
                        }
                        if(list.Count==0)
                        {
                            message.errorCode = 200;
                            return message.ReturnJson();
                        }
                        decimal courseid = list[0].CourseId;
                        int number = list[0].CourseNumber;

                        var notPresent = from t in _Context.Takes
                                         where t.CourseId == courseid
                                         select new Attendance
                                         {
                                             CourseId = t.CourseId,
                                             CourseNumber = number,
                                             StudentId = t.StudentId,
                                             StartTime = null,
                                             EndTime = null,
                                             IsEffective = false //考勤无效
                                         };
                        var np = notPresent.ToList();
                        for (int i = 0; i<np.Count; i++)
                        {
                            bool flag = false;
                            for (int j = 0; j<list.Count; j++)
                            {
                                if (np[i].StudentId == list[j].StudentId)
                                    flag = true;
                            }
                            if (flag == false)
                            {
                                try
                                {
                                    _Context.Attendances.Add(np[i]);
                                    _Context.SaveChanges();
                                }
                                catch
                                {
                                    message.errorCode = 203;//数据库更新失败
                                    return message.ReturnJson();
                                }
                            }
                            flag = true;
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

        [HttpGet("getInfo")]
        public string GetAttendanceInfo()
        {
            Message message = new Message();
            message.data.Add("AttendanceList", new List<AttendanceInfo>());
            var Attendances = from a in _Context.Attendances
                              join s in _Context.Students on a.StudentId equals s.StudentId
                              join c in _Context.Courses on a.CourseId equals c.CourseId
                              where (Request.Query["courseid"]==""||a.CourseId == decimal.Parse(Request.Query["courseid"])) 
                              && (Request.Query["number"]==""||a.CourseNumber == int.Parse(Request.Query["number"]))
                              && (Request.Query["studentid"]==""||a.StudentId== decimal.Parse(Request.Query["studentid"]))
                              select new AttendanceInfo
                              {
                                  CourseId = a.CourseId,
                                  CourseName = c.CourseName,
                                  CourseNumber = a.CourseNumber,
                                  StudentId = a.StudentId,
                                  Name = s.Name,
                                  StartTime = a.StartTime,
                                  EndTime = a.EndTime,
                                  IsEffective = a.IsEffective
                              };
            message.data["AttendanceList"] = Attendances.ToList();
            return message.ReturnJson();
        }

        [HttpGet("getGrade")]
        public string GetAttendanceGrade()
        {
            Message message = new Message();
            message.data.Add("AttendanceGradeList", new List<AttendanceGrade>());

            var temp = (from a in _Context.Attendances
                        join s in _Context.Students on a.StudentId equals s.StudentId
                        join c in _Context.Courses on a.CourseId equals c.CourseId
                        where (Request.Query["courseid"] == "" || a.CourseId == decimal.Parse(Request.Query["courseid"]))
                        && (Request.Query["studentid"] == "" || a.StudentId == decimal.Parse(Request.Query["studentid"]))
                        select new
                        {
                            CourseId = a.CourseId,
                            CourseName = c.CourseName,
                            StudentId = a.StudentId,
                            Name = s.Name,
                            CourseNumber = a.CourseNumber,
                            IsEffective = a.IsEffective
                        });



            var AttendanceGrade = (from a in temp
                                   where a!=null
                                   group a by new
                                   {
                                       CourseId = a.CourseId,
                                       StudentId = a.StudentId,
                                       CourseName = a.CourseName,
                                       Name = a.Name
                                   }into g
                                   select new AttendanceGrade
                                   {
                                       CourseId = g.Key.CourseId,
                                       CourseName = g.Key.CourseName,
                                       Name = g.Key.Name,
                                       StudentId = g.Key.StudentId,
                                       absence = (g == null ? 0 : g.Count(a => (bool)!a.IsEffective)),
                                       grade = (g == null ? 0 : (100 - 10 * g.Count(a => (bool)!a.IsEffective)))        //缺勤一次扣十分
                                   });
            message.data["AttendanceGradeList"] = AttendanceGrade;
            return message.ReturnJson();
        }

        [HttpPut("ModifyInfo")]

        public string ModifyAttendanceInfo()
        {
            Message message = new Message();
            decimal courseid = decimal.Parse(Request.Form["courseid"]);
            decimal studentid = decimal.Parse(Request.Form["studentid"]);
            decimal coursenumber = decimal.Parse(Request.Form["coursenumber"]);
            Attendance att = _Context.Attendances.Find(courseid, studentid, coursenumber);
            if (att != null)
            {
                bool isEffective = bool.Parse(Request.Form["isEffective"]);
                att.IsEffective = isEffective;
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
                message.errorCode = 203;//无对应考勤信息
                return message.ReturnJson();
            }
        }

        [HttpDelete("DeleteInfo")]
        public string DeleteAttendanceInfo()
        {
            Message message = new Message();
            decimal courseid = decimal.Parse(Request.Form["courseid"]);
            decimal studentid = decimal.Parse(Request.Form["studentid"]);
            decimal coursenumber = decimal.Parse(Request.Form["coursenumber"]);
            Attendance att = _Context.Attendances.Find(courseid, studentid, coursenumber);
            if (att != null)
            {
                try
                {
                    _Context.Attendances.Remove(att);
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
    }
}
