using FluentValidation;
using SchoolAPI.DTO;
using SchoolAPI.StaticFiles;

namespace SchoolAPI.Validators
{
    public class StudentUpdateValidator : AbstractValidator<StudentUpdateDTO>
    {
        private static readonly DateTime MinimumBirthDate = new DateTime(2019, 1, 1);
        
        public StudentUpdateValidator() 
        {
            RuleFor(student => student.FirstName)
                .MinimumLength(3).WithMessage(ValidationMessages.FIRSTNAME_MIN_LENGTH)
                .MaximumLength(20).WithMessage(ValidationMessages.FIRSTNAME_MAX_LENGTH)
                .When(s => !string.IsNullOrEmpty(s.FirstName));  

            RuleFor(student => student.LastName)
                .MinimumLength(3).WithMessage(ValidationMessages.LASTNAME_MIN_LENGTH)
                .MaximumLength(20).WithMessage(ValidationMessages.LASTNAME_MAX_LENGTH)
                .When(s => !string.IsNullOrEmpty(s.LastName));

            RuleFor(student => student.Email)
                .EmailAddress().WithMessage(ValidationMessages.EMAIL_INVALID)
                .When(s => !string.IsNullOrEmpty(s.Email));

            RuleFor(student => student.Phone)
                .MaximumLength(10).WithMessage(ValidationMessages.PHONE_MAX_LENGTH)
                .When(s => !string.IsNullOrEmpty(s.Phone))
                .NotEqual("0000000000").WithMessage(ValidationMessages.PHONE_INVALID);

            RuleFor(student => student.DateOfBirth)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_DOB)
                .LessThan(MinimumBirthDate).WithMessage(ValidationMessages.DOB_INVALID)
                .When(s => s.DateOfBirth.HasValue);
        }
    }
}
