using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StupidTodo.Domain.EventSource.Test
{
    public class TodoAggregateTest
    {
        [Fact]
        public async Task Test1()
        {
            var eventStore = Substitute.For<IEventStore>();
            eventStore.GetEventRecordsAsync(Arg.Any<string>()).Returns(GetEventRecords());
            var aggregate = new TodoAggregate(eventStore);

            //var todo = await aggregate.GetTodoAsyc("some_id");
            aggregate.AddEvent("some_id", typeof(Created), GetEventRecords().First().EventData).GetAwaiter().GetResult();

            Assert.True(true);
        }

        private IEnumerable<EventRecord> GetEventRecords()
        {
            return new EventRecord[]
            {
                new EventRecord()
                {
                    EventData = JsonConvert.SerializeObject(new CreateEventSchema() { Description = "Create a todo" }),
                    EventType = typeof(Created).ToString(),
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = "dane"
                },
                new EventRecord()
                {
                    EventData = JsonConvert.SerializeObject(new UpdateDescriptionEventSchema() { Description = "Update description" }),
                    EventType = typeof(DescriptionUpdated).ToString(),
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = "dane"
                },
                new EventRecord()
                {
                    EventData = JsonConvert.SerializeObject(new UpdateDescriptionEventSchema() { Description = "Update description again" }),
                    EventType = typeof(DescriptionUpdated).ToString(),
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = "dane"
                },
                new EventRecord()
                {
                    EventData = JsonConvert.SerializeObject(new UpdateDoneEventSchema() { Done = true }),
                    EventType = typeof(DoneUpdated).ToString(),
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = "dane"
                },
                new EventRecord()
                {
                    EventData = JsonConvert.SerializeObject(new UpdateDoneEventSchema() { Done = false }),
                    EventType = typeof(DoneUpdated).ToString(),
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = "dane"
                },
                new EventRecord()
                {
                    EventData = JsonConvert.SerializeObject(new UpdateDescriptionEventSchema() { Description = "Undone then update description" }),
                    EventType = typeof(DescriptionUpdated).ToString(),
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = "dane"
                },
                new EventRecord()
                {
                    EventData = JsonConvert.SerializeObject(new UpdateDoneEventSchema() { Done = true }),
                    EventType = typeof(DoneUpdated).ToString(),
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = "dane"
                }
            };
        }
    }
}
