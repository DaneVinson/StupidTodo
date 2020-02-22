using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    [ProtoContract]
    public class Todo
    {
        [ProtoMember(1)]
        public string Description { get; set; }

        [ProtoMember(2)]
        public bool Done { get; set; }

        [ProtoMember(3)]
        public string Id { get; set; }

        [ProtoMember(4)]
        public string UserId { get; set; }
    }
}
