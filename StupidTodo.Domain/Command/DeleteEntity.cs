using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.Command
{
    public class DeleteEntity<TEntity> : IRequest<TEntity> where TEntity : IEntity
    {
        public DeleteEntity()
        { }

        public DeleteEntity(string id)
        {
            Id = id ?? string.Empty;
        }

        public string Id { get; set; } = string.Empty;
    }
}
