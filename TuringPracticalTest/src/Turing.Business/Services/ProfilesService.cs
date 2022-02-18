using AutoMapper;
using Optional;
using System.Threading.Tasks;
using Turing.Core;
using Turing.Core.Models;
using Turing.Core.Services;
using Turing.Data.EntityFramework;

namespace Turing.Business.Services
{
    public class ProfilesService : BaseService, IProfilesService
    {
        public ProfilesService(
            IUsersService usersService,
            ApplicationDbContext dbContext,
            IMapper mapper)
            : base(dbContext)
        {
        }

        /// <inheritdoc />
        public Task<Option<UserProfileModel, Error>> FollowAsync(string followerId, string userToFollowUsername)
        {
            // Write your solution here.
            return null;
        }

        /// <inheritdoc />
        public Task<Option<UserProfileModel, Error>> UnfollowAsync(string followerId, string userToUnfollowUsername)
        {
            // Write your solution here.
            return null;
        }

        /// <inheritdoc />
        public Task<Option<UserProfileModel, Error>> ViewProfileAsync(Option<string> viewingUserId, string profileUsername)
        {
            // Write your solution here.
            return null;
        }
    }
}