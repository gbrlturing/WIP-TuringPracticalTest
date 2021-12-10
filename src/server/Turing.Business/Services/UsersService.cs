using AutoMapper;
using Turing.Core;
using Turing.Core.Identity;
using Turing.Core.Models;
using Turing.Core.Services;
using Turing.Data.Entities;
using Turing.Data.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Optional;
using Optional.Async;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Turing.Business.Services
{
    public class UsersService : BaseService, IUsersService
    {
        public UsersService(
            ApplicationDbContext dbContext,
            UserManager<User> userManager,
            IJwtFactory jwtFactory,
            IMapper mapper)
            : base(dbContext)
        {
            UserManager = userManager;
            JwtFactory = jwtFactory;
            Mapper = mapper;
        }

        protected UserManager<User> UserManager { get; }
        protected IJwtFactory JwtFactory { get; }
        protected IMapper Mapper { get; }

        public Task<Option<UserModel>> GetByIdAsync(string userId) =>
            GetUserById(userId.SomeNotNull())
                .MapAsync(async user => Mapper.Map<UserModel>(user));

        public Task<Option<UserModel, Error>> LoginAsync(CredentialsModel model) =>
            GetUser(u => u.Email == model.Email)
                .FilterAsync<User, Error>(user => UserManager.CheckPasswordAsync(user, model.Password), "Invalid credentials.")
                .MapAsync(
                    async user =>
                    {
                        var result = Mapper.Map<UserModel>(user);

                        result.Token = GenerateToken(user.Id, user.Email);

                        return result;
                    });

        public async Task<Option<UserModel, Error>> RegisterAsync(RegisterUserModel model)
        {
            var user = Mapper.Map<User>(model);

            var creationResult = await UserManager.CreateAsync(user, model.Password)
                .SomeWhen<IdentityResult, Error>(
                    x => x.Succeeded,
                    x => x.Errors.Select(e => e.Description).ToArray());

            return await creationResult.MapAsync(async _ =>
            {
                var result = Mapper.Map<UserModel>(user);
                result.Token = GenerateToken(user.Id, user.Email);
                return result;
            });
        }

        private string GenerateToken(string userId, string email) =>
            JwtFactory.GenerateEncodedToken(userId, email, new List<Claim>());
    }
}
