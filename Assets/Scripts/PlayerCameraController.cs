using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraController : MonoBehaviour
{
	[SerializeField] PlayerInput PlayerInput;
	[SerializeField] float MinZoom, MaxZoom, ZoomDuration, ZoomSpeed;
	[SerializeField] Text WinText, SurviveText, PlayerText;
	[SerializeField] float CameraTransitionSpeed;
	[SerializeField] PlayerCameraController OtherPlayerCamera;

	Vector3 DampVelocity;
	Vector3 LocalStart, LocalMaxZoom, LocalMinZoom;
	float LocalStartY;
	Camera camera;
	float SurvivalCounter;
	MenuManager Menu;
	bool bDoSurvivalCount;
	const string GameOverText = "You won, and lasted for X extra seconds";

	// Start is called before the first frame update
	void Start()
	{
		PlayerInput = GetComponentInParent<PlayerInput>();
		LocalStartY = transform.localPosition.y;
		WinText.enabled = false;
		SurviveText.enabled = false;
		camera = GetComponent<Camera>();
		Menu = FindObjectOfType<MenuManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (transform.parent == null && PlayerInput != null)
		{
			transform.LookAt(PlayerInput.transform.position);
		}

		if (PlayerInput != null)
		{
			LocalStart = PlayerInput.transform.position + new Vector3(0, 2, 0);
			LocalMinZoom = LocalStart - new Vector3(0, 0, MinZoom);
			LocalMaxZoom = LocalStart - new Vector3(0, 0, MaxZoom);

#if UNITY_EDITOR
			Debug.DrawRay(LocalMaxZoom + Vector3.up, Vector3.up * 2, Color.blue);
			Debug.DrawRay(LocalMinZoom + Vector3.up, Vector3.up * 2, Color.magenta);
#endif
		}

		Zoom();

		transform.localPosition = new Vector3(0, LocalStartY, Mathf.Clamp(transform.localPosition.z, -MaxZoom, MinZoom));

		UpdateSurvivalCounter();
	}

	void Zoom()
	{
		if (CanSeePlayer())
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, LocalMaxZoom, ref DampVelocity, ZoomDuration, ZoomSpeed);
		}
		else
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, LocalMinZoom, ref DampVelocity, ZoomDuration, ZoomSpeed);
		}
	}

	bool CanSeePlayer()
	{
#if UNITY_EDITOR
		Debug.DrawLine(LocalStart + transform.up, LocalStart + transform.up - transform.forward * MaxZoom, Color.green);
#endif
		if (Physics.SphereCast(LocalStart + transform.up, .5f, -transform.forward, out RaycastHit hit, MaxZoom))
		{
#if UNITY_EDITOR
			Debug.DrawRay(hit.point, Vector3.up * 2, Color.red);
#endif
			LocalMinZoom = new Vector3(0, 0, Mathf.Clamp(LocalMinZoom.z, transform.InverseTransformPoint(hit.point).z, 0));
			return false;
		}
		return true;
	}

	void UpdateSurvivalCounter()
	{
		if (!bDoSurvivalCount) return;

		SurvivalCounter += Time.deltaTime;

		SurviveText.text = string.Format("{0:0.00s}", SurvivalCounter);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(LocalStart + transform.up, .5f);
		Gizmos.DrawWireSphere(LocalStart + transform.up - transform.forward * MaxZoom, .5f);
	}
#endif

	public void Decouple()
	{
		transform.SetParent(null);
		PlayerText.enabled = false;
		Menu.NotifyPlayerDeath();
		if (OtherPlayerCamera != null)
		{
			EnableCameraTransition(true);
			OtherPlayerCamera.EnableCameraTransition();
		}
		else
		{
			StartCoroutine(IGameOverSequence());
		}
	}

	public void EnableCameraTransition(bool deleteCamera = false)
	{
		StartCoroutine(ITransitionCamera(deleteCamera));
	}

	IEnumerator ITransitionCamera(bool deleteCamera = false)
	{
		if (!deleteCamera)
			WinText.enabled = true;
		if (PlayerInput.Playertype == PlayerType.PlayerOne)
		{
			if (deleteCamera)
			{
				while (camera.rect.y < 1.0f)
				{
					Rect newRect = new Rect(new Vector2(0, camera.rect.y + Time.deltaTime * CameraTransitionSpeed), new Vector2(1, 1));
					camera.rect = newRect;
					yield return null;
				}
			}
			else
			{
				while (camera.rect.y > 0.0f)
				{
					Rect newRect = new Rect(new Vector2(0, camera.rect.y - Time.deltaTime * CameraTransitionSpeed), new Vector2(1, 1));
					camera.rect = newRect;
					yield return null;
				}
			}
		}
		else
		{
			if (deleteCamera)
			{
				while (camera.rect.height > 0.0f)
				{
					Rect newRect = new Rect(new Vector2(0, 0), new Vector2(1, camera.rect.height - Time.deltaTime * CameraTransitionSpeed));
					camera.rect = newRect;
					yield return null;
				}
			}
			else
			{
				while (camera.rect.height < 1.0f)
				{
					Rect newRect = new Rect(new Vector2(0, 0), new Vector2(1, camera.rect.height + Time.deltaTime * CameraTransitionSpeed));
					camera.rect = newRect;
					yield return null;
				}
			}
		}
		if (!deleteCamera)
		{
			WinText.enabled = false;
			SurviveText.enabled = true;
			bDoSurvivalCount = true;
		}
		else
		{
			yield return new WaitForFixedUpdate();
			Destroy(gameObject);
		}
	}

	IEnumerator IGameOverSequence()
	{
		var Wait = new WaitForFixedUpdate();
		bDoSurvivalCount = false;
		string SecondsSurvived = SurviveText.text.Remove(SurviveText.text.Length - 1);
		while (SurviveText.text.Length > 0)
		{
			SurviveText.text = SurviveText.text.Remove(SurviveText.text.Length - 1);
			yield return Wait;
		}

		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < GameOverText.Length; i++)
		{
			char Letter = GameOverText[i];

			if (Letter == 'X')
			{
				for (int j = 0; j < SecondsSurvived.Length; j++)
				{
					sb.Append(SecondsSurvived[j]);
					SurviveText.text = sb.ToString();
					yield return Wait;
				}
			}
			else
			{
				sb.Append(Letter);
				SurviveText.text = sb.ToString();
				yield return Wait;
			}
		}
		yield return new WaitForSeconds(2);
		Menu.Restart();
	}
}