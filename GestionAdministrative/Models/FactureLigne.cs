using SQLite;

namespace GestionAdministrative.Models;

/// <summary>
/// Ligne d'une facture
/// </summary>
public class FactureLigne : BaseEntity
{
    [Indexed]
    public int FactureId { get; set; }

    public int? PrestationId { get; set; }

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public decimal Quantite { get; set; } = 1;

    public decimal PrixUnitaireHT { get; set; }

    public decimal TauxTVA { get; set; } = 20m;

    // Propriétés calculées
    [Ignore]
    public decimal MontantHT => Quantite * PrixUnitaireHT;

    [Ignore]
    public decimal MontantTVA => MontantHT * (TauxTVA / 100);

    [Ignore]
    public decimal MontantTTC => MontantHT + MontantTVA;

    // Navigation
    [Ignore]
    public Prestation? Prestation { get; set; }
}
