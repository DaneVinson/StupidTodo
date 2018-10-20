using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StupidTodo.Client.Blazor
{
    public class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        protected void AddError(PropertyError error)
        {
            if (error != null)
            {
                Errors.Add(error);
                ApplyErrorsChanged(error.PropertyName);
            }
        }

        protected void ClearErrors(string propertyName = null)
        {
            if (propertyName == null) { Errors.Clear(); }
            Errors.Where(e => e.PropertyName == propertyName)
                    .ToList()
                    .ForEach(e => Errors.Remove(e));
            ApplyErrorsChanged(propertyName);
        }

        // INotifyDataErrorInfo
        public IEnumerable GetErrors(string propertyName)
        {
            return Errors.Where(e => e.PropertyName == propertyName)
                        .Select(e => e.Message);
        }

        private void ApplyErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            ApplyPropertyChanged("HasErrors");
        }

        public void ApplyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // INotifyDataErrorInfo
        public bool HasErrors => Errors != null && Errors.Count > 0;

        // INotifyDataErrorInfo
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        public readonly List<PropertyError> Errors = new List<PropertyError>();
    }
}
