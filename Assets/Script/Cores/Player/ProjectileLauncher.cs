using System.Collections;

using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
	[Header("References")][Space]

	[SerializeField] GameObject m_serverProjectilePrefab;
	[SerializeField] GameObject m_clientProjectilePrefab;
	[SerializeField] Transform m_projectileSpawnTransform;
	[SerializeField] InputListener m_inputListener;
	[SerializeField] GameObject m_muzzleFlash;
	[SerializeField] Collider2D m_playerCollider;
	[SerializeField] CoinWallet m_coinWallet;

	[Header("Settings")][Space]

	[SerializeField] float m_projectileSpeed = 10.0f;
	[SerializeField] float m_fireRate;
	[SerializeField] float m_muzzleFlashDuration;
	[SerializeField] int m_costToFire = 5;


	bool m_canFire = true;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
		{
			return;
		}

		m_inputListener.PrimaryFireActionTriggered += HandlePrimaryFire;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsOwner)
		{
			return;
		}

		m_inputListener.PrimaryFireActionTriggered -= HandlePrimaryFire;
	}

	// 발사 입력 처리
	void HandlePrimaryFire(bool shouldFire)
	{
		// 내가 누구건 이 캐릭터 소유자가 아니면 무시
		// (개별 클라이언트가 소유)
		if (!IsOwner)
		{
			return;
		}

		// 발사가 가능하다면 실행
		// (BlockFireTimerRoutine이 이 값을 조정)

		if (m_canFire)
		{
			// 코인이 부족하면 발사가 불가능
			if (m_coinWallet.TotalCoins.Value < m_costToFire)
			{
				return;
			}

			StartCoroutine(BlockFireTimerRoutine()); // 연사 속도 제한 위한 타이머 실행

			// 클라이언트는 서버에 발사 요청을 보냄
			PrimaryFire_ServerRpc(m_projectileSpawnTransform.position, m_projectileSpawnTransform.up);

			// 클라이언트에서만 보이는, 더미 총알
			SpawnDummyProjectile(m_projectileSpawnTransform.position, m_projectileSpawnTransform.up);
		}
		else
		{
			// Do nothing
		}
	}

	// 발사 속도 제한
	IEnumerator BlockFireTimerRoutine()
	{
		m_canFire = false;

		float elapsedTime = 0.0f;
		while (elapsedTime < 1.0f / m_fireRate)
		{
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		m_canFire = true;
	}

	// 서버에서 발사 처리
	[ServerRpc] // 서버에서만 실행되는 메서드임!!
	void PrimaryFire_ServerRpc(Vector3 spawnPosition, Vector3 spawnDirection)
	{
		// 코인 체크 확실히 함 (보안)
		if (m_coinWallet.TotalCoins.Value < m_costToFire)
		{
			return;
		}

		// 코인을 소모시킴
		m_coinWallet.SpendCoins(m_costToFire);

		// 서버 프로젝타일 생성 후 설정
		GameObject projectileInstance 
			= Instantiate(m_serverProjectilePrefab, spawnPosition, Quaternion.identity);
		projectileInstance.transform.up = spawnDirection;

		// 충돌 무시 (자기자신과)
		Physics2D.IgnoreCollision(m_playerCollider, projectileInstance.GetComponent<Collider2D>());

		if (projectileInstance.TryGetComponent<DealDamageOnContact>(out var dealDamgeOnContact))
		{
			// 발사체가 누구의 공격인지 기록
			// OwnerClientID는 서버 RPC를 호출한 클라이언트의 ID
			// [SeverRpc]바깥에서는 무용
			// OwnerClientID 는 각 네트워크 객체가 어느 클라이언트에게 속해 있는지 나타냄
			dealDamgeOnContact.SetOwner(OwnerClientId);
		}

		if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rigidbody2D))
		{
			rigidbody2D.velocity = rigidbody2D.transform.up * m_projectileSpeed;
		}

		// 클라이언트에 프로젝타일 생성
		SpawnDummyProjectile_ClientRpc(spawnPosition, spawnDirection);
	}

	[ClientRpc] // 클라이언트에서만 실행되는 메서드임!!
	void SpawnDummyProjectile_ClientRpc(Vector3 spawnPosition, Vector3 spawnDirection)
	{
		// "나머지 클라이언트" 들이 더미 프로젝타일을 발사하기 위한 코드
		if (IsOwner)
		{
			return;
		}

		SpawnDummyProjectile(spawnPosition, spawnDirection);
	}

	IEnumerator MuzzleFlashActiveTimerRoutine()
	{
		var elapsedTime = 0.0f;

		m_muzzleFlash.SetActive(true);

		while (elapsedTime < m_muzzleFlashDuration)
		{
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		m_muzzleFlash.SetActive(false);
	}

	private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 spawnDirection)
	{
		StartCoroutine(MuzzleFlashActiveTimerRoutine());

		// 클라이언트 프로젝타일
		GameObject projectileInstance 
			= Instantiate(m_clientProjectilePrefab, spawnPosition, Quaternion.identity);
		projectileInstance.transform.up = spawnDirection;

		Physics2D.IgnoreCollision(m_playerCollider, projectileInstance.GetComponent<Collider2D>());

		if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rigidbody2D))
		{
			rigidbody2D.velocity = rigidbody2D.transform.up * m_projectileSpeed;
		}
	}
}
