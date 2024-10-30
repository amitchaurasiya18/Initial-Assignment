using FluentValidation;
using SchoolAPI.DTO;

namespace SchoolAPI.Validators
{
    public class StudentUpdateValidator : AbstractValidator<StudentUpdateDTO>
    {
        private static readonly DateTime MinimumBirthDate = new DateTime(2014, 1, 1);
        public StudentUpdateValidator() 
        {
            RuleFor(student => student.FirstName)
                .MinimumLength(3).WithMessage("First Name should have minimum 3 letters")
                .MaximumLength(20).WithMessage("First Name should have only 20 letters")
                .When(s => !string.IsNullOrEmpty(s.FirstName));  

            RuleFor(student => student.LastName)
                .MinimumLength(3).WithMessage("Last Name should have minimum 3 letters")
                .MaximumLength(20).WithMessage("Last Name should have only 20 letters")
                .When(s => !string.IsNullOrEmpty(s.LastName));

            RuleFor(student => student.Email)
                .EmailAddress().WithMessage("Email Address not valid")
                .When(s => !string.IsNullOrEmpty(s.Email));

            RuleFor(student => student.Phone)
                .MaximumLength(10).WithMessage("Phone number cannot exceed 10 digits")
                .When(s => !string.IsNullOrEmpty(s.Phone))
                .NotEqual("0000000000").WithMessage("Not a valid phone number");

            RuleFor(student => student.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required")
                .LessThan(MinimumBirthDate).WithMessage("Not a Valid Date of Birth")
                .When(s => s.DateOfBirth.HasValue);
        }
    }
}
