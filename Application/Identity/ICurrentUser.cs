using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Identity;

public interface ICurrentUser
{
    string? Name { get; }

    Guid GetUserId();

    string GetUserEmail();
}