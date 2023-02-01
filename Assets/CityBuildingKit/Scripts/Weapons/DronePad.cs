using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.Serialization;

public class DronePad : MonoBehaviour {
	
	[FormerlySerializedAs("drone")] public GameObject dronePrefab;

	private GameObject _createdDrone;

	public GameObject CreatedDrone
	{
		get => _createdDrone;
		set => _createdDrone = value;
	}

	[SerializeField] private StructureSelector _structureSelector;

	public void LaunchDrone(int id, int level, ShopCategoryType categoryType)
	{
		if (!CreatedDrone)
		{
			CreatedDrone = Instantiate(dronePrefab, transform.position+ new Vector3(0,0,-5), Quaternion.identity);
			if (int.Parse(Helper.chainId) != 4000)
			{
				ServerRpc.SpawnObjectServerRpc(CreatedDrone);
			}
			else
			{
				ServerRpc.removeNetworkObject(CreatedDrone);
			}
		}
		StructureSelector dronFlyerStructureSelector = CreatedDrone.GetComponent<StructureSelector>();
		if (dronFlyerStructureSelector != null)
		{
			dronFlyerStructureSelector.Id = id;
			dronFlyerStructureSelector.Level = level;
			dronFlyerStructureSelector.CategoryType = categoryType;
		}
		CreatedDrone.transform.parent = GameObject.Find ("GroupWeapons").transform;
	}

}
