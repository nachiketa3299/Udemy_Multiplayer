using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
	public Action<Health> Died;

	// ���� �������� �� ����� ������ �� ����.
	// Ŭ���̾�Ʈ�� �̰� �����Ϸ��� �ϸ� �ƹ� �ϵ� �Ͼ�� ���� ����.
	// �׸��� ������ �� ���� �����ϸ�, ��� Ŭ���̾�Ʈ�� �װ� �� �� ����.
	public NetworkVariable<int> CurrentHealth = new();

	// ������Ƽ�� �����Ϳ� �����ų �� ����. (Read-only)
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

