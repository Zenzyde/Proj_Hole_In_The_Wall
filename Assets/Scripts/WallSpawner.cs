using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
	[SerializeField] Transform WallPosition;
	[SerializeField] Transform[] Walls;

	// Start is called before the first frame update
	void OnEnable()
	{
		SpawnWall();
		Destroy(this);
	}

	void SpawnWall()
	{
		Transform Wall = Walls[Random.Range(0, Walls.Length)];
		Instantiate(Wall, WallPosition.position, Quaternion.identity, transform);
	}
}