using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Data.AzureTableStorage
{
    public class TodoTableEntity : TableEntity, ITodo
    {
        public string Description { get; set; }
        public bool Done { get; set; }
        public string Id { get; set; }
    }
}
