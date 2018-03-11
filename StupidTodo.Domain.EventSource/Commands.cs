using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource
{
    public interface ICommand
    {
        string HandlerType { get; }
        string TargetId { get; set; }
    }

    public class CreateCommand : ICommand
    {
        public string Description { get; set; }

        public string HandlerType => typeof(CreateCommandHandler).ToString();

        public string TargetId { get; set; }
    }

    public class DeleteCommand : ICommand
    {
        public string HandlerType => typeof(DeleteCommandHandler).ToString();

        public string TargetId { get; set; }
    }

    public class UpdateCommand : ICommand
    {
        public string Description { get; set; }

        public bool? Done { get; set; }

        public string HandlerType => typeof(UpdateCommandHandler).ToString();

        public string TargetId { get; set; }
    }
}
