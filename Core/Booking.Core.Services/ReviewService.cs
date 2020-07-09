using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.Entities;
using Booking.Common.Exceptions;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Services
{
    public class ReviewService : IReviewService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(BookingDBContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReviewDto> CreateReviewAsync(long userId, AddReviewDto addReviewDto, CancellationToken cancellationToken)
        {
            try
            {
                ReviewDto reviewDto = null;

                if (addReviewDto != null)
                {
                    var review = new Review
                    {
                        CreatedBy = userId.ToString(),
                        ReviewerId = userId,
                        Grade = addReviewDto.Grade,
                        PostDate = DateTime.Now,
                        Comment = addReviewDto.Comment,
                        RatedUserId = addReviewDto.RatedUserId,
                    };

                    _context.Reviews.Add(review);
                    await _context.SaveChangesAsync(cancellationToken);

                    reviewDto = await _context.Reviews.Where(x => x.Id == review.Id)
                                                      .Select(x => new ReviewDto
                                                      {
                                                          Reviewer = new ReviewerDto
                                                          {
                                                              Name = $"{x.ReviewerUser.FirstName} {x.ReviewerUser.LastName}",
                                                              ProfileImage = x.ReviewerUser.Attachments.Where(a => a.DocumentType == DocumentType.ProfileImage).Select(a => a.Data).FirstOrDefault(),
                                                              Rank = decimal.Round(x.ReviewerUser.Reviews.Sum(a => a.Grade) / x.ReviewerUser.Reviews.Count, 2, MidpointRounding.AwayFromZero)
                                                          },
                                                          PostDate = x.PostDate.ToString("MMMM dd, yyyy"),
                                                          Comment = x.Comment
                                                      })
                                                        .FirstOrDefaultAsync(cancellationToken);
                }

                return reviewDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create review service exception: ");
                throw;
            }
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsAsync(long userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId <= 0)
                    throw new NotFoundException($"UserId is null");

                return await _context.Reviews.Where(x => x.RatedUserId == userId)
                                             .Select(x => new ReviewDto
                                             {
                                                 Reviewer = new ReviewerDto
                                                 {
                                                     Name = $"{x.ReviewerUser.FirstName} {x.ReviewerUser.LastName}",
                                                     ProfileImage = x.ReviewerUser.Attachments.Where(a => a.DocumentType == DocumentType.ProfileImage).Select(a => a.Data).FirstOrDefault(),
                                                     Rank = decimal.Round(x.ReviewerUser.Reviews.Sum(a => a.Grade) / x.ReviewerUser.Reviews.Count, 2, MidpointRounding.AwayFromZero)
                                                 },
                                                 PostDate = x.PostDate.ToString("MMMM dd, yyyy"),
                                                 Comment = x.Comment
                                             })
                                             .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get reviewers data service exception:");
                throw;
            }
        }
    }
}
