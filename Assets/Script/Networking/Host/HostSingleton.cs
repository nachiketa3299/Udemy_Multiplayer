using UnityEngine;

public class HostSingleton : MonoBehaviour
{
	static HostSingleton _instance;
	public static HostSingleton Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}

			_instance = FindObjectOfType<HostSingleton>();

			if (_instance == null)
			{
				Debug.LogWarning("씬에 HostSingleton이 없습니다.");
				return null;
			}

			return _instance;
		}
	}

	public HostGameManager GameManager { get; private set; }

	// Host 생성은 Async 할 필요가 없음
	public void CreateHost()
	{
		GameManager = new HostGameManager();
	}

	void Start()
	{
		Debug.Log("호스트 생성됨");
		DontDestroyOnLoad(gameObject);
	}
}