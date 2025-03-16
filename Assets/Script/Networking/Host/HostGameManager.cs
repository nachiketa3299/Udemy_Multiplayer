using System;
using System.Threading.Tasks;

using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

using UnityEngine;
using UnityEngine.SceneManagement;

// 유니티의 릴레이 서비스를 사용하여 멀티플레이어 게임의 호스트를 설정
// 호스트는 다른 플레이어들이 접속할 수 있도록 게임을 열어주는 역할
// 릴레이를 이용하므로 데디 서버는 아니고, 플레이어중 한 명이 호스트

public class HostGameManager
{
	const int MaxConnections = 20;
	const string GameSceneName = "Game";

	Allocation _allocation;
	string _joinCode;

	// 호스트가 되기 위한 준비 과정 수행
	public async Task StartHostAsync()
	{
		try 
		{
			// Unity Relay에 지정된 규모의 릴레이 서버 할당 요청
			// UGS Allocation 위해 기다리기
			// MaxConnections 만큼의 플레이어가 접속 가능하도록 할당을 요청
			// 성공하면 _allocation에는 네트워크 정보(주소나 포트)가 저장됨
			_allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
		}
		catch (Exception exception)
		{
			Debug.Log(exception);
			return;
		}

		try 
		{
			// UGS Allocation 위해 기다리기
			// 유니티 릴레이는 Join Code를 제공하며, 이 코드를 공유하면 호스트에 접속 가능
			_joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
		}
		catch (Exception exception)
		{
			Debug.Log(exception);
			return;
		}

		// UnityTransport 가져환환서, Relay 서버를 네트워크 전송 방식으로 사용하도록 설정
		// 유니티에서는 플레이어간 데이터를 주고받는 네트워크 통신 방식이 필요함
		// 이를 담당하는 것이 Unity Transport (UTP)
		UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

		// 릴레이 서버와 연결하기
		// 릴레이 서버를 사용하려면, 호스트의 네트워크 설정을 릴레이 서버에 맞게 변경해야 함
		// 유니티 릴레이 서버에서 할당받은 _allocation 데이터를 기반으로 "이 호스트는 Unity Relay를 통해 데이터 전송할 것"이라고 설정
		// TCP 대신에 UDP 사용 (실시간 게임에서 속도가 빠르고 지연이 적은 방식이나 손실 위험)
		RelayServerData relayServerData = new RelayServerData(_allocation, "udp"); // user datagram protocoll
		unityTransport.SetRelayServerData(relayServerData);

		// Unity Relay 서비스에서 _allocation 받아오고, Unity Transport 설정에 적용

		// 릴레이 셋업으로 외부에서 접근 가능하도록 한 후 호스팅 시작
		NetworkManager.Singleton.StartHost(); // 호스트 역할 시작
		// 씬 전환
		NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
	}
}