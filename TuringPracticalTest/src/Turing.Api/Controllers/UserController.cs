using Turing.Core;
using Turing.Core.Models;
using Turing.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Turing.Api.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private readonly IUsersService _usersService;

        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Returns the current logged in user.
        /// </summary>
        /// <returns>The current logged in user.</returns>
        /// <response code="200">Returns the currently logged in user.</response>
        /// <response code="404">No user was found (this would mean that something is wrong with the claims).</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrentUser() =>
            (await _usersService.GetByIdAsync(CurrentUserId))
            .Match<IActionResult>(user => Ok(new { user }), NotFound);
    }
}
