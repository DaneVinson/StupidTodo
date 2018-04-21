// source: http://guid.us/GUID/JavaScript
function createGuid(){  
  function S4() {  
     return (((1+Math.random())*0x10000)|0).toString(16).substring(1);  
  }  
  return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0,3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();  
}

var vm = new Vue({
    el: '#app',
    data: {
      api: 'http://stupidtodo-api-bravo.azurewebsites.net/api/todo',
      doneToggleText: 'Show done',
      newDescription: '',
      showDone: false,
      todos: []
    },
    methods: {
      addTodo : function () {
        if (this.newDescription) {
          let todo = {
            editDescription: this.newDescription,
            description: this.newDescription,
            done: false,
            id: createGuid(),
            isEdit: false
          };
          let dto = {
            description: todo.description, 
            done: todo.done, 
            id: todo.id
          };
          this.$http.post(this.api, dto).then((response) => { });
          this.newDescription = '';
          this.todos.unshift(todo);
        }
      },
      completeTodo : function (id) {
        let todo = this.todos.find(t => t.id == id);
        if (todo) {
          todo.done = !todo.done;
          this.saveTodo(id);
        }
      },
      deleteTodo : function (id) {
        let todo = this.todos.find(t => t.id == id);
        if (todo) {
          this.$http.delete(this.api + '/' + id).then((response) => {
          });
          for (let i = 0; i < this.todos.length; i++) {
            if (this.todos[i].id == id) {
              this.todos.splice(i, 1);
              i = this.todos.length;
            }
          }
        }
      },
      filteredTodos : function (done) {
        let todos = [];
        for (let i = 0; i < this.todos.length; i++) {
          if (this.todos[i].done === done) {
            todos.push(this.todos[i]);
          }
        }
        return todos;
      },
      getDoneTodos: function () {
        this.$http.get(this.api + '/done').then((response) => {
          this.removeDoneTodos();
          for (let i = 0; i < response.data.length; i++) {
            this.todos.push(this.getTodoFromDto(response.data[i]));
          }
        });
      },
      getTodoFromDto : function (dto) {
        return {
          editDescription: dto.description,
          description: dto.description,
          done: dto.done,
          id: dto.id,
          isEdit: false
        }
      },
      getTodos : function () {
        this.$http.get(this.api).then((response) => {
          this.todos = [];
          for (let i = 0; i < response.data.length; i++) {
            this.todos.push(this.getTodoFromDto(response.data[i]));
          }
        });
      },
      removeDoneTodos : function () {
        let done = this.todos.find(t => t.done);
        while (done) {
          for (let i = 0; i < this.todos.length; i++) {
            if (this.todos[i].id == done.id) {
              this.todos.splice(i, 1);
              i = this.todos.length;
            }
          }
          done = this.todos.find(t => t.done);
        }
      },
      saveTodo : function (id) {
        let todo = this.todos.find(t => t.id == id);
        if (todo) {
          console.log(todo);
          todo.description = todo.editDescription;
          this.$http.put(this.api + '/' + id, todo).then((response) => { });
          todo.isEdit = false;
        }
      },
      toggleEditDescription : function (id) {
        let todo = this.todos.find(t => t.id == id);
        if (todo) {
          todo.isEdit = !todo.isEdit;
          todo.editDescription = todo.description;
        }
      },
      toggleShowDone : function () {
        this.showDone = !this.showDone;
        if (this.showDone){
          this.doneToggleText = 'Hide done';
          this.getDoneTodos();
        }
        else {
          this.doneToggleText = 'Show done';
          this.removeDoneTodos();
        }
      }
    }
});

vm.getTodos();