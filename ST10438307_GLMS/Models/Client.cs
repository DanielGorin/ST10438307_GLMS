// client entity - one client many contracts

using System.ComponentModel.DataAnnotations;

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

    // EF uses this to join contracts back to their client
    public List<Contract> Contracts { get; set; } = new();
}