using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
	const string MenuSceneName = "Menu";
	// Async method - just a method that you call and it can last an unknown amount of time
	// does not freeze your game

	// JoinAllocation에는 호스트가 만든 할당에 클라이언트 참여할 때 사용되는 정보가 담겨 있음
	JoinAllocation _joinAllocation;

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

	public async Task StartClientAsync(string joinCode)
	{
		try
		{
			// 릴레이 얼로케이션에 접속
			// 조인 코드 자체는 HostGameManager에서 생성
			_joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
		}
		catch (Exception exception)
		{
			Debug.Log(exception);
			return;
		}

		UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

		RelayServerData relayServerdata = new RelayServerData(_joinAllocation, "dtls");
		transport.SetRelayServerData(relayServerdata);

		NetworkManager.Singleton.StartClient();
	}
}