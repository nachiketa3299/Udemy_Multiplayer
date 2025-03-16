using Unity.Netcode.Components;

public class ClientTransform : NetworkTransform
{
	protected override bool OnIsServerAuthoritative() => false;

	// 이게 뭐하는 함수이지?
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		CanCommitToTransform = true;
	}

	protected override void Update()
	{
		CanCommitToTransform = IsOwner;
		base.Update();

		if (NetworkManager == null)
		{
			return;
		}

		if (!(NetworkManager.IsConnectedClient && NetworkManager.IsListening))
		{
			return;
		}

		if (!CanCommitToTransform)
		{
			return;
		}

		TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
	}

}