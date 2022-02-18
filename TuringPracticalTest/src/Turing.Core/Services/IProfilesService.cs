using Turing.Core.Models;
using Optional;
using System.Threading.Tasks;

namespace Turing.Core.Services
{
    public interface IProfilesService
    {
        /// <summary>
        /// Follows a user.
        /// </summary>
        /// <param name="followerId">The follower id.</param>
        /// <param name="userToFollowUsername">The user to follow.</param>
        /// <returns>The followed user's profile or an error.</returns>
        Task<Option<UserProfileModel, Error>> FollowAsync(string followerId, string userToFollowUsername);

        /// <summary>
        /// Unfollows a user.
        /// </summary>
        /// <param name="followerId">The follower id.</param>
        /// <param name="userToUnfollowUsername">The user to follow.</param>
        /// <returns>The unfollowed user's profile or an error.</returns>
        Task<Option<UserProfileModel, Error>> UnfollowAsync(string followerId, string userToUnfollowUsername);

        /// <summary>
        /// Retrieves a user's profile by username.
        /// </summary>
        /// <param name="viewingUserId">The id of the user viewing the profile.</param>
        /// <param name="profileUsername">The username to look for.</param>
        /// <returns>A user profile or not found.</returns>
        Task<Option<UserProfileModel, Error>> ViewProfileAsync(Option<string> viewingUserId, string profileUsername);
    }
}
