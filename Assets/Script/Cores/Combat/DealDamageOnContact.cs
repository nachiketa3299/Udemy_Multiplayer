using Unity.Netcode;
using UnityEngine;

// Server only component only has server version
public class DealDamageOnContact : MonoBehaviour
{
	[SerializeField] private int m_damage = 5;

	private ulong m_ownerClientId; // very big Integer

	public void SetOwner(ulong ownerClientId)
	{
		m_ownerClientId = ownerClientId;
	}

	void OnTriggerEnter2D(Collider2D collider2D)
	{
		var attachedRigidbody = collider2D.attachedRigidbody;

		if (attachedRigidbody == null)
		{
			return;
		}

		// 자기 자신 소유인 경우에는 아니 로직을 처리하도록 한다.
		if (attachedRigidbody.TryGetComponent<NetworkObject>(out var networkObject))
		{
			if (m_ownerClientId == networkObject.OwnerClientId)
			{
				return;
			}
		}

		if (attachedRigidbody.TryGetComponent<Health>(out var health))
		{
			health.TakeDamage(m_damage);
		}
	}
}
