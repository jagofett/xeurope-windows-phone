using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.JsonClasses
{
    public class RegisterJson
    {
        public RegisterJson(string mail, string pass, string n) 
        {
            email = mail;
            password = pass;
            name = n;
        }

        public string email;
        public string password;
        public string name;
    }
}
