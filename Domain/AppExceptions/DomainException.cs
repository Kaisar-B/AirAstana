namespace Domain.AppExceptions;
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
