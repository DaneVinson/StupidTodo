using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.Client.Blazor
{
    public class HttpOptions
    {
        public HttpOptions()
        { }

        public HttpOptions(string baseUri)
        {
            ApiUri = baseUri;
        }

        public string ApiUri { get; set; }
    }
}
