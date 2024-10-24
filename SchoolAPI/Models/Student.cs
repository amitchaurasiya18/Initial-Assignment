namespace SchoolAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt  { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool isActive { get; set; }
    }
}
