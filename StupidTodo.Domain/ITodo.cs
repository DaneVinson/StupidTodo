namespace StupidTodo.Domain;

public interface ITodo
{
    string Description { get; set; }
    bool Done { get; set; }
    string Id { get; set; }
}
