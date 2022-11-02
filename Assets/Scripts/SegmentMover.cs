using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMover : MonoBehaviour
{
	[SerializeField] float BaseMoveSpeed;
	[SerializeField] float AppearDuration;

	float CurrentMoveSpeed;

	List<MeshRenderer> Renderers;
	MaterialPropertyBlock MPB;

	SegmentSpawner Spawner;

	bool bHasRequestedNewSegment;

	// Start is called before the first frame update
	void Start()
	{
		Spawner = FindObjectOfType<SegmentSpawner>();
		Renderers = new List<MeshRenderer>();
		Renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
		MPB = new MaterialPropertyBlock();

		MPB.SetFloat("Vector1_41aa22721d7c4af3848bf0ab22dbe411", 0);
		foreach (MeshRenderer renderer in Renderers)
		{
			renderer.SetPropertyBlock(MPB);
		}
		StartCoroutine(IAppearLerp());
	}

	// Update is called once per frame
	void Update()
	{
		CurrentMoveSpeed += BaseMoveSpeed + Mathf.Pow(2, Mathf.Log(Time.timeSinceLevelLoad, 1.5f));
		CurrentMoveSpeed = Mathf.Log(CurrentMoveSpeed, 10);
		transform.position -= transform.forward * CurrentMoveSpeed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		if (!bHasRequestedNewSegment)
		{
			bHasRequestedNewSegment = true;
			Spawner.SpawnNextSegment();
			Destroy(gameObject);
		}
	}

	// void OnTriggerExit(Collider other)
	// {
	// 	Destroy(gameObject);
	// }

	IEnumerator IAppearLerp()
	{
		float CurrentDuration = 0.0f;
		while (CurrentDuration < AppearDuration)
		{
			CurrentDuration += Time.deltaTime * CurrentMoveSpeed;
			MPB.SetFloat("Vector1_41aa22721d7c4af3848bf0ab22dbe411", CurrentDuration / AppearDuration);
			foreach (MeshRenderer renderer in Renderers)
			{
				renderer.SetPropertyBlock(MPB);
			}
			yield return null;
		}
	}
}