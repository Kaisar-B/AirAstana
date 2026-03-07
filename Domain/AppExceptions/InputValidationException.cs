namespace Domain.AppExceptions;
public class InputValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public InputValidationException(IDictionary<string, string[]> errors) : base("Произошла одна или несколько ошибок при проверке данных.")
    {
        Errors = errors;
    }

}
