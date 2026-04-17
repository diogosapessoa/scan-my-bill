using SQLite;

namespace ScanMyBill.Data;

public interface IDatabaseHelper
{
    SQLiteAsyncConnection Connection { get; }
}
