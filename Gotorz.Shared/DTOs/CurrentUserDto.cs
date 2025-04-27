using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gotorz.Shared.DTOs;

/// <summary>
/// Represents the current user information used for data transfer between the client and server.
/// </summary>
/// <author>Eske</author>
public class CurrentUserDto
{
    public string? Email { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<ClaimDto> Claims { get; set; } = new();
}

