using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Turing.Business.Services;
using Turing.Core.Identity;
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
            var user1 = new User { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@users.com" };
            var user2 = new User { Id = Guid.NewGuid().ToString(), UserName = "user2", Email = "user2@users.com" };

            AddUserWithEmail(user1.Email, user1);
            AddUserWithEmail(user2.Email, user2);

            _dbContext.FollowedUsers.Add(new FollowedUser
            {
                FollowerId = user1.Id,
                FollowingId = user2.Id,
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var followResp = await _profilesService.FollowAsync(user1.Id, user2.UserName);

            // Assert
            followResp.MatchSome(model => model.Id.ShouldBe(user2.Id));
        }

        [Fact]
        public async Task Follow_ReturnsError_WhenFollowingAnAlreadyFollowedUser()
        {
            // Arrange
            var user1 = new User { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@users.com" };
            var user2 = new User { Id = Guid.NewGuid().ToString(), UserName = "user2", Email = "user2@users.com" };

            AddUserWithEmail(user1.Email, user1);
            AddUserWithEmail(user2.Email, user2);

            _dbContext.FollowedUsers.Add(new FollowedUser
            {
                FollowerId = user1.Id,
                FollowingId = user2.Id,
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var followResp = await _profilesService.FollowAsync(user1.Id, user2.UserName);

            // Assert
            followResp.MatchNone(error => error.Messages?[0].ShouldBe("You are already following this user"));
        }

        [Fact]
        public async Task Follow_ReturnsError_WhenUserFollowHimself()
        {
            // Arrange
            var user1 = new User { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@users.com" };

            AddUserWithEmail(user1.Email, user1);

            // Act
            var followResp = await _profilesService.FollowAsync(user1.Id, user1.UserName);

            // Assert
            followResp.MatchNone(error => error.Messages?[0].ShouldBe("A user cannot follow himself."));
        }

        private void AddUserWithEmail(string email, User expected)
        {
            expected.Email = email;
            _dbContext.Users.Add(expected);
            _dbContext.SaveChanges();
        }
    }
}
