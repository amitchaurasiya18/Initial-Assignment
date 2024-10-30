namespace SchoolAPI.StaticFiles
{
    public static class ValidationMessages
    {
        public const string EMPTY_FIRSTNAME = "FirstName is required";
        public const string EMPTY_LASTNAME = "LastName is required";
        public const string EMPTY_EMAIL = "Email address is required";
        public const string EMPTY_PHONE = "Phone number is required";
        public const string EMPTY_DOB = "Date of Birth is required";
        public const string FIRSTNAME_MIN_LENGTH = "FirstName should have minimum 3 letters";
        public const string FIRSTNAME_MAX_LENGTH = "FirstName should have only 20 letters";
        public const string LASTNAME_MIN_LENGTH = "LastName should have minimum 3 letters";
        public const string LASTNAME_MAX_LENGTH = "LastName should have only 20 letters";
        public const string EMAIL_INVALID = "Email Address not valid";
        public const string PHONE_MAX_LENGTH = "Phone number cannot exceed 10 digits";
        public const string PHONE_INVALID = "Not a valid phone number";
        public const string DOB_INVALID = "Not a Valid Date of Birth";
    }
}
