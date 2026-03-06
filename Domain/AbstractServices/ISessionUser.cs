using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AbstractServices;
public interface ISessionUser
{
    string? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}