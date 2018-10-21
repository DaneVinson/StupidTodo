using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class Todo
    {
        public string Description { get; set; }
        public bool Done { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            string doneText = Done ? " Done" : String.Empty;
            return $"{Description} ({Id}){doneText}";
        }
    }
}
