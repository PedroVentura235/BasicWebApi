using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException()
       : base("you are not authorized to access this resource.", new Collection<string>(), HttpStatusCode.Unauthorized)
    {
    }

    public UnauthorizedException(string message)
       : base(message, null, HttpStatusCode.Unauthorized)
    {
    }
}