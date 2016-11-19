using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinkSombrero.Web.Services;

namespace PinkSombrero.Web.Controllers
{
	[AllowAnonymous]
	public class AuthController : Controller
	{
		public AuthController(AuthService authService)
		{
			_authService = authService;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await _authService.SignOutAsync(HttpContext);
			return RedirectToAction("Login");
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
		{
			returnUrl = returnUrl ?? "/";
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
				return View(model);

			var user = await _authService.SignInAsync(HttpContext, model.Username, model.Password);
			if (user == null)
			{
				ModelState.AddModelError(
					string.Empty, 
					"Введены неверные учётные данные");

				return View(model);
			}

			return LocalRedirect(returnUrl);
		}
		
		private readonly AuthService _authService;
	}
}