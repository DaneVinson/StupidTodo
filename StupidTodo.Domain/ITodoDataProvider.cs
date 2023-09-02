namespace StupidTodo.Domain;

public interface ITodoDataProvider
{
    Task<bool> Delete(string id);
    Task<IEnumerable<Todo>> Get(bool done = false);
    Task<Todo> Upsert(Todo todo);
}
