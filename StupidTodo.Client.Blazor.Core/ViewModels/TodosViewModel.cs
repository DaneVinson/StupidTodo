using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StupidTodo.Client.Blazor.Core.ViewModels
{
    public class TodosViewModel
    {
        private readonly ITodoApi _api;

        public TodosViewModel(ITodoApi api)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
        }

        public async Task AddTodoAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTodoDescription)) { return; }

            try
            {
                ActionIsBusy = true;

                var todo = await _api.AddTodoAsync(new Todo(NewTodoDescription, Bilbo.Id));

                NewTodoDescription = string.Empty;
                if (todo != null) { Todos.Add(todo); }
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

            var _ = await _api.DeleteTodoAsync(id);
        }

        public async Task InitializeAsync()
        {
            var todos = await _api.GetTodosAsync();
            Todos = todos?.ToList() ?? new List<Todo>();
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

            var _ = await _api.UpdateTodoAsync(todo);

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
                var dones = await _api.GetTodosAsync(true);
                Dones = dones?.ToList() ?? new List<Todo>();
            }
            else { Dones.Clear(); }
        }

        public bool ActionIsBusy { get; set; }
        public List<Todo> Dones { get; set; } = new List<Todo>();
        public string NewTodoDescription { get; set; } = string.Empty;
        public string DoneToggleText => ShowingDone ? "Hide Done" : "Show Done";
        public bool ShowingDone { get; set; }
        public Action? StateChanged { get; set; }
        public List<Todo> Todos { get; set; } = new List<Todo>();
    }
}
