using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
	[SerializeField] InputListener m_inputListListener;
	[SerializeField] Transform m_turretTransform;

	Vector2 m_previousAimPosition;

	void HandleAim(Vector2 aimInput) => m_previousAimPosition = aimInput;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
		{
			return;
		}

		m_inputListListener.AimActionTriggered += HandleAim;
	}

	void LateUpdate()
	{
		if (!IsOwner)
		{
			return;
		}

		// 마우스로 조준한 위치
		var worldAimPosition 
			= (Vector2)Camera.main.ScreenToWorldPoint(m_previousAimPosition);
		var lookDirection 
			= worldAimPosition - (Vector2)m_turretTransform.position;

		m_turretTransform.up = lookDirection;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsOwner)
		{
			return;
		}

		m_inputListListener.AimActionTriggered -= HandleAim;
	}
}
