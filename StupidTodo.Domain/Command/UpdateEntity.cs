using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.Command
{
    public class UpdateEntity<TEntity> : IRequest<TEntity> where TEntity : class, IEntity, new()
    {
        public UpdateEntity()
        {
            Entity = new TEntity();
        }

        public UpdateEntity(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
