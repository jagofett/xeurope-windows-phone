using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.JsonClasses
{
    public class CommentsJson
    {
        public struct Message
        {
            public int user_id;
            public string extName;
            public string voterName;
            public string message;
        }

        public Message[] messages;
        public bool error;
        public string error_code;

        public CommentsJson()
        {

        }
    }
}
