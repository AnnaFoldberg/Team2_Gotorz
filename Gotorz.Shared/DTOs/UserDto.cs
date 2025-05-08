using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.DTOs;

/// <summary>
/// Represents the current user information used for data transfer between the client and server.
/// </summary>
/// <author>Eske</author>
public class UserDto
{
    //[JsonIgnore]
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<ClaimDto> Claims { get; set; } = new();
}