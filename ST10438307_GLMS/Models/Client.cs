using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace ST10438307_GLMS.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ContactDetails { get; set; } = string.Empty;

    [Required]
    public string Region { get; set; } = string.Empty;

    // 1 Client many Contracts
    public List<Contract> Contracts { get; set; } = new();
}