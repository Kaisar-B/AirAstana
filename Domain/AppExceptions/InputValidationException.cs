using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AppExceptions;
public class InputValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public InputValidationException(IDictionary<string, string[]> errors) : base("Произошла одна или несколько ошибок при проверке данных.")
    {
        Errors = errors;
    }

}
