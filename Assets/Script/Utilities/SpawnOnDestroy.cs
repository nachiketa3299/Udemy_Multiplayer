using UnityEngine;

namespace BM
{
	public class SpawnOnDestroy : MonoBehaviour
	{
		[SerializeField] private GameObject m_prefab;

		private void OnDestroy()
		{
			Instantiate(m_prefab, transform.position, Quaternion.identity);
		}
	}
}