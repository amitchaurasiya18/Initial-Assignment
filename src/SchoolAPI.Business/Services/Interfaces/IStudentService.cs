using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Services.Interfaces
{
    public interface IStudentService
    {
        int CalculateAge(DateTime dateOfBirth);
    }
}
