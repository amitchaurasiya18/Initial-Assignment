using FluentValidation;
using SchoolAPI.DTO;
using SchoolAPI.StaticFiles;

namespace SchoolAPI.Validators
{
    public class StudentValidator : AbstractValidator<StudentPostDTO>
    {
        private static readonly DateTime MinimumBirthDate = new DateTime(2014, 1, 1);
        
        public StudentValidator() 
        {
            RuleFor(student => student.FirstName)
                .MinimumLength(3).WithMessage(ValidationMessages.FIRSTNAME_MIN_LENGTH)
                .MaximumLength(20).WithMessage(ValidationMessages.FIRSTNAME_MAX_LENGTH)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_FIRSTNAME);

            RuleFor(student => student.LastName)
                .MinimumLength(3).WithMessage(ValidationMessages.LASTNAME_MIN_LENGTH)
                .MaximumLength(20).WithMessage(ValidationMessages.LASTNAME_MAX_LENGTH)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_LASTNAME);

            RuleFor(student => student.Email)
                .EmailAddress().WithMessage(ValidationMessages.EMAIL_INVALID)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_EMAIL);

            RuleFor(student => student.Phone)
                .MaximumLength(10).WithMessage(ValidationMessages.PHONE_MAX_LENGTH)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_PHONE)
                .NotEqual("0000000000").WithMessage(ValidationMessages.PHONE_INVALID);

            RuleFor(student => student.DateOfBirth)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_DOB)
                .LessThan(MinimumBirthDate).WithMessage(ValidationMessages.DOB_INVALID);
        }
    }
}
