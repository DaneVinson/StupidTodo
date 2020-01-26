using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class TestingOptions
    {
        public string DataFilesFolder { get; set; }
        public string GrpcUri { get; set; }
        public int Iterations { get; set; }
        public string WcfUri { get; set; }
        public string WebApiUri { get; set; }
    }
}
