using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.ClientConsole
{
    public class QueryOptions
    {
        public int FrequencySeconds { get; set; }

        public Args QueryArgs { get; set; }


        public class Args
        {
            public string LonelyData { get; set; }
            public string PartialUri { get; set; }
            public int Timeout { get; set; }
        }
    }
}
