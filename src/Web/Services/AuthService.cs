using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PinkSombrero.Core;

namespace PinkSombrero.Web.Services
{
	public class AuthService
	{
		public const string Scheme = "LDAP";
		public const string AdminRoleName = "Admin";

		public AuthService(ActiveDirectoryService adService, UserRepository userRepository)
		{
			_adService = adService;
			_userRepository = userRepository;
		}

		public async Task<User> SignInAsync(HttpContext ctx, string login, string password)
		{
			string normalizedName;

			if (!_adService.ValidateCredentials(login, password, out normalizedName))
				return null;

			var user = await _userRepository.GetAsync(normalizedName);
			if (user == null)
			{
				var isFirstUser = ! await _userRepository.HasAnyUsers();
				user = new User(normalizedName, isAdmin: isFirstUser);
				await _userRepository.InsertAsync(user);
			}

			return await signInAsync(ctx, user);
		}

		public Task SignOutAsync(HttpContext ctx)
		{
			return ctx.Authentication.SignOutAsync(Scheme);
		}

		private async Task<User> signInAsync(HttpContext ctx, User user)
		{
			var identity = new ClaimsIdentity(
				new[]
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, user.AccessRights == AccessRights.Admin ? AdminRoleName : "User")
				},
				Scheme);

			await ctx.Authentication.SignInAsync(Scheme, new ClaimsPrincipal(identity));
			return user;
		}

		private readonly ActiveDirectoryService _adService;
		private readonly UserRepository _userRepository;
	}
}