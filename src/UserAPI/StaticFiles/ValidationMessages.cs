using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.StaticFiles
{
    public static class ValidationMessages
    {
        public const string PASSWORD_VALIDATION = "Password must be at least 8 characters long and contain at least one letter, one number, and one special character.";
        public const string EMPTY_EMAIL = "Email address is required";
        public const string EMAIL_INVALID = "Email Address not valid";
        public const string EMPTY_USERNAME = "Username is required";
        public const string USERNAME_MIN_LENGTH = "Username must be at least 6 characters long.";
        public const string USERNAME_LETTER_REQUIRED = "Username must contain at least one letter.";
        public const string USERNAME_DIGIT_REQUIRED = "Username must contain at least one digit.";
        public const string EMPTY_PASSWORD = "Password is required";
        public const string USERNAME_VALIDATION = "Username must contain at least one letter, one number, and be at least 6 characters long.";
        public const string PASSWORD_UPPERCASE_REQUIRED = "Password must contain at least one uppercase letter.";
        public const string PASSWORD_LOWERCASE_REQUIRED = "Password must contain at least one lowercase letter.";
        public const string PASSWORD_DIGIT_REQUIRED = "Password must contain at least one digit.";
        public const string PASSWORD_SPECIAL_CHAR_REQUIRED = "Password must contain at least one special character.";
        public const string PASSWORD_MIN_LENGTH = "Password must be at least 8 characters long.";
    }
}