using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.JsonClasses
{
    public class VoteWithCommentJson : VoteJson
    {
        public string message;

        public VoteWithCommentJson() { }

        public VoteWithCommentJson(string code, string m)
        {
            vote = code;
            message = m;
        }
    }
}
    