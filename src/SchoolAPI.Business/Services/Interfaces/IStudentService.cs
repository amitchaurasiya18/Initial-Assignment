using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Services.Interfaces
{
    public interface IStudentService
    {
        Task<int> CalculateAge(DateTime dateOfBirth);
    }
}
