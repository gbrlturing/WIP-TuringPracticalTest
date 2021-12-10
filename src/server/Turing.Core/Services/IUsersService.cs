using Turing.Core.Models;
using Optional;
using System.Threading.Tasks;

namespace Turing.Core.Services
{
    public interface IUsersService
    {
        Task<Option<UserModel, Error>> LoginAsync(CredentialsModel model);

        Task<Option<UserModel, Error>> RegisterAsync(RegisterUserModel model);

        Task<Option<UserModel>> GetByIdAsync(string userId);
    }
}
