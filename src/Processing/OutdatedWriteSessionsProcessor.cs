using System.Threading;
using System.Threading.Tasks;
using PinkSombrero.Core;

namespace PinkSombrero.Processing
{
	public class OutdatedWriteSessionsProcessor
	{
		public OutdatedWriteSessionsProcessor(
			UserRepository userRepository,
			MongoCredentialsService credentialsService) 
		{
			_userRepository = userRepository;
			_credentialsService = credentialsService;
		}

		public void Start()
		{
			lock (_sync)
			{
				if (_cts != null)
					return;

				_cts = new CancellationTokenSource();
				_task = revokeWriteAccess(_cts.Token);
			}
		}

		public void Stop()
		{
			Task copy;

			lock (_sync)
			{
				if (_cts == null)
					return;

				_cts.Cancel();
				_cts = null;

				copy = _task;
				_task = null;
			}

			copy.Wait();
		}

		private async Task revokeWriteAccess(CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					var users = await _userRepository.GetUsersWithOutdatedWriteAccess();

					foreach (var user in users)
					{
						if (token.IsCancellationRequested)
							return;

						await _credentialsService.RevokeWriteAccess(user);
						await _userRepository.UpdateWriteSession(user, null);
					}

					await Task.Delay(5000, token);
				}
			}
			catch (TaskCanceledException)
			{
			}
		}

		private readonly UserRepository _userRepository;
		private readonly MongoCredentialsService _credentialsService;

		private readonly object _sync = new object();
		private CancellationTokenSource _cts;
		private Task _task;
	}
}
