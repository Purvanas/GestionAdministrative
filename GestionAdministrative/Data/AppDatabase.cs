using SQLite;
using GestionAdministrative.Models;

namespace GestionAdministrative.Data;

/// <summary>
/// Gestion de la base de données SQLite
/// </summary>
public class AppDatabase
{
    private SQLiteAsyncConnection? _connection;
    private bool _isInitialized = false;

    public SQLiteAsyncConnection Connection => _connection
        ?? throw new InvalidOperationException("La base de données n'est pas initialisée. Appelez InitAsync() d'abord.");

    /// <summary>
    /// Initialise la base de données et crée les tables
    /// </summary>
    public async Task InitAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            _connection = new SQLiteAsyncConnection(
                DatabaseConstants.DatabasePath,
                DatabaseConstants.Flags);

            // Création des tables
            await _connection.CreateTableAsync<Client>();
            await _connection.CreateTableAsync<Prestation>();
            await _connection.CreateTableAsync<Devis>();
            await _connection.CreateTableAsync<DevisLigne>();
            await _connection.CreateTableAsync<Facture>();
            await _connection.CreateTableAsync<FactureLigne>();

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            // Log l'erreur
            Console.WriteLine($"Erreur lors de l'initialisation de la base de données : {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Ferme la connexion à la base de données
    /// </summary>
    public async Task CloseAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection = null;
            _isInitialized = false;
        }
    }

    /// <summary>
    /// Supprime la base de données (utile pour les tests)
    /// </summary>
    public async Task DeleteDatabaseAsync()
    {
        await CloseAsync();

        if (File.Exists(DatabaseConstants.DatabasePath))
        {
            File.Delete(DatabaseConstants.DatabasePath);
        }
    }

    /// <summary>
    /// Retourne le chemin de la base de données
    /// </summary>
    public string GetDatabasePath() => DatabaseConstants.DatabasePath;
}
