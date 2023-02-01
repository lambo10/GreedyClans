using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMesh : MonoBehaviour
{
	[SerializeField]
	private GameObject filter;


	[SerializeField]
	private MeshFilter[] meshFilters;

	void Start()
	{
		//MeshFilter[] meshFilters = filter.GetComponentsInChildren<MeshFilter>();
		meshFilters = filter.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		Matrix4x4 pTransform = transform.worldToLocalMatrix;

		while (i < meshFilters.Length)
		{
			//if(meshFilters[i] != null)
			{
				combine[i].mesh = meshFilters[i].sharedMesh;
//				combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

				combine[i].transform = pTransform * meshFilters[i].transform.localToWorldMatrix;

				meshFilters[i].gameObject.active = false;

			}
			i++;	
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
		transform.gameObject.active = true; 
	}
}