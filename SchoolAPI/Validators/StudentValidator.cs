using FluentValidation;
using SchoolAPI.DTO;
using SchoolAPI.Models;

namespace SchoolAPI.Validators
{
    public class StudentValidator : AbstractValidator<StudentPostDTO>
    {
        public StudentValidator() 
        {
            RuleFor(student => student.FirstName)
                .MinimumLength(3).WithMessage("First Name should have minimum 3 letters")
                .MaximumLength(20).WithMessage("First Name should have only 20 letters")
                .NotEmpty().WithMessage("First Name cannot be empty")
                .NotEqual("foo").WithMessage("Last Name not valid");

            RuleFor(student => student.LastName)
                .MinimumLength(3).WithMessage("Last Name should have minimum 3 letters")
                .MaximumLength(20).WithMessage("Last Name should have only 20 letters")
                .NotEmpty().WithMessage("Last Name cannot be empty")
                .NotEqual("foo").WithMessage("Last Name not valid");

            RuleFor(student => student.Email)
                .EmailAddress().WithMessage("Email Address not valid")
                .NotEmpty().WithMessage("Email is required");

            RuleFor(student => student.Phone)
                .MaximumLength(10).WithMessage("Phone number cannot exceed 10 digits")
                .NotEmpty().WithMessage("Phone number is required")
                .NotEqual("0000000000").WithMessage("Not a valid phone number");

            RuleFor(student => student.DateOfBirth)
                .Must(BeAValidBirthDate).WithMessage("Not a Valid Date of Birth")
                .NotEmpty().WithMessage("Date of Birth is required");
        }

        private bool BeAValidBirthDate(DateTime birthDate)
        {
            var minDate = DateTime.Now.AddYears(-10);
            return birthDate <= minDate;
        }
    }
}
