using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StupidTodo.Client.Blazor.Core.ViewModels
{
    public class TodoViewModel
    {
        public TodoViewModel(TodosViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel ?? throw new ArgumentNullException(nameof(parentViewModel));
        }

        public void CancelEdit()
        {
            DescriptionEdit = string.Empty;
            IsEdit = false;
        }

        public void Edit(string id)
        {
            IsEdit = true;
            DescriptionEdit = _parentViewModel.Todos.First(t => t.Id == id).Description;
        }

        public async Task SaveAsync(Todo todo)
        {
            try
            {
                IsBusy = true;

                if (IsEdit)
                {
                    if (string.IsNullOrWhiteSpace(DescriptionEdit)) { return; }
                    todo.Description = DescriptionEdit;
                }

                await _parentViewModel.SaveAsync(todo.Id);
            }
            finally 
            { 
                IsBusy = false;
                IsEdit = false;
            }
        }

        public async Task SetDoneAsync(Todo todo)
        {
            todo.Done = true;
            await SaveAsync(todo);
        }

        public string DescriptionEdit { get; set; } = string.Empty;
        public bool IsBusy { get; set; }
        public bool IsEdit { get; set; }

        private readonly TodosViewModel _parentViewModel;
    }
}
