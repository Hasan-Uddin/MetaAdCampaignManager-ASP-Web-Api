
namespace Domain.Leads;

public sealed class LeadFieldData
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public Dictionary<string, string> Extra { get; set; } = [];
}
