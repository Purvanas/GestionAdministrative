namespace GestionAdministrative.Data;

/// <summary>
/// Constantes pour la base de données
/// </summary>
public static class DatabaseConstants
{
    public const string DatabaseFilename = "gestionadmin.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // Ouvrir en lecture/écriture
        SQLite.SQLiteOpenFlags.ReadWrite |
        // Créer si n'existe pas
        SQLite.SQLiteOpenFlags.Create |
        // Activer le multi-threading
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}
