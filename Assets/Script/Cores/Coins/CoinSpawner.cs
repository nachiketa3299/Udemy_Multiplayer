using Unity.Netcode;

using UnityEngine;

namespace BM
{
	public class CoinSpawner : NetworkBehaviour
	{
		[SerializeField] private RespawningCoin m_coinPrefab;

		[SerializeField] private int m_maxCoins = 50;
		[SerializeField] private int m_coinValue = 10;

		[SerializeField] private Vector2 m_xSpawnRange;
		[SerializeField] private Vector2 m_ySpawnRange;

		[SerializeField] private LayerMask m_layerMask;

		private float m_coinRadius;
		private Collider2D[] m_coinBuffer = new Collider2D[1];

		public override void OnNetworkSpawn()
		{
			if (!IsServer)
			{
				return;
			}

			m_coinRadius = m_coinPrefab.GetComponent<CircleCollider2D>().radius;

			for (int i = 0; i < m_maxCoins; ++i)
			{
				SpawnCoin();
			}
		}

		private void SpawnCoin()
		{
			var coinInstance = Instantiate
			(
				m_coinPrefab,
				GetSpawnPoint(),
				Quaternion.identity
			);

			coinInstance.SetValue(m_coinValue);
			coinInstance.GetComponent<NetworkObject>().Spawn();

			coinInstance.Collected += HandleCoinCollected;
		}

		private void HandleCoinCollected(RespawningCoin coin)
		{
			coin.transform.position = GetSpawnPoint();
			coin.Reset();
		}

		private Vector2 GetSpawnPoint()
		{
			var x = 0.0f;
			var y = 0.0f;

			while (true)
			{
				x = Random.Range(m_xSpawnRange.x, m_xSpawnRange.y);
				y = Random.Range(m_ySpawnRange.x, m_ySpawnRange.y);

				var candidateSpawnPoint = new Vector2(x, y);
				var count = Physics2D.OverlapCircleNonAlloc(candidateSpawnPoint, m_coinRadius, m_coinBuffer, m_layerMask);

				if (count == 0)
				{
					return candidateSpawnPoint;
				}
			}
		}
	}
}