using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.AzureStorage
{
    public class AzureStorageOptions
    {
        public string ConnectionString { get; set; }
        public string TodoContainer { get; set; }
    }
}
