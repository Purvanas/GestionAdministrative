using SQLite;

namespace GestionAdministrative.Models;

/// <summary>
/// Classe de base pour toutes les entités du système
/// </summary>
public abstract class BaseEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
