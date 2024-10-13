namespace todo_list.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }   
        public string Description { get; set; }
        public bool IsFinished { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
