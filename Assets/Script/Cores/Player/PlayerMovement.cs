using Unity.Netcode;
using UnityEngine;



public class PlayerMovement : NetworkBehaviour
{
	[Header("References")]
	[Space()]

	[SerializeField] InputListener _inputListener;
	[SerializeField] Transform _bodyTransform;
	[SerializeField] Rigidbody2D _rigidbody2D;

	[Header("Settings")]
	[Space()]

	[SerializeField] float _movementSpeed = 4.0f;
	[SerializeField] float _turningRate = 30.0f;

	Vector2 _previousMovementInput;

	void HandleMove(Vector2 movementInput)
	{
	  _previousMovementInput = movementInput;
	}

	// [주의점]
	// Networking 쓸 때 Start는 너무 빠르다. 지연 시간에 대해 항상 생각해야 한다. -> OnNetworkSpawn
	// 반대로, OnDestroy는 너무 늦게 실행된다. -> OnNetworkDespawn
	// 모든것이 네트워크에 세팅 되었을 때에 콜백된다.
	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
		{
			return;
		}

		_inputListener.MoveActionTriggered += HandleMove;
	}

	void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		float rotationAmount = -1.0f * _turningRate * Time.deltaTime;
		float zRotation 
			= rotationAmount * _previousMovementInput.x;
		_bodyTransform.Rotate(0.0f, 0.0f, zRotation);
	}

	void FixedUpdate()
	{
		if (!IsOwner)
		{
			return;
		}

		float speed = _previousMovementInput.y * _movementSpeed;
		_rigidbody2D.velocity 
			= speed * (Vector2)_bodyTransform.up;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsOwner)
		{
			return;
		}

		_inputListener.MoveActionTriggered -= HandleMove;
	}
}
