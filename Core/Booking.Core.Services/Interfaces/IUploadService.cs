using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Services.Interfaces
{
    public interface IUploadService
    {
        Task UploadFilesAsync(long userId, IEnumerable<UploadFileDto> files, CancellationToken cancellationToken);

        Task UploadFileAsync(long userId, UploadFileDto uploadFileDto, CancellationToken cancellationToken);

        Task UpdateFileAsync(long userId, long attachmentId, UploadFileDto updateFileDto, CancellationToken cancellationToken);

        Task<IEnumerable<GetFileDto>> GetFilesAsync(long userId, IEnumerable<DocumentType> documentTypeIds, CancellationToken cancellationToken);

        Task<GetFileDto> GetFileAsync(long userId, DocumentType documentType, CancellationToken cancellationToken);

        Task DeleteFileAsync(long Id, CancellationToken cancellationToken);
    }
}
