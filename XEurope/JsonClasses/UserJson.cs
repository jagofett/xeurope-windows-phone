using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.JsonClasses
{
    class UserJson
    {
        public bool error { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string code { get; set; }
        public string createdAt { get; set; }
        public string image_url { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public bool isVoted { get; set; }
        public string message { get; set; }
    }
}
