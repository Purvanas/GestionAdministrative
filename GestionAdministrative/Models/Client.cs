using SQLite;

namespace GestionAdministrative.Models;

/// <summary>
/// Entité représentant un client
/// </summary>
public class Client : BaseEntity
{
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telephone { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Adresse { get; set; } = string.Empty;

    [MaxLength(14)]
    public string? SIREN { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties (non persistées)
    [Ignore]
    public List<Devis> Devis { get; set; } = new();

    [Ignore]
    public List<Facture> Factures { get; set; } = new();
}
