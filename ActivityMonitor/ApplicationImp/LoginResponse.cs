using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActivityMonitor.ApplicationImp
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
