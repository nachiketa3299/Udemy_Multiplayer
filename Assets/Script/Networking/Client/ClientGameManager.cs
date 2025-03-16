using System.Threading.Tasks;

using Unity.Services.Core;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
	const string MenuSceneName = "Menu";
	// Async method - just a method that you call and it can last an unknown amount of time
	// does not freeze your game

	// 클라이언트 게임 매니저를 초기화하기 위한 코드
	// Authentication 같은 일을 함
	public async Task<bool> InitializeAsync()
	{
		// 유니티 서비스에 접근
		await UnityServices.InitializeAsync(); // Connect to Unity Service
		AuthState authState = await AuthenticationWrapper.DoAuthenticate();

		// 성공
		if (authState == AuthState.Authenticated)
		{
			return true;
		}

		return false; // bool 반환하니까 Task<bool>임
	}

	public void GoToMenu()
	{
		// Normal Scene Change
		SceneManager.LoadScene(MenuSceneName);
	}
}