using Unity.Netcode;

using UnityEngine;

// 플레이어가 코인을 얼마나 가지고 있는지는, 서버가 컨트롤 해야 할 정보 
// 하지만 플레이어도 그 정보를 알아야 함 -> NetworkVariable 을 쓸 때임! (이거 쓰러면 모노 비헤이비어면 안 되는 건가?)
public class CoinWallet : NetworkBehaviour
{
	public NetworkVariable<int> TotalCoins = new();

	private void OnTriggerEnter2D(Collider2D collider2D)
	{
		if (!collider2D.TryGetComponent<Coin>(out var coin))
		{
			return;
		}

		// 강의에서 나온 아주 안좋은 패턴
		// Collect 는 "코인을 숨기"면서 "값을 반환" 하고 있음
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