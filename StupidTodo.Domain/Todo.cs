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
        public string UserId { get; set; }

        public bool Boolean1 { get; set; }
        public DateTime Date1 { get; set; }
        public DateTimeOffset Date2 { get; set; }
        public int Number1 { get; set; }
        public long Number2 { get; set; }
        public double Number3 { get; set; }
        public decimal Number4 { get; set; }
        public string String1 { get; set; }
        public string String2 { get; set; }
    }
}
