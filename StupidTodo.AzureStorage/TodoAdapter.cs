using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.AzureStorage
{
    internal class TodoAdapter : TableEntityAdapter<Todo>
    {
        public TodoAdapter()
        { }

        public TodoAdapter(Todo todo) : base(todo)
        {
            PartitionKey = todo.UserId;
            RowKey = todo.Id;
        }
    }
}
