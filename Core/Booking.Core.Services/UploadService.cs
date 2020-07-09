using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Common.Exceptions;
using Booking.Core.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Booking.Core.Services
{
    public class UploadService : IUploadService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<UploadService> _logger;

        public UploadService(BookingDBContext context, ILogger<UploadService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UploadFilesAsync(long userId, IEnumerable<UploadFileDto> files, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var file in files)
                {
                    if (file.DocumentType != DocumentType.Certification)
                    {
                        var duplicateFiles = await _context.Attachments.Where(x => x.DocumentType == file.DocumentType && x.UserId == userId).ToListAsync(cancellationToken);

                        if (duplicateFiles.Any())
                            _context.Attachments.RemoveRange(duplicateFiles);
                    }

                    var attachment = new Attachment
                    {
                        CreatedBy = userId.ToString(),
                        FileName = file.File.FileName,
                        ContentType = file.File.ContentType,
                        DocumentType = file.DocumentType,
                        UserId = userId,
                    };

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.File.CopyToAsync(memoryStream);
                        attachment.Data = memoryStream.ToArray();
                    }

                    _context.Attachments.Add(attachment);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload files service exception:");
                throw;
            }
        }

        public async Task UploadFileAsync(long userId, UploadFileDto uploadFileDto, CancellationToken cancellationToken)
        {
            try
            {
                if (uploadFileDto.DocumentType != DocumentType.Certification)
                {
                    var currentProfileImage = await _context.Attachments.FirstOrDefaultAsync(x => x.DocumentType == uploadFileDto.DocumentType && x.UserId == userId, cancellationToken);

                    if (currentProfileImage != null)
                        _context.Attachments.Remove(currentProfileImage);
                }

                var attachment = new Attachment
                {
                    CreatedBy = userId.ToString(),
                    FileName = uploadFileDto.File.FileName,
                    ContentType = uploadFileDto.File.ContentType,
                    DocumentType = uploadFileDto.DocumentType,
                    UserId = userId
                };

                using (var memoryStream = new MemoryStream())
                {
                    await uploadFileDto.File.CopyToAsync(memoryStream);
                    attachment.Data = memoryStream.ToArray();
                }

                _context.Attachments.Add(attachment);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload file service exception:");
                throw;
            }
        }

        public async Task UpdateFileAsync(long userId, long attachmentId, UploadFileDto updateFileDto, CancellationToken cancellationToken)
        {
            try
            {
                var attachment = await _context.Attachments.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == attachmentId);

                if (attachment is null)
                    new NotFoundException($"Attachment with id {attachmentId} doesn't exist.");

                using (var memoryStream = new MemoryStream())
                {
                    await updateFileDto.File.CopyToAsync(memoryStream);
                    var fileByteArray = memoryStream.ToArray();

                    attachment.ModifiedBy = userId.ToString();
                    attachment.ModifiedDate = DateTime.UtcNow;
                    attachment.UserId = userId;
                    attachment.DocumentType = updateFileDto.DocumentType;
                    attachment.FileName = updateFileDto.File.FileName;
                    attachment.ContentType = updateFileDto.File.ContentType;
                    attachment.Data = fileByteArray;

                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update file service exception");
                throw;
            }
        }

        public async Task<IEnumerable<GetFileDto>> GetFilesAsync(long userId, IEnumerable<DocumentType> documentTypeIds, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Attachments.Where(x => x.UserId == userId && documentTypeIds.Contains(x.DocumentType))
                                                  .Select(x => new GetFileDto
                                                  {
                                                      Id = x.Id,
                                                      FileName = x.FileName,
                                                      Data = x.Data
                                                  })
                                                 .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get files service exception");
                throw;
            }
        }

        public async Task<GetFileDto> GetFileAsync(long userId, DocumentType documentType, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Attachments.Where(x => x.UserId == userId && x.DocumentType == documentType)
                                                 .Select(x => new GetFileDto
                                                 {
                                                     Id = x.Id,
                                                     FileName = x.FileName,
                                                     Data = x.Data
                                                 })
                                                 .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get files service exception:");
                throw;
            }
        }

        public async Task DeleteFileAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var attachment = await _context.Attachments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (attachment is null)
                    new NotFoundException($"Attachment with id {id} doesn't exist.");

                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete file service exception:");
                throw;
            }
        }
    }
}
