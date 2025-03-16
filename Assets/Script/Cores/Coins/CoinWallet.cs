using Unity.Netcode;

using UnityEngine;

// �÷��̾ ������ �󸶳� ������ �ִ�����, ������ ��Ʈ�� �ؾ� �� ���� 
// ������ �÷��̾ �� ������ �˾ƾ� �� -> NetworkVariable �� �� ����! (�̰� ������ ��� �����̺��� �� �Ǵ� �ǰ�?)
public class CoinWallet : NetworkBehaviour
{
	public NetworkVariable<int> TotalCoins = new();

	private void OnTriggerEnter2D(Collider2D collider2D)
	{
		if (!collider2D.TryGetComponent<Coin>(out var coin))
		{
			return;
		}

		// ���ǿ��� ���� ���� ������ ����
		// Collect �� "������ ����"�鼭 "���� ��ȯ" �ϰ� ����
		var coinValue = coin.Collect();

		if (!IsServer)
		{
			return;
		}

		TotalCoins.Value += coinValue;
	}

	public void SpendCoins(int requestedValue)
	{
		TotalCoins.Value -= requestedValue;
	}
}