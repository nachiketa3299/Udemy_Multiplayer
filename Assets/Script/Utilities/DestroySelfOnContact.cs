using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log("Collided");
		// 뭐가 어디에 박은건지는 에디터에서 설정하고 코드는 간단하게 유지
		Destroy(gameObject);
	}
}
