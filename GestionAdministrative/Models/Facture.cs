using SQLite;

namespace GestionAdministrative.Models;

/// <summary>
/// Entité représentant une facture
/// </summary>
public class Facture : BaseEntity
{
    [MaxLength(50)]
    public string Numero { get; set; } = string.Empty; // FAC-YYYY-XXXX

    [Indexed]
    public int ClientId { get; set; }

    public int? DevisId { get; set; } // Si issue d'un devis

    public DateTime DateEmission { get; set; } = DateTime.Now;

    public DateTime? DateEcheance { get; set; }

    [MaxLength(20)]
    public string Statut { get; set; } = "EnAttente"; // EnAttente, Payée, Impayée, PartialementPayée

    public decimal MontantHT { get; set; }

    public decimal MontantTVA { get; set; }

    public decimal MontantTTC { get; set; }

    public decimal MontantPaye { get; set; } = 0m;

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public DateTime? DatePaiement { get; set; }

    // Navigation properties
    [Ignore]
    public Client? Client { get; set; }

    [Ignore]
    public List<FactureLigne> Lignes { get; set; } = new();

    [Ignore]
    public decimal MontantRestant => MontantTTC - MontantPaye;
}
