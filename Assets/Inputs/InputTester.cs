using UnityEngine;

public class InputTester : MonoBehaviour
{
	void OnEnable()
	{
		inputListener.MoveActionTriggered += OnMoveActionTriggered;
		inputListener.PrimaryFireActionTriggered += OnPrimaryActionTriggered;
	}

	void OnDisable()
	{

	}

	void OnMoveActionTriggered(Vector2 direction)
	{
		Debug.Log($"Move Action Triggered with value {direction}");
	}

	void OnPrimaryActionTriggered(bool value)
	{
		Debug.Log($"Primary Action Triggered with value {value}");
	}

	[SerializeField] InputListener inputListener;
}
