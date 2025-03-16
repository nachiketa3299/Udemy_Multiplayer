using Unity.Netcode;
using UnityEngine;

public class ConnectionButton : MonoBehaviour
{
	public void StartClient() 
	{
		NetworkManager.Singleton.StartClient();
	}

	public void StartHost()
	{
	  NetworkManager.Singleton.StartHost();
	}
}
