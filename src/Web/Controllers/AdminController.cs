using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinkSombrero.Core;
using PinkSombrero.Web.Services;

namespace PinkSombrero.Web.Controllers
{
	[Authorize(Roles = AuthService.AdminRoleName)]
	public class AdminController : Controller
	{
		public AdminController(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Users(int skip = 0)
		{
			var users = await _userRepository.GetAsync(skip, limit: 100);
			return View(users);
		}

		[HttpGet]
		public async Task<IActionResult> UpdateUser(string userId, AccessRights accessRights)
		{
			var user = await _userRepository.GetAsync(userId);
			await _userRepository.UpdateAccessRightsAsync(user, accessRights);
			return RedirectToAction(nameof(Users));
		}

		private readonly UserRepository _userRepository;
	}
}