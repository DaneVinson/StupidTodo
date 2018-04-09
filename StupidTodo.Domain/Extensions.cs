using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StupidTodo.Domain
{
    public static class Extensions
    {
        public static Todo GetTodo(this ITodo todo)
        {
            return new Todo()
            {
                Description = todo.Description,
                Done = todo.Done,
                Id = todo.Id
            };
        }

        public static bool IsSuccessCode(this HttpStatusCode code)
        {
            return ((int)code).IsSuccessCode();
        }

        public static bool IsSuccessCode(this int code)
        {
            return code > 199 && code < 300;
        }

        public static T NewObjectFromSection<T>(this IConfiguration configuration, string sectionKey) where T : new()
        {
            T newObject = (T)Activator.CreateInstance(typeof(T));
            if (configuration != null) { configuration.GetSection(sectionKey).Bind(newObject); }
            return newObject;
        }
    }
}
