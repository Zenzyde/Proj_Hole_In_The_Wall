using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	PlayerInput PlayerInput;
	Rigidbody Rigidbody;

	bool bIsGrounded;

	PlayerCameraController CameraController;

	// Start is called before the first frame update
	void Start()
	{
		PlayerInput = GetComponent<PlayerInput>();
		Rigidbody = GetComponent<Rigidbody>();
		CameraController = GetComponentInChildren<PlayerCameraController>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Vector3 Movement = new Vector3(PlayerInput.Position.x, 0, PlayerInput.Position.z) * Time.fixedDeltaTime;
		Rigidbody.position += Movement;
		Vector3 Jump = new Vector3(0, PlayerInput.Position.y, 0);

		bIsGrounded = Physics.SphereCast(transform.position + Vector3.up, 0.4f, Vector3.down, out RaycastHit hit, .5f);
		if (bIsGrounded)
		{
			Rigidbody.AddForce(Jump, ForceMode.VelocityChange);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.name.Contains("Death"))
			CameraController.Decouple();
	}

	void OnTriggerExit(Collider other)
	{
		if (other.name.Contains("Death"))
			Destroy(gameObject);
	}
}