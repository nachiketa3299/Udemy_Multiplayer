using Unity.Netcode;

using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
	[Header("References")]
	[Space()]
	[SerializeField] private Health m_health;
	[SerializeField] private Image m_healthBarImage;

	public override void OnNetworkSpawn()
	{
		if (!IsClient)
		{
			return;
		}

		// ������ ������ ��ü�� 
		// ���� Ŭ���̾�Ʈ���߸� �̰Ϳ� ������ ����
		m_health.CurrentHealth.OnValueChanged += HandleHealthChange;
		HandleHealthChange(0, m_health.CurrentHealth.Value); // Manually call for init state
	}

	public override void OnNetworkDespawn()
	{
		if (!IsClient)
		{
			return;
		}

		m_health.CurrentHealth.OnValueChanged -= HandleHealthChange;
	}

	private void HandleHealthChange(int _, int newHealth)
	{
		var healthRatio = (float)newHealth / m_health.MaxHealth;
		m_healthBarImage.fillAmount = healthRatio;
	}
}
