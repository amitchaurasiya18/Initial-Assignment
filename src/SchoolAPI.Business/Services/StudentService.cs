using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services.Interfaces;

namespace SchoolAPI.Business.Services
{
    public class StudentService : IStudentService
    {
        public async Task<int> CalculateAge(DateTime dateOfBirth)
        {
            return await Task.Run(() =>
            {
                DateTime today = DateTime.Now;
                int age = today.Year - dateOfBirth.Year;
                if (dateOfBirth > today.AddYears(-age))
                {
                    age--;
                }
                return age;
            });
        }
    }
}
