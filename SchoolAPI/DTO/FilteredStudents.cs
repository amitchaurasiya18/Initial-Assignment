namespace SchoolAPI.DTO
{
    public class FilteredStudent
    {
        public IEnumerable<StudentGetDTO> Students { get; set; }
        public int TotalCount { get; set; }
    }
}