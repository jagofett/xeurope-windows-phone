using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.JsonClasses
{
    class NewPasswordJson
    {
        public NewPasswordJson(string mail, string oldpass, string newpass) 
        {
            email = mail;
            old_password = oldpass;
            new_password = newpass;
        }

        public string email;
        public string old_password;
        public string new_password;
    }
}
