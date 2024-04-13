namespace AuthAi.Models.Authentication
{
    public enum ResponseStatus
    {
        Success = 0,
        EqualEmailAndPassword = 1,
        UserExists = 2,
        CantCreateUser = 3,
        UserNotExist = 4,
        WrongPassword = 5,
        EmailNotConfirmed = 6,
        WrongCode = 7,
        CodeExpired = 8,
        WrongRepeatPassword = 9,
        BadModel = 10,
        CantDeleteUser
    }
}
