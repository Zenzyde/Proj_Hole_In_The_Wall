using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	Canvas canvas;
	int PlayersDead;
	[SerializeField] float DeathTimer;

	float CurrentDeathTimer;

	// Start is called before the first frame update
	void Start()
	{
		canvas = GetComponent<Canvas>();
		ShowCanvas();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!canvas.enabled)
			{
				ShowCanvas();
			}
			else
			{
				HideCanvas();
			}
		}

		if (!canvas.enabled && CurrentDeathTimer > 0.0f)
			CurrentDeathTimer -= Time.deltaTime;
	}

	public void ShowCanvas()
	{
		canvas.enabled = true;
		Time.timeScale = 0;
	}

	public void HideCanvas()
	{
		canvas.enabled = false;
		Time.timeScale = 1;
	}

	public void Play()
	{
		HideCanvas();
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void NotifyPlayerDeath()
	{
		PlayersDead++;
		if (PlayersDead == 2 && CurrentDeathTimer > 0.0f)
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		CurrentDeathTimer = DeathTimer;
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}