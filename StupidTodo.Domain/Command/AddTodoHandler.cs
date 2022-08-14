using MediatR;
using StupidTodo.Domain;
using StupidTodo.Domain.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StupidTodo.Domain.Command
{
    public class AddTodoHandler : IRequestHandler<AddEntity<Todo>, Todo>
    {
        private readonly ITodoDataProvider _dataProvider;

        public AddTodoHandler(ITodoDataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public async Task<Todo> Handle(AddEntity<Todo> command, CancellationToken _)
        {
            return await _dataProvider.Upsert(command.Entity);
        }
    }
 }
