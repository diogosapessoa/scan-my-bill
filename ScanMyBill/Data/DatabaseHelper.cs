using SQLite;

namespace ScanMyBill.Data;

public sealed class DatabaseHelper
{
    public const string DatabaseName = "DATABASE.db";

    private static SQLiteAsyncConnection? _connection;

    public static SQLiteAsyncConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var databasePath = Path.Combine(directory, DatabaseName);

                _connection = new SQLiteAsyncConnection(databasePath);
            }

            return _connection;
        }
    }

    private DatabaseHelper() { }
}
