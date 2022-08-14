using MediatR;
using StupidTodo.Domain;
using StupidTodo.Domain.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StupidTodo.Domain.Query
{
    public class GetDoneHandler : IRequestHandler<GetEntitiesQuery<Todo, GetDoneFilter>, IEnumerable<Todo>>
    {
        private readonly ITodoDataProvider _dataProvider;

        public GetDoneHandler(ITodoDataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public Task<IEnumerable<Todo>> Handle(GetEntitiesQuery<Todo, GetDoneFilter> request, CancellationToken _)
        {
            return _dataProvider.Get(true);
        }
    }
}
