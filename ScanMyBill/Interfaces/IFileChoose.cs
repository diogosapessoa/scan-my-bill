using ScanMyBill.Enums;
using ScanMyBill.Models;

namespace ScanMyBill.Interfaces;

public interface IFileChoose
{
    public Task<FileChooseResult> GetPdfAsync(CancellationToken cancellationToken = default);
    public Task<FileChooseResult> GetImageAsync(EFileFormat format, CancellationToken cancellationToken = default);
}
