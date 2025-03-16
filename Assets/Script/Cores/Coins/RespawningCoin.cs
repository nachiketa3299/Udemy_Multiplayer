using System;
using UnityEngine;

public class RespawningCoin : Coin
{
	public event Action<RespawningCoin> Collected;

	private Vector3 m_previousPosition;

	private void Update()
	{
		if (transform.position == m_previousPosition)
		{
			return;
		}

		m_previousPosition = transform.position;
		Show(true);
	}

	public override int Collect()
	{
		if (!IsServer)
		{
			Show(false);

			return 0;
		}

		if (m_alreadyColledted)
		{
			return 0;
		}

		// we are on the server,  we're allowed to pickup coin
		m_alreadyColledted = true;

		Collected?.Invoke(this);

		return m_coinValue;
	}

	public void Reset()
	{
		m_alreadyColledted = false;
	}
}