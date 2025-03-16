using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
	public Action<Health> Died;

	// 오직 서버만이 이 멤버를 수정할 수 있음.
	// 클라이언트가 이걸 수정하려고 하면 아무 일도 일어나지 않을 것임.
	// 그리고 서버가 이 것을 변경하면, 모든 클라이언트는 그걸 알 수 있음.
	public NetworkVariable<int> CurrentHealth = new();

	// 프로퍼티를 에디터에 노출시킬 수 있음. (Read-only)
	[field: SerializeField] public int MaxHealth { get; private set; } = 100;

	private bool m_isDead;


	public override void OnNetworkSpawn()
	{
		if (!IsServer)
		{
			return;
		}

		CurrentHealth.Value = MaxHealth;
	}

	public void TakeDamage(int damageValue)
	{
		ModifyHealth(-damageValue);
	}

	public void RestoreValue(int healValue)
	{
		ModifyHealth(healValue);
	}

	private void ModifyHealth(int value)
	{
		if (m_isDead)
		{
			return;
		}

		var newHealth = Mathf.Clamp(CurrentHealth.Value + value, 0, MaxHealth);

		if (newHealth == 0)
		{
			m_isDead = true;
			Died?.Invoke(this);
		}

		CurrentHealth.Value = newHealth;
	}

	public override void OnNetworkDespawn()
	{
	}


}

