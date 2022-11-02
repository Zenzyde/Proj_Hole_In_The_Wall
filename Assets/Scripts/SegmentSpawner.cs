using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentSpawner : MonoBehaviour
{
	[SerializeField] Transform NextSegmentPosition;
	[SerializeField] SegmentMover[] Segments;

	void OnEnable()
	{
		SpawnNextSegment();
	}

	public void SpawnNextSegment()
	{
		SegmentMover NextSegment = Segments[Random.Range(0, Segments.Length)];
		Instantiate(NextSegment, NextSegmentPosition.position, Quaternion.identity);
	}
}