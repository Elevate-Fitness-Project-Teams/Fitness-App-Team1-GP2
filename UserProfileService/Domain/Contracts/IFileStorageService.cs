using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.UserProfileService.Domain.Contracts
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
    }
}
