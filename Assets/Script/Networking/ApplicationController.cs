using System.Threading.Tasks;

using UnityEngine;

public class ApplicationController : MonoBehaviour
{
	[SerializeField] ClientSingleton _clientSingletonPrefab;
	[SerializeField] HostSingleton _hostSingletonPrefab;

	// Start는 async 
	async void Start()
	{
		DontDestroyOnLoad(gameObject);

		// 현재 실행 중인 환경이 Dedicated Server인지 판별
		// Dedicated Server에는 그래픽이 필요하지 않기 때문
		bool isDedicatedServer 
			= SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;

		// Dedicated Server 여부에 따라서 실행
		// 비동기로 실행되므로 초기화 작업 끝날 때까지 기다릴 수 있음
		await LaunchInMode(isDedicatedServer);
	}

	async Task LaunchInMode(bool isDedicatedServer)
	{
		if (isDedicatedServer)
		{
		}
		else
		{
			// 현재 데디서버가 아니라면,
			// 클라이언성 싱글턴과 호스트 싱글턴을 둘 다 생성

			// 호스트 싱글턴 생성 (TODO: 왜 생성이 안됨?) // 먼저 생성해야 아래 await에서 걸림?
			HostSingleton hostSingleton = Instantiate(_hostSingletonPrefab);
			hostSingleton.CreateHost();

			// 클라이언트 싱글턴 생성
			ClientSingleton clientSignleton = Instantiate(_clientSingletonPrefab);
			// Authenticate가 일어날 때 가지 기다려야함
			bool isAuthenticated = await clientSignleton.CreateClient(); // wait


			// 클라이언트가 성공적으로 Authenticated 되었으면, 메인 메뉴로 가면 됨
			if (isAuthenticated)
			{
				clientSignleton.GameManager.GoToMenu();
			}
		}
	}
}
