﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.JsonClasses
{
    public class VoteJson
    {
        public string vote;

        public VoteJson() { }

        public VoteJson(string code) : base ()
        {
            vote = code;
        }
    }
}
    