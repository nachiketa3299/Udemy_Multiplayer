using System;
using System.Collections;

using UnityEngine;

public class Lifetime : MonoBehaviour
{
	[SerializeField] float m_maxLifetime = 3.0f;

	[Obsolete] float m_elapsedLifetime = default;

	void Start()
	{
		Destroy(gameObject, m_maxLifetime);
	}


	[Obsolete]
	IEnumerator LifetimeTimerRoutine()
	{
		m_elapsedLifetime = 0.0f;

		while (m_elapsedLifetime < m_maxLifetime)
		{
			m_elapsedLifetime += Time.deltaTime;
			yield return null;
		}

		m_elapsedLifetime = m_maxLifetime;

		Destroy(gameObject);
	}
}
