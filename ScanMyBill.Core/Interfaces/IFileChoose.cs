using ScanMyBill.Core.Enums;
using ScanMyBill.Core.Models;

namespace ScanMyBill.Core.Interfaces;

public interface IFileChoose
{
    public Task<FileChooseResult> GetPdfAsync(CancellationToken cancellationToken = default);
    public Task<FileChooseResult> GetImageAsync(EFileFormat format, CancellationToken cancellationToken = default);
}
