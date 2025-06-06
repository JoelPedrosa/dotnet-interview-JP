namespace TodoApi.Dtos
{
    public class UpdateTodoItem
    {
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
