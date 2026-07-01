using AuthenticationService.Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LogoutDto> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var activeTokens = await _unitOfWork.RefreshTokens.GetQueryable(ignoreQueryFilters: false)
                .Where(t => t.UserId == request.UserId && !t.RevokedAt.HasValue && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                await _unitOfWork.RefreshTokens.UpdateAsync(token, cancellationToken);
            }

            if (activeTokens.Any())
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return new LogoutDto
            {
                LoggedOut = true
            };
        }
    }
}
