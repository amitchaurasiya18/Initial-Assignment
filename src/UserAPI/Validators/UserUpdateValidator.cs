using FluentValidation;
using UserAPI.DTO;
using UserAPI.StaticFiles;

namespace UserAPI.Validators
{
    public class UserUpdateValidator: AbstractValidator<UserUpdateDTO>
    {        
        public UserUpdateValidator() 
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_USERNAME)
                .MinimumLength(6).WithMessage(ValidationMessages.USERNAME_MIN_LENGTH)
                .Matches(@"(?=.*[A-Za-z])").WithMessage(ValidationMessages.USERNAME_LETTER_REQUIRED)
                .Matches(@"(?=.*\d)").WithMessage(ValidationMessages.USERNAME_DIGIT_REQUIRED);

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_EMAIL)
                .EmailAddress().WithMessage(ValidationMessages.EMAIL_INVALID);

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage(ValidationMessages.EMPTY_PASSWORD)
                .Matches(@"(?=.*[A-Z])").WithMessage(ValidationMessages.PASSWORD_UPPERCASE_REQUIRED)
                .Matches(@"(?=.*[a-z])").WithMessage(ValidationMessages.PASSWORD_LOWERCASE_REQUIRED)
                .Matches(@"(?=.*\d)").WithMessage(ValidationMessages.PASSWORD_DIGIT_REQUIRED)
                .Matches(@"(?=.*[@$!%*?&])").WithMessage(ValidationMessages.PASSWORD_SPECIAL_CHAR_REQUIRED)
                .MinimumLength(8).WithMessage(ValidationMessages.PASSWORD_MIN_LENGTH);
        }
    }
}