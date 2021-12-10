using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Optional;
using Shouldly;
using System;
using System.Threading.Tasks;
using Turing.Business.Services;
using Turing.Core.Identity;
using Turing.Core.Models;
using Turing.Data.Entities;
using Turing.Data.EntityFramework;
using Xunit;

namespace Turing.Business.Tests.Services
{
    public class ProfilesServiceTests
    {
        private readonly ProfilesService _profilesService;
        private readonly ApplicationDbContext _dbContext;
        private readonly UsersService _usersService;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IJwtFactory> _jwtFactoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly User _user1 = new User { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@users.com" };
        private readonly User _user2 = new User { Id = Guid.NewGuid().ToString(), UserName = "user2", Email = "user2@users.com" };

        public ProfilesServiceTests()
        {
            _dbContext = DbContextProvider.GetInMemoryDbContext();

            _userManagerMock = IdentityMocksProvider.GetMockUserManager();
            _jwtFactoryMock = new Mock<IJwtFactory>();
            _mapperMock = new Mock<IMapper>();
            _dbContext = DbContextProvider.GetInMemoryDbContext();

            _usersService = new UsersService(
                _dbContext,
                _userManagerMock.Object,
                _jwtFactoryMock.Object,
                _mapperMock.Object);

            _profilesService = new ProfilesService(_usersService, _dbContext, _mapperMock.Object);
        }

        [Fact]
        public async Task Follow_ReturnsCorrectData_ByDefault()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);
            await AddUserWithEmailAsync(_user2.Email, _user2);

            _mapperMock.Setup(mapper => mapper
                .Map<UserProfileModel>(It.IsAny<User>()))
                .Returns(new UserProfileModel { Id = _user2.Id });

            // Act
            var followResp = await _profilesService.FollowAsync(_user1.Id, _user2.UserName);

            // Assert
            followResp.MatchSome(model => model.Id.ShouldBe(_user2.Id));
            followResp.MatchSome(model => model.Following.ShouldBe(true));
        }

        [Fact]
        public async Task Follow_ReturnsError_WhenFollowingAnAlreadyFollowedUser()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);
            await AddUserWithEmailAsync(_user2.Email, _user2);
            await FollowUserAsync(_user1.Id, _user2.Id);

            // Act
            var result = await _profilesService.FollowAsync(_user1.Id, _user2.UserName);

            // Assert
            result.MatchNone(error => error.Messages?.Count.ShouldBeGreaterThan(0));
        }

        [Fact]
        public async Task Follow_ReturnsError_WhenUserFollowHimself()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);

            // Act
            var result = await _profilesService.FollowAsync(_user1.Id, _user1.UserName);

            // Assert
            result.MatchNone(error => error.Messages?.Count.ShouldBeGreaterThan(0));
        }

        [Fact]
        public async Task Unfollow_ReturnsCorrectData_ByDefault()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);
            await AddUserWithEmailAsync(_user2.Email, _user2);
            await FollowUserAsync(_user1.Id, _user2.Id);

            _mapperMock.Setup(mapper => mapper
                .Map<UserProfileModel>(It.IsAny<User>()))
                .Returns(new UserProfileModel { Id = _user2.Id });

            // Act
            var result = await _profilesService.UnfollowAsync(_user1.Id, _user2.UserName);

            // Assert
            result.MatchSome(model => model.Id.ShouldBe(_user2.Id));
            result.MatchSome(model => model.Following.ShouldBe(false));

            _dbContext.FollowedUsers.ShouldNotContain(new FollowedUser
            {
                FollowerId = _user1.Id,
                FollowingId = _user2.Id,
            });
        }

        [Fact]
        public async Task Unfollow_ReturnsError_WhenUnfollowingUserNotAlreadyFollowing()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);
            await AddUserWithEmailAsync(_user2.Email, _user2);

            // Act
            var result = await _profilesService.UnfollowAsync(_user1.Id, _user2.UserName);

            // Assert
            result.MatchNone(error => error.Messages?.Count.ShouldBeGreaterThan(0));
        }

        [Fact]
        public async Task Unfollow_ReturnsError_WhenUserUnfollowHimself()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);

            // Act
            var result = await _profilesService.UnfollowAsync(_user1.Id, _user1.UserName);

            // Assert
            result.MatchNone(error => error.Messages?.Count.ShouldBeGreaterThan(0));
        }

        [Fact]
        public async Task ViewProfile_ReturnsFollowedUserProfile_ByDefault()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);
            await AddUserWithEmailAsync(_user2.Email, _user2);

            _mapperMock.Setup(mapper => mapper
                .Map<UserProfileModel>(It.IsAny<User>()))
                .Returns(new UserProfileModel { Id = _user2.Id });

            // Act
            var result = await _profilesService.ViewProfileAsync(_user1.Id.SomeNotNull(), _user2.UserName);

            // Assert
            result.MatchSome(model => model.Id.ShouldBe(_user2.Id));
            result.MatchSome(model => model.Following.ShouldBe(false));
        }

        [Fact]
        public async Task ViewProfile_ReturnsUnfollowedUserProfile_ByDefault()
        {
            // Arrange
            await AddUserWithEmailAsync(_user1.Email, _user1);
            await AddUserWithEmailAsync(_user2.Email, _user2);
            await FollowUserAsync(_user1.Id, _user2.Id);

            _mapperMock.Setup(mapper => mapper
                .Map<UserProfileModel>(It.IsAny<User>()))
                .Returns(new UserProfileModel { Id = _user2.Id });

            // Act
            var result = await _profilesService.ViewProfileAsync(_user1.Id.SomeNotNull(), _user2.UserName);

            // Assert
            result.MatchSome(model => model.Id.ShouldBe(_user2.Id));
            result.MatchSome(model => model.Following.ShouldBe(true));
        }

        private async Task FollowUserAsync(string userId1, string userId2)
        {
            var fo = new FollowedUser
            {
                FollowerId = userId1,
                FollowingId = userId2,
            };

            _dbContext.FollowedUsers.Add(fo);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(fo).State = EntityState.Detached;
        }

        private async Task UnfollowUserAsync(string userId1, string userId2)
        {
            _dbContext.FollowedUsers.Remove(new FollowedUser
            {
                FollowerId = userId1,
                FollowingId = userId2,
            });
            await _dbContext.SaveChangesAsync();
        }

        private async Task AddUserWithEmailAsync(string email, User expected)
        {
            expected.Email = email;
            _dbContext.Users.Add(expected);
            await _dbContext.SaveChangesAsync();
        }
    }
}
