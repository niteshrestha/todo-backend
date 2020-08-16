namespace Todo.Data.Request
{
    public class ItemInput
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
    }
}
