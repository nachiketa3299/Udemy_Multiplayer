using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log("Collided");
		// ���� ��� ���������� �����Ϳ��� �����ϰ� �ڵ�� �����ϰ� ����
		Destroy(gameObject);
	}
}
