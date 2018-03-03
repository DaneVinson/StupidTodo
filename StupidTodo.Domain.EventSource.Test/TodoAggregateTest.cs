using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
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
            eventStore.GetEventsAsync(Arg.Any<string>()).Returns(GetEvents());
            var aggregate = new TodoAggregate(eventStore);

            var todo = await aggregate.GetTodoAsyc("some_id");
            Assert.True(true);
        }

        private IEnumerable<string> GetEvents()
        {
            var eventsJson = new List<string>();
            eventsJson.Add(JsonConvert.SerializeObject(new CreateEventSchema() { Description = "Create a todo" }));
            eventsJson.Add(JsonConvert.SerializeObject(new UpdateDescriptionEventSchema() { Description = "Update description" }));
            eventsJson.Add(JsonConvert.SerializeObject(new UpdateDescriptionEventSchema() { Description = "Update description again" }));
            eventsJson.Add(JsonConvert.SerializeObject(new UpdateDoneEventSchema() { Done = true }));
            eventsJson.Add(JsonConvert.SerializeObject(new UpdateDoneEventSchema() { Done = false }));
            eventsJson.Add(JsonConvert.SerializeObject(new UpdateDescriptionEventSchema() { Description = "Undone then update description" }));
            eventsJson.Add(JsonConvert.SerializeObject(new UpdateDoneEventSchema() { Done = true }));
            return eventsJson;
        }
    }
}
