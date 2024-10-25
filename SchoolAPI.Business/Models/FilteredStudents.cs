using SchoolAPI.Business.Models;

namespace SchoolAPI.Business
{
    public class FilteredStudent
    {
        public IEnumerable<Student> Students { get; set; }
        public int TotalCount { get; set; }
    }
}