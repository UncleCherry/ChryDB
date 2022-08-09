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
        // 获取所有考试信息
        [HttpGet("all")]
        public string GetAllExams()
        {
            Message message = new Message();
            message.data.Add("ExamsList", new List<Exam>());
            message.data["ExamsList"] = _Context.Exams.ToList();
            return message.ReturnJson();
        }
    }
}
