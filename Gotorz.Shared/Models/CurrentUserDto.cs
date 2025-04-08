using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gotorz.Shared.Models;

public class CurrentUserDto
{
    public string? Email { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? FirstName { get; set; }
    public List<ClaimDto> Claims { get; set; } = new();
}

