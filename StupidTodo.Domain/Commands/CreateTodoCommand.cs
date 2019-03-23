using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class CreateTodoCommand : Command, ICommand
    {
        public CreateTodoCommand()
        { }

        public CreateTodoCommand(string id, string description)
        {
            Description = description;
            Id = id;
        }


        public string Description { get; set; }
        public string Id { get; set; }
    }
}
