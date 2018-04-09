using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    /// <summary>
    /// Public interface defining actions to update and view projection data.
    /// </summary>
    public interface IProjector<TEntity, TOptions>
    {
        Task<Result> PersistProjectionAsync(TEntity entity);
        Task<Result> RemoveProjectionAsync(TEntity entity);
        Task<Result<IEnumerable<TEntity>>> ViewProjectionAsync(TOptions options);
    }
}
