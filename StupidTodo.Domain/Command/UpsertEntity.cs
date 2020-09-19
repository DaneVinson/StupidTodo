using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.Command
{
    public class UpsertEntity<TEntity> : IRequest<TEntity> where TEntity : class, IEntity, new()
    {
        public UpsertEntity()
        {
            Entity = new TEntity();
        }

        public UpsertEntity(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
