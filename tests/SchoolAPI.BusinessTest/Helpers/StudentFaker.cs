using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using SchoolAPI.Business.Models;

namespace SchoolAPI.BusinessTest.Helpers
{
    public class StudentFaker
    {
        public static Faker<Student> CreateFaker()
        {
            return new Faker<Student>()
                .RuleFor(s => s.Id, f => f.Random.Int(1, 1000))
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.Email, f => f.Internet.Email())
                .RuleFor(s => s.Phone, f =>
                {
                    var firstDigit = f.Random.Int(7, 9);
                    var remainingDigits = f.Random.Number(10000000, 99999999);
                    return $"{firstDigit}{remainingDigits}";
                })
                .RuleFor(s => s.DateOfBirth, f => f.Date.Past(20, null))
                .RuleFor(s => s.CreatedAt, f => DateTime.Now)
                .RuleFor(s => s.UpdatedAt, f => DateTime.Now)
                .RuleFor(s => s.IsActive, true);
        }
    }
}