using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[SerializeField] PlayerType Player;
	[SerializeField] float MoveSpeed, JumpForce;

	public Vector3 Position { get { return position; } }
	private Vector3 position;

	public PlayerType Playertype { get { return Player; } }

	// Update is called once per frame
	void Update()
	{
		position = Vector3.zero;

		if (Player == PlayerType.PlayerOne)
		{
			if (Input.GetKey(KeyCode.W))
				position += Vector3.forward;
			if (Input.GetKey(KeyCode.S))
				position -= Vector3.forward;
			if (Input.GetKey(KeyCode.D))
				position += Vector3.right;
			if (Input.GetKey(KeyCode.A))
				position -= Vector3.right;
			if (Input.GetKeyDown(KeyCode.Space))
				position += Vector3.up;
		}
		else if (Player == PlayerType.PlayerTwo)
		{
			if (Input.GetKey(KeyCode.UpArrow))
				position += Vector3.forward;
			if (Input.GetKey(KeyCode.DownArrow))
				position -= Vector3.forward;
			if (Input.GetKey(KeyCode.RightArrow))
				position += Vector3.right;
			if (Input.GetKey(KeyCode.LeftArrow))
				position -= Vector3.right;
			if (Input.GetKeyDown(KeyCode.RightControl))
				position += Vector3.up;
		}

		position.Normalize();
		position.x *= MoveSpeed;
		position.y *= JumpForce;
		position.z *= MoveSpeed;
	}
}

public enum PlayerType
{
	PlayerOne,
	PlayerTwo
}