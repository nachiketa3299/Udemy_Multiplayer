using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
	// Ŭ���̾�Ʈ ���̵忡�� �ٷ� ���ֱ� ���� ������ �ʿ�
	[SerializeField] private SpriteRenderer m_spriteRenderer;

	protected int m_coinValue = 10;
	protected bool m_alreadyColledted; // 2 �÷��̾ ���� �����ӿ� �����ϴ� Race condition �ذ�

	public abstract int Collect();

	public void SetValue(int value)
	{
		m_coinValue = value;
	}

	protected void Show(bool show)
	{
		m_spriteRenderer.enabled = show;
	}
}
