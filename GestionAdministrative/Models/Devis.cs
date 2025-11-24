using SQLite;

namespace GestionAdministrative.Models;

/// <summary>
/// Entité représentant un devis
/// </summary>
public class Devis : BaseEntity
{
    [MaxLength(50)]
    public string Numero { get; set; } = string.Empty; // DEV-YYYY-XXXX

    [Indexed]
    public int ClientId { get; set; }

    public DateTime DateEmission { get; set; } = DateTime.Now;

    public DateTime? DateValidite { get; set; }

    [MaxLength(20)]
    public string Statut { get; set; } = "Brouillon"; // Brouillon, Envoyé, Accepté, Refusé, Converti

    public decimal MontantHT { get; set; }

    public decimal MontantTVA { get; set; }

    public decimal MontantTTC { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public int? FactureId { get; set; } // Si converti en facture

    // Navigation properties
    [Ignore]
    public Client? Client { get; set; }

    [Ignore]
    public List<DevisLigne> Lignes { get; set; } = new();
}
