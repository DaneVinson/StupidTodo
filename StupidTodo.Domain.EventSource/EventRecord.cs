using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource
{
    public interface IEventRecord
    {
        string EventData { get; set; }
        string EventType { get; set; }
        string Id { get; set; }
        string OwnerId { get; set; }
    }

    public class EventRecord : IEventRecord
    {
        public string EventData { get; set; }
        public string EventType { get; set; }
        public string Id { get; set; }
        public string OwnerId { get; set; }
    }

    /// <summary>
    /// Implementation of IEventRecord for use with Azure Storage Table.
    /// </summary>
    public class EventTableRecord : TableEntity, IEventRecord
    {
        public string EventData { get; set; }
        public string EventType { get; set; }
        public string Id { get; set; }
        public string OwnerId { get; set; }
    }
}
