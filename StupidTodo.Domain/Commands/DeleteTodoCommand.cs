using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class DeleteTodoCommand : Command, ICommand
    {
        public DeleteTodoCommand()
        { }

        public DeleteTodoCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}
