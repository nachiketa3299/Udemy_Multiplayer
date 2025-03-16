using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
	// 클라이언트 사이드에선 바로 없애기 위해 참조가 필요
	[SerializeField] private SpriteRenderer m_spriteRenderer;

	protected int m_coinValue = 10;
	protected bool m_alreadyColledted; // 2 플레이어가 같은 프레임에 접근하는 Race condition 해결

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
