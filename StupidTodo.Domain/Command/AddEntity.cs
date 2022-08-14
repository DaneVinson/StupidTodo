using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.Command
{
    public class AddEntity<TEntity> : IRequest<TEntity> where TEntity : class, IEntity, new()
    {
        public AddEntity()
        {
            Entity = new TEntity();
        }

        public AddEntity(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
