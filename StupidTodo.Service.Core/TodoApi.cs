namespace StupidTodo.Service.Core;

public class TodoApi : ITodoApi
{
	private readonly ITodoDataProvider _dataProvider;

	public TodoApi(ITodoDataProvider dataProvider)
	{
		_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
	}


	public async Task<Todo?> AddTodoAsync(Todo todo)
	{
		if (todo?.Description == null) { return null; }
		todo.Id = Guid.NewGuid().ToString();

		return await _dataProvider.Upsert(todo);
	}

	public async Task<bool> DeleteTodoAsync(string id)
	{
		if (string.IsNullOrWhiteSpace(id)) { return false; }

		return await _dataProvider.Delete(id);
	}

	public async Task<Todo[]?> GetTodosAsync(bool done = false)
	{
		return (await _dataProvider.Get(done)).ToArray();
	}

	public async Task<Todo?> UpdateTodoAsync(Todo todo)
	{
		return await _dataProvider.Upsert(todo);
	}
}
