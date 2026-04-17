using SQLite;

namespace ScanMyBill.Data;

public sealed class DatabaseHelper : IDatabaseHelper
{
    public const string DatabaseName = "DATABASE.db";

    public SQLiteAsyncConnection Connection { get; private set; }

    public DatabaseHelper()
    {
        var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var databasePath = Path.Combine(directory, DatabaseName);

        Connection = new SQLiteAsyncConnection(databasePath);
    }
}
