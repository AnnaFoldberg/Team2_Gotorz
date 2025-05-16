namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a claim used for data transfer between the client and server.
    /// </summary>
    /// <author>Eske</author>
    public class ClaimDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
