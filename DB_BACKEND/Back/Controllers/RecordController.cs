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
    public class RecordController : ControllerBase
    {
        private readonly ModelContext _Context;
        public RecordController(ModelContext modelContext)
        {
            _Context = modelContext;
        }

        // 根据courseid获取录播信息
        [HttpGet("getRecords")]
        public string GetRecords(decimal courseid = -1)
        {
            Message message = new Message();
            message.data.Add("RecordsList", new List<Record>());
            var records = from c in _Context.Courses
                          join r in _Context.Records
                          on c.CourseId equals r.CourseId
                          where c.CourseId == courseid
                          select new Record
                          {
                              RecordId = r.RecordId,
                              Time = r.Time,
                              Url = r.Url
                          };
            message.data["RecordsList"] = records.ToList();
            message.errorCode = 200;
            return message.ReturnJson();
        }

        // 教师/教务为课程添加录播
        [HttpPost("postRecord")]
        public string PostRecord()
        {
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师/教务身份
            {
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var aUser = from a in _Context.Users
                                where a.UserId == id && (a.UserType == 1 || a.UserType == 2)
                                select a;
                    User admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        //验证教师/教务身份成功
                        //添加录播信息
                        decimal recordid = Decimal.Parse(Request.Form["Recordid"]);
                        decimal courseid = Decimal.Parse(Request.Form["Courseid"]);
                        // 格式：2022-08-08T21:00:00
                        DateTime time = DateTime.Parse(Request.Form["Time"]);
                        string url = Request.Form["Url"];
                        Record record = new Record();
                        record.CourseId = courseid;
                        record.RecordId = recordid;
                        record.Time = time;
                        record.Url = url;
                        try
                        {
                            _Context.Records.Add(record);
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

        // 教师/教务修改录播
        [HttpPut("altRecord")]
        public string AltRecord()
        {
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师/教务身份
            {
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var aUser = from a in _Context.Users
                                where a.UserId == id && (a.UserType == 1 || a.UserType == 2)
                                select a;
                    User admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        //验证身份成功
                        //修改录播信息
                        decimal recordid = decimal.Parse(Request.Form["recordid"]);

                        //查找有无对应录播
                        Record record = _Context.Records.Find(recordid);
                        if (record != null)
                        {
                            //对Records表改
                            string url = Request.Form["url"];
                            record.Url = url;
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
                            message.errorCode = 203;//无对应录播
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


        // 教师/教务删除考试
        [HttpDelete("delRecord")]
        public string DelRecord()
        {
            Message message = new Message();
            message.errorCode = 300;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))//验证token教师/教务身份
            {
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    decimal id = (decimal)data["id"];
                    var aUser = from a in _Context.Users
                                where a.UserId == id && (a.UserType == 1 || a.UserType == 2)
                                select a;
                    User admin = aUser.FirstOrDefault();
                    if (admin != null)
                    {
                        //验证身份成功
                        //删除录播信息
                        decimal recordid = decimal.Parse(Request.Form["recordid"]);

                        //查找有无对应录播
                        Record record = _Context.Records.Find(recordid);
                        if (record != null)
                        {
                            try
                            {
                                _Context.Records.Remove(record);
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
                            message.errorCode = 203;//无对应录播
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
