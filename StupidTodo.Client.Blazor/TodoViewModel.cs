using Microsoft.AspNetCore.Blazor;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Client.Blazor
{
    public class TodoViewModel : BaseViewModel, ITodo
    {
        public TodoViewModel(HttpClient httpClient, HttpOptions httpOptions)
        {
            HttpOptions = httpOptions ?? throw new ArgumentNullException();
            ServiceHttpClient = httpClient ?? throw new ArgumentNullException();
        }


        public void BeginEdit()
        {
            IsEditing = true;
        }

        public void CancelEdit()
        {
            IsEditing = false;
        }
        
        public void DescriptionChanged(UIEventArgs args)
        {
            Errors.Add(new PropertyError() { Message = $"{Description} -> {args.Type}", PropertyName = DescriptionProperty });

            //ApplyStateChanged?.Invoke();
        }

        public void Load(ITodo todo)
        {
            ClearErrors();
            Done = todo.Done;
            Id = todo.Id;
            IsEditing = false;
            _description = todo.Description;
            OriginalDescription = _description;
        }

        public async Task<bool> SaveTodoAsync()
        {
            Errors.Add(new PropertyError() { Message = $"{Description}", PropertyName = DescriptionProperty });
            //var todo = this as ITodo;

            //if (String.IsNullOrWhiteSpace(todo.Description)) { return false; }

            return false;

            //IsBusy = true;
            //string description = todo.Description;
            //var resultTodo = await ServiceHttpClient.PutJsonAsync<Todo>($"{HttpOptions.ApiUri}/{todo.Id}", todo);
            //if (resultTodo != null)
            //{
            //    todo.Description = resultTodo.Description;
            //    todo.DescriptionEdit = resultTodo.Description;
            //    todo.Done = resultTodo.Done;
            //    todo.IsEditing = false;
            //    IsBusy = false;
            //    return true;
            //}
            //else
            //{
            //    todo.Description = description;
            //    todo.DescriptionEdit = description;
            //    todo.IsEditing = false;
            //    IsBusy = false;
            //    return false;
            //}
        }

        public async Task ToggleDoneAsync()
        {
            Done = !Done;
            var result = await SaveTodoAsync();
            if (!result) { Done = !Done; }
        }


        public string Description
        {
            get => _description;
            set
            {
                ClearErrors(DescriptionProperty);
                if (String.IsNullOrWhiteSpace(value))
                {
                    AddError(new PropertyError() { Message = "Description is required", PropertyName = DescriptionProperty });
                }
                ApplyPropertyChanged(DescriptionProperty);
                _description = value;
            }
        }
        private string _description;
        private const string DescriptionProperty = "Description";

        public bool Done { get; set; }

        public string Id { get; set; }

        public bool IsEditing { get; set; }

        public string OriginalDescription { get; set; }


        public event Action ApplyStateChanged;


        private readonly HttpOptions HttpOptions;
        private readonly HttpClient ServiceHttpClient;
    }
}
