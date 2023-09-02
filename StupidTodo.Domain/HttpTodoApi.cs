namespace StupidTodo.Domain;

public class HttpTodoApi : ITodoApi
{
    private readonly HttpClient _httpClient;

    public HttpTodoApi(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Todo?> AddTodoAsync(Todo todo)
    {
        return await _httpClient.PostAsync<Todo>("api/todo", todo);
    }

    public async Task<bool> DeleteTodoAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/todo/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Todo[]?> GetTodosAsync(bool done = false)
    {
        string route;
        if (done) { route = "api/todo/done"; }
        else { route = "api/todo"; }

        var todos = await _httpClient.GetAsync<IEnumerable<Todo>>(route);
        return todos?.ToArray();
    }

    public async Task<Todo?> UpdateTodoAsync(Todo todo)
    {
        return await _httpClient.PutAsync<Todo>($"api/todo/{todo.Id}", todo);
    }
}
