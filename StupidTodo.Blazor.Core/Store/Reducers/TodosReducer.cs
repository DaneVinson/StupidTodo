using Fluxor;
using StupidTodo.Blazor.Core.Store.Actions;
using StupidTodo.Blazor.Core.Store.States;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Reducers
{
    public static class TodosReducer
    {
        [ReducerMethod]
        public static TodosState ReduceAddTodoAction(TodosState state, AddTodoAction action) =>
            new TodosState(
                    todos: state.Todos.ToArray(),
                    busy: true,
                    showingDone: state.ShowingDone);

        [ReducerMethod]
        public static TodosState ReduceAddTodoResultAction(TodosState state, AddTodoResultAction action)
        {
            var todos = state.Todos
                                .Select(t => new TodoState(t))
                                .ToList();
            todos.Add(new TodoState(action.Todo));

            return new TodosState(
                            todos,
                            false,
                            state.ShowingDone);
        }

        [ReducerMethod]
        public static TodosState ReduceDeleteTodoAction(TodosState state, DeleteTodoAction action) =>
            new TodosState(
                    todos: state.Todos.ToArray(),
                    busy: true,
                    showingDone: state.ShowingDone);

        [ReducerMethod]
        public static TodosState ReduceDeleteTodoResultAction(TodosState state, DeleteTodoResultAction action)
        {
            var todos = state.Todos
                                .Where(t => t.Id != action.Id)
                                .Select(t => new TodoState(t))
                                .ToList();

            return new TodosState(
                            todos,
                            false,
                            state.ShowingDone);
        }

        [ReducerMethod]
        public static TodosState ReduceGetDonesAction(TodosState state, GetDonesAction _) =>
            new TodosState(
                    todos: state.Todos.ToArray(),
                    busy: true,
                    showingDone: true);

        [ReducerMethod]
        public static TodosState ReduceGetDonesResultAction(TodosState state, GetDonesResultAction action)
        {
            var todos = state.Todos.Where(t => !t.Done).ToList();
            todos.AddRange(action.Todos.Select(t => new TodoState(t)));
             
            return new TodosState(
                            todos: todos,
                            busy: false,
                            showingDone: true);
        }

        [ReducerMethod]
        public static TodosState ReduceGetTodosAction(TodosState state, GetTodosAction _) =>
            new TodosState(
                    todos: state.Todos.ToArray(),
                    busy: true,
                    showingDone: false);

        [ReducerMethod]
        public static TodosState ReduceGetTodosResultAction(TodosState state, GetTodosResultAction action)
        {
            var todos = new List<ITodo>(action.Todos.Select(t => new TodoState(t)));

            return new TodosState(
                            todos,
                            busy: false,
                            showingDone: false);
        }

        [ReducerMethod]
        public static TodosState ReduceHideDoneAction(TodosState state, HideDoneAction _) =>
            new TodosState(
                    state.Todos.Where(t => !t.Done).ToArray(),
                    busy: false,
                    showingDone: false);

        [ReducerMethod]
        public static TodosState ReduceUpdateTodoAction(TodosState state, UpdateTodoAction action) =>
            new TodosState(
                    todos: state.Todos.ToArray(),
                    busy: true,
                    showingDone: state.ShowingDone);

        [ReducerMethod]
        public static TodosState ReduceUpdateTodoResultAction(TodosState state, UpdateTodoResultAction action)
        {
            var todos = new List<TodoState>();
            foreach (var todo in state.Todos)
            {
                if (todo.Id == action.Todo?.Id)
                {
                    todos.Add(new TodoState(action.Todo));
                }
                else
                {
                    todos.Add(new TodoState(todo));
                }
            }

            return new TodosState(
                            todos,
                            false,
                            state.ShowingDone);
        }
    }
}
