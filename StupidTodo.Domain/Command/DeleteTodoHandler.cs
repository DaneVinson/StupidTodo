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
    public class DeleteTodoHandler : IRequestHandler<DeleteEntity<Todo>, bool>
    {
        private readonly ITodoDataProvider _dataProvider;

        public DeleteTodoHandler(ITodoDataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public async Task<bool> Handle(DeleteEntity<Todo> request, CancellationToken _)
        {
            return await _dataProvider.Delete(request.Id);
        }
    }
 }
