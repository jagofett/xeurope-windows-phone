using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.JsonClasses
{
    class LoginJson
    {
        public LoginJson(string mail, string pass) : base ()
        {
            email = mail;
            password = pass;
        }

        public string email;
        public string password;
    }
}
