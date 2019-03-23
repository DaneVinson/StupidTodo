using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class UpdateTodoCommand : Command, ICommand
    {
        public UpdateTodoCommand()
        { }

        public UpdateTodoCommand(string id, string description, bool done)
        {
            Description = description;
            Done = done;
            Id = id;
        }


        public string Description { get; set; }
        public bool Done { get; set; }
        public string Id { get; set; }
    }
}
