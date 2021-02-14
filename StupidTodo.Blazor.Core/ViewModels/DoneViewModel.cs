using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.ViewModels
{
    public class DoneViewModel
    {
        public DoneViewModel(TodosViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel ?? throw new ArgumentNullException(nameof(parentViewModel));
        }

        public async Task DeleteAsync(Todo todo)
        {
            try
            {
                IsBusy = true;
                await _parentViewModel.DeleteAsync(todo.Id);
            }
            finally { IsBusy = false; }
        }

        public async Task UndoAsync(Todo todo)
        {
            try
            {
                IsBusy = true;

                todo.Done = false;

                await _parentViewModel.SaveAsync(todo.Id);
            }
            finally { IsBusy = false; }
        }

        public bool IsBusy { get; set; }

        private readonly TodosViewModel _parentViewModel;
    }
}
