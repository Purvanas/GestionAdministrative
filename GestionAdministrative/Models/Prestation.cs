using SQLite;

namespace GestionAdministrative.Models;

/// <summary>
/// Entité représentant une prestation / service
/// </summary>
public class Prestation : BaseEntity
{
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public decimal PrixUnitaireHT { get; set; }

    [MaxLength(10)]
    public string Unite { get; set; } = "unité"; // Ex: heure, jour, unité

    public decimal TauxTVA { get; set; } = 20m; // Par défaut 20%

    public bool EstActif { get; set; } = true;
}
