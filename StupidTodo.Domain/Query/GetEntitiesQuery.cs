using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.Query
{
    public class GetEntitiesQuery<TEntity, TFilter> : IRequest<IEnumerable<TEntity>>
        where TEntity : class, IEntity, new()
        where TFilter : class, new()
    {
        public GetEntitiesQuery()
        {
            Filter = new TFilter();
        }

        public GetEntitiesQuery(TFilter filter)
        {
            Filter = filter;
        }

        public TFilter Filter { get; set; }
    }
}
