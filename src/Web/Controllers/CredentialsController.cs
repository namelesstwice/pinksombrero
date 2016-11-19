using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PinkSombrero.Core;
using System.Linq;

namespace PinkSombrero.Web.Controllers
{
	[Authorize]
	public class CredentialsController : Controller
	{
		public CredentialsController(UserRepository userRepository, MongoCredentialsService credentialsService)
		{
			_userRepository = userRepository;
			_credentialsService = credentialsService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var userName = User.Identity.Name;
			var user = await _userRepository.GetAsync(userName);

			if (user.AccessRights == AccessRights.None)
				return View("RequestAccessRights");

			if (user.DatabaseUsername == null)
				return View("CreateMongoCredentials", new CreateMongoCredentialsModel {Login = user.Id});

			return View(new GrantWriteAccessModel(user, getAvailableClusters())
			);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateMongoCredentialsModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateMongoCredentials");

			var userName = User.Identity.Name;
			var user = await _userRepository.GetAsync(userName);

			if (user.AccessRights == AccessRights.None)
				return RedirectToAction(nameof(Index));

			var dbUsername = await _credentialsService.CreateUser(user, model.Password);
			await _userRepository.UpdateDatabaseUsername(user, dbUsername);

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> GrantWriteAccess(GrantWriteAccessModel model)
		{
			var userName = User.Identity.Name;
			var user = await _userRepository.GetAsync(userName);

			if (!ModelState.IsValid)
				return View("Index", new GrantWriteAccessModel(user, getAvailableClusters()));

			var requestEndTime = DateTime.UtcNow + getExpiration(model.WriteModePeriod);
			await _userRepository.AddWriteAccessRequestAsync(new UserWriteAccessLogEntry(user, requestEndTime, model.Reason));
			await _userRepository.UpdateWriteSession(user, new DatabaseWriteSession(model.ClusterIds, requestEndTime));
			await _credentialsService.GrantWriteAccess(user, model.ClusterIds);

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> RevokeWriteAccess()
		{
			var user = await _userRepository.GetAsync(User.Identity.Name);

			await _credentialsService.RevokeWriteAccess(user);
			await _userRepository.UpdateWriteSession(user, null);

			return RedirectToAction(nameof(Index));
		}

		private TimeSpan getExpiration(Period period)
		{
			switch (period)
			{
				case Period.Minute: return TimeSpan.FromMinutes(1);
				case Period.FiveMinutes: return TimeSpan.FromMinutes(5);
				case Period.FifteenMinutes: return TimeSpan.FromMinutes(15);
				case Period.HalfAnHour: return TimeSpan.FromMinutes(30);
				default:
					throw new ArgumentOutOfRangeException(nameof(period), period, null);
			}
		}

		private SelectListItem[] getAvailableClusters()
		{
			return _credentialsService.GetClusters()
				.Select(c => new SelectListItem { Value = c.ClusterId, Text = c.ClusterId })
				.ToArray();
		}

		private readonly UserRepository _userRepository;
		private readonly MongoCredentialsService _credentialsService;
	}
}