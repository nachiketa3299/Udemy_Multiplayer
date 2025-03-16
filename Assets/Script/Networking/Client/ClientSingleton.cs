
using System.Threading.Tasks;

using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
	static ClientSingleton _instance;

	public static ClientSingleton Instance
	{
		get
		{
			// 있으면 반환
			if (_instance != null)
			{
				return _instance;
			}

			// 없으면 일단 찾아봄 (비싸지만, 싱글턴마다 한번만 작동)
			_instance = FindObjectOfType<ClientSingleton>();

			// 그래도 없으면 오류
			if (_instance == null)
			{
				Debug.LogError("씬에 ClientSingleton이 없습니다.");
				return null;
			}

			return _instance;
		}
	}

	public ClientGameManager GameManager { get; private set; }

	public async Task<bool> CreateClient()
	{
		// 게임 매니저의 인스턴스 만들기
		GameManager = new ClientGameManager();

		// 뭔가 일어날 때까지 기다리기 (TODO: Await?)
		return await GameManager.InitializeAsync(); // wait
	}

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
}