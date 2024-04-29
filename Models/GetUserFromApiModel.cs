namespace Blog.Models
{
    public class GetUserFromApiModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string DateJoined { get; set; }
        public string Image { get; set; } 
        public int Blogs { get; set; }

    }
}
