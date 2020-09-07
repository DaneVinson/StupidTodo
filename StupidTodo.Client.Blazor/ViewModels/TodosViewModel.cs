using StupidTodo.Client.Blazor.Views;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StupidTodo.Client.Blazor.ViewModels
{
    public class TodosViewModel
    {
        public TodosViewModel(HttpClient httpClient)
        {
            _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task AddTodoAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTodoDescription)) { return; }

            try
            {
                ActionIsBusy = true;

                var todo = await _http.PostAsync<Todo>("api/todo", new Todo(NewTodoDescription));

                NewTodoDescription = string.Empty;
                Todos.Add(todo);
                StateChanged?.Invoke();
            }
            finally
            {
                ActionIsBusy = false;
            }
        }

        public async Task DeleteAsync(string id)
        {
            var todo = Dones.First(t => t.Id == id);
            Dones.Remove(todo);

            var _ = await _http.DeleteAsync($"api/todo/{id}");
        }

        public async Task InitializeAsync()
        {
            Todos = (await _http.GetAsync<IEnumerable<Todo>>("api/todo"))
                                .ToList();
        }

        public async Task SaveAsync(string id)
        {
            var doneBeforeChange = false;
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            { 
                todo = Dones.FirstOrDefault(t => t.Id == id);
                doneBeforeChange = true;
            }
            if (todo == null) { return; }

            var _ = await _http.PutAsync<Todo>($"api/todo/{id}", todo);

            if (todo.Done && !doneBeforeChange)
            {
                Todos.Remove(todo);
                if (ShowingDone) { Dones.Add(todo); }
            }
            else if (!todo.Done && doneBeforeChange)
            {
                if (ShowingDone) { Dones.Remove(todo); }
                Todos.Add(todo);
            }

            StateChanged?.Invoke();
        }

        public async Task ToggleShowDone()
        {
            ShowingDone = !ShowingDone;

            if (ShowingDone)
            {
                Dones = (await _http.GetAsync<IEnumerable<Todo>>("api/todo/done"))
                                    .ToList();
            }
            else { Dones.Clear(); }
        }

        public bool ActionIsBusy { get; set; }
        public List<Todo> Dones { get; set; } = new List<Todo>();
        public string NewTodoDescription { get; set; } = string.Empty;
        public string DoneToggleText => ShowingDone ? "Hide Done" : "Show Done";
        public bool ShowingDone { get; set; }
        public Action StateChanged { get; set; }
        public List<Todo> Todos { get; set; } = new List<Todo>();


        private readonly HttpClient _http;
    }
}
