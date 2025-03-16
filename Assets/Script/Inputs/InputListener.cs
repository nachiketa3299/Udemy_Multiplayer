using System;
using UnityEngine;
using UnityEngine.InputSystem;

using static Controls;

[CreateAssetMenu(fileName = "InputListener_XX", menuName = "Udemy_Multiplayer/Scriptable Objects/Input Listener")]
public class InputListener : ScriptableObject, IPlayerActions
{
	public event Action<Vector2> MoveActionTriggered;
	public event Action<bool> PrimaryFireActionTriggered;
	public event Action<Vector2> AimActionTriggered;

	Controls m_controls;

	void OnEnable()
	{
		m_controls ??= new();
		m_controls.Player.AddCallbacks(this);
		m_controls.Enable();
	}

	void OnDisable()
	{
		m_controls.Disable();
	}

	void IPlayerActions.OnMove(InputAction.CallbackContext context)
	{
		var direction = context.ReadValue<Vector2>();
		MoveActionTriggered?.Invoke(direction);
	}

	void IPlayerActions.OnPrimaryFire(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			PrimaryFireActionTriggered?.Invoke(true);
		}
		else if (context.canceled)
		{
			PrimaryFireActionTriggered?.Invoke(false);
		}
	}

	void IPlayerActions.OnAim(InputAction.CallbackContext context)
	{
		var position = context.ReadValue<Vector2>();
		AimActionTriggered?.Invoke(position);
	}

}
