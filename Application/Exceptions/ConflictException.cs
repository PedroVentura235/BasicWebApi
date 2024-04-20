using System.Collections.ObjectModel;
using System.Net;

namespace Application.Exceptions;

public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message, new Collection<string>(), HttpStatusCode.Conflict)
    {
    }
}