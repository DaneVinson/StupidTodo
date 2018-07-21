using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    /// <summary>
    /// Implementation of <see cref="IEventRecord"/> for use with Azure Storage Table.
    /// </summary>
    public class EventRecordTableEntity : TableEntity, IEventRecord
    {
        public Guid EntityId { get; set; }
        public string EventData { get; set; }
        public string EventType { get; set; }
        public Guid Id { get; set; }
        public string Version { get; set; }
    }
}
