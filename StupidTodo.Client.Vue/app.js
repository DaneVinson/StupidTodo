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
      api: 'http://stupidtodo.azurewebsites.net/api/todos',
      newDescription: '',
      todos: [
      ]
    },
    methods: {
      addTodo : function () {
        if (this.newDescription) {
          let todo = {
            editDescription: this.newDescription,
            description: this.newDescription,
            id: createGuid(),
            isEdit: false
          };
          this.$http.post(this.api, {description: todo.description, id: todo.id }).then((response) => {
            console.log('post');
            console.log(response);
          });
          this.newDescription = '';
          this.todos.unshift(todo);
        }
      },
      completeTodo : function (id) {
        let todo = this.todos.find(t => t.id == id);
        if (todo) {
          this.$http.delete(this.api + '/' + id).then((response) => {
            console.log('delete');
            console.log(response);
          });
          for (var i = 0; i < this.todos.length; i++)
          {
            if (this.todos[i].id == id) {
              this.todos.splice(i, 1);
              i = this.todos.length;
            }
          }
        }
      },
      getTodos : function () {
        this.$http.get(this.api).then((response) => {
          this.todos = [];
          for (var i = 0; i < response.data.length; i++)
          {
            this.todos.push({
              editDescription: response.data[i].description,
              description: response.data[i].description,
              id: response.data[i].id,
              isEdit: false
            })
          }
        });
      },
      saveTodo : function (id) {
        let todo = this.todos.find(t => t.id == id);
        if (todo) {
          todo.description = todo.editDescription;
          this.$http.put(this.api + '/' + id, todo).then((response) => {
            console.log('put');
            console.log(response);
          });
          todo.isEdit = false;
        }
      },
      toggleEditDescription : function (id) {
        let todo = this.todos.find(t => t.id == id);
        if (todo) {
          todo.isEdit = !todo.isEdit;
          todo.editDescription = todo.description;
        }
      }
    }
});

vm.getTodos();