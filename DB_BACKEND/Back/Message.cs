using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Back
{
    public class Message
    {

        public int errorCode { get; set; }

        public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();


        public string ReturnJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }


    public class LoginMessage : Message
    {
        public LoginMessage()
        {
            errorCode = 300;
            data.Add("loginState", false);
            data.Add("userName", null);
            data.Add("userAvatar", null);
            data.Add("userType", null);
        }
    }


    public class RegisterMessage : Message
    {
        public RegisterMessage()
        {
            errorCode = 300;
            data.Add("registerState", false);
        }
    }


    public class StudentInfoMessage : Message
    {
        public StudentInfoMessage()
        {
            errorCode = 400;
            data.Add("studentID", null);
            data.Add("studentName", null);
            data.Add("studentGrade", null);
            data.Add("studentMajor", null);
            data.Add("studentCredit", null);
        }
    }
    public class VerifyControllerMessage : Message
    {
        public VerifyControllerMessage()
        {
            errorCode = 300;
            data.Add("verifycode", null);
            data.Add("codeimg", null);
        }
    }

    public class ChangePasswordMessage : Message
    {
        public ChangePasswordMessage()
        {
            errorCode = 400;
            data.Add("changestate", false);
        }

    }

}
