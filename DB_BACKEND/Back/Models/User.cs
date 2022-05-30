using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Back.Models
{
    public class User
    {
        int UserID { get; set; }
        string UserNmae { get; set; }
        string Password { get; set; }
        int UserType { get; set; }
    }
}
