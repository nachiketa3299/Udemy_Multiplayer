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
		AuthState = AuthState.Authenticating;

		// 아예 실패할 확률이 있기 때문에 핸들링 해주어야 함
		int retries = 0;
		while (AuthState == AuthState.Authenticating && retries < maxRetries) 
		{
			try 
			{
				// 프로토타이핑에 좋음 
				await AuthenticationService.Instance.SignInAnonymouslyAsync();

				// 모든 게 다 준비된 것
				if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
				{
					AuthState = AuthState.Authenticated;
					break;
				}
			}
			// 인증 실패
			catch (AuthenticationException authException)
			{
				Debug.Log(authException);
				AuthState = AuthState.Error;
			}
			// 연결 실패
			catch (RequestFailedException requestException)
			{
				Debug.Log(requestException);
				AuthState = AuthState.Error;
			}

			++retries;
			// 바로 재도전하면 어차피 또 실패라 1초 후에 다시 도전을 추천
			await Task.Delay(1000); // 1s
		}

		if (AuthState != AuthState.Authenticated)
		{
			// 모든 트라이 했는데도 안 되었음
			Debug.LogWarning($"플레이어가 성공적으로 사인인 할 수 없었습니다. ({retries} 번 시도)");
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

		// Auth 하고 있을때 또 Auth 하라하면 어떡함?
		if (AuthState == AuthState.Authenticating)
		{
			Debug.LogWarning("이미 인증 중입니다");
			// 여기서는 그냥 기다려야함
			await Authenticating();

			return AuthState;
		}
		
		// 익명으로 maxRetries까지 인증
		await SignInAnonymouslyAsync(maxRetries);

		return AuthState;
	}

	static async Task<AuthState> Authenticating()
	{
		// 안되었거나 하는 중이면 계속 체크하며 기다리기
		while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
		{
			await Task.Delay(200); // 0.2s 유니티 샘플에서 제시하는 간격
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
