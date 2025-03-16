using System.Threading.Tasks;

using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

// MonoBehaviour는 Static일 수 없음
public static class AuthenticationWrapper
{
	public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

	// 네트워크 거치는거는 1프레임 이상 걸리므로 비동기로
	static async Task SignInAnonymouslyAsync(int maxRetries)
	{
		int retries = 0;

		while (AuthState == AuthState.Authenticating && retries < maxRetries)
		{
			try
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Do for no auth. (no account)

				if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
				{
					AuthState = AuthState.Authenticated;
					break;
				}
			}
			catch (AuthenticationException exception)
			{
				Debug.LogError(exception);
				AuthState = AuthState.Error;
			}
			catch (RequestFailedException exception) // Connection
			{
				Debug.LogError(exception);
				AuthState = AuthState.Error;
			}

			++retries;

			await Task.Delay(1000); // if it fails, wait for 1s
		}

		if (AuthState != AuthState.Authenticated)
		{
			Debug.LogWarning($"Player was not signed in successfully after {retries} retries");
			AuthState = AuthState.TimeOut;
		}
	}

	public static async Task<AuthState> DoAuthenticate(int maxRetries = 5)
	{
		// 이미 Auth 되었으면, 상태 반환
		if (AuthState == AuthState.Authenticated)
		{
			return AuthState;
		}

		// Auth 하고 있었으면,
		if (AuthState == AuthState.Authenticating)
		{
			Debug.LogWarning("Aready authenticating!");
			await Authenticating();

			return AuthState;
		}

		AuthState = AuthState.Authenticating;

		int tries = 0;
		while (AuthState == AuthState.Authenticating && tries < maxRetries) 
		{
			// 프로토타이핑에 좋음 
			await AuthenticationService.Instance.SignInAnonymouslyAsync();

			// 모든 게 다 준비된 것
			if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
			{
				AuthState = AuthState.Authenticated;
				break;
			}

			++tries;
			// 바로 재도전하면 어차피 또 실패라 1초 후에 다시 도전을 추천
			await Task.Delay(1000); // 1s
		}

		return AuthState;
	}

	static async Task<AuthState> Authenticating()
	{
		while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
		{
			await Task.Delay(200); // 0.2s
		}

		return AuthState;
	}
}

public enum AuthState
{
	NotAuthenticated,
	Authenticating,
	Authenticated,
	Error,
	TimeOut,
}
