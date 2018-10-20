using Microsoft.AspNetCore.Blazor;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StupidTodo.Client.Blazor
{
    public class MainViewModel
    {
        public MainViewModel(HttpClient httpClient, HttpOptions httpOptions)
        {
            HttpOptions = httpOptions ?? throw new ArgumentNullException();
            ServiceHttpClient = httpClient ?? throw new ArgumentNullException();
        }


        public void CancelEdit(StatefulTodo todo)
        {
            todo.DescriptionEdit = todo.Description;
            todo.IsEditing = false;
        }

        public async Task CreateTodoAsync()
        {
            if (String.IsNullOrWhiteSpace(NewTodoDescription)) { return; }

            IsBusy = true;
            var todo = await ServiceHttpClient.PostJsonAsync<Todo>($"{HttpOptions.ApiUri}", new Todo(NewTodoDescription));
            if (todo != null) { Todos = Todos.Concat(new StatefulTodo[] { new StatefulTodo(todo) }).ToArray(); }
            NewTodoDescription = String.Empty;
            IsBusy = false;
            ApplyStateChanged();
        }

        public async Task DeleteTodoAsync(StatefulTodo todo)
        {
            var result = await ServiceHttpClient.DeleteAsync($"{HttpOptions.ApiUri}/{todo.Id}");
            if (result != null && result.IsSuccessStatusCode)
            {
                Todos = Todos.Where(t => t.Id != todo.Id).ToArray();
            }
            ApplyStateChanged();
        }

        public void EditTodo(StatefulTodo todo)
        {
            todo.IsEditing = true;
        }

        public async Task GetDoneAsync()
        {
            IsBusy = true;
            ShowingDone = !ShowingDone;
            if (!ShowingDone)
            {
                Todos = Todos.Where(t => !t.Done).ToArray();
            }
            else
            {
                var dones = await ServiceHttpClient.GetJsonAsync<Todo[]>($"{HttpOptions.ApiUri}/done");
                dones = dones ?? new Todo[0];
                Todos = Todos.Where(t => !t.Done)
                            .Concat(dones)
                            .Select(t => new StatefulTodo(t))
                            .ToArray();
            }
            IsBusy = false;
            ApplyStateChanged();
        }

        public async Task GetTodosAsync()
        {
            IsBusy = true;
            var todos = await ServiceHttpClient.GetJsonAsync<Todo[]>($"{HttpOptions.ApiUri}") ?? new Todo[0];
            Todos = todos.Select(t => new StatefulTodo(t)).ToArray();
            IsBusy = false;
        }


        public bool IsSaveEnabled(StatefulTodo todo)
        {
            return !String.IsNullOrWhiteSpace(todo.DescriptionEdit);
        }

        public async Task<bool> SaveTodoAsync(StatefulTodo todo)
        {
            if (String.IsNullOrWhiteSpace(todo.DescriptionEdit)) { return false; }

            IsBusy = true;
            string description = todo.Description;
            todo.Description = todo.DescriptionEdit;
            var resultTodo = await ServiceHttpClient.PutJsonAsync<Todo>($"{HttpOptions.ApiUri}/{todo.Id}", todo);
            if (resultTodo != null)
            {
                todo.Description = resultTodo.Description;
                todo.DescriptionEdit = resultTodo.Description;
                todo.Done = resultTodo.Done;
                todo.IsEditing = false;
                IsBusy = false;
                return true;
            }
            else
            {
                todo.Description = description;
                todo.DescriptionEdit = description;
                todo.IsEditing = false;
                IsBusy = false;
                return false;
            }
        }

        public async Task ToggleDoneAsync(StatefulTodo todo)
        {
            todo.Done = !todo.Done;
            var result = await SaveTodoAsync(todo);
            if (!result) { todo.Done = !todo.Done; }
            ApplyStateChanged();
        }


        public bool IsBusy { get; set; }
        public string NewTodoDescription { get; set; }
        public string ShowDoneToggleText => ShowingDone ? "Hide done" : "Show done";
        public bool ShowingDone { get; set; }
        public StatefulTodo[] Todos { get; set; }


        public event Action ApplyStateChanged;


        private readonly HttpOptions HttpOptions;
        private readonly HttpClient ServiceHttpClient;
    }
}
