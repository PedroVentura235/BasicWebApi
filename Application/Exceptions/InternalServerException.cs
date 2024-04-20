using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class InternalServerException : CustomException
{
    public InternalServerException()
       : base("you are not authorized to access this resource.", new Collection<string>(), HttpStatusCode.InternalServerError)
    {
    }

    public InternalServerException(string message, List<string>? errors = default)
       : base(message, errors, HttpStatusCode.InternalServerError)
    {
    }
}