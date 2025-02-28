using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerimeterColliderBomb : MonoBehaviour {		//used to color the grass red when in collision
		
	public int collisionCounter = 0;				//keeps track of how many other grass patches this one is overlapping

	private int 
	ghostZ = -9,
	explosionZ = -6,
	zeroZ = 0;

	public bool inCollision = false, isExploded = false;
	private List<GameObject> victims = new List<GameObject>();

	public GameObject ExplosionPf, Vortex, Grave, Ghost; 

	private SoundFX soundFX;
	private Helios helios;
	private FighterController _fighterController;
	private Vector3 _positionForInstantiation;
	private int _index;
	void Start () 
	{
		soundFX = GameObject.Find ("SoundFX").GetComponent <SoundFX>();
		helios = GameObject.Find ("Helios").GetComponent<Helios>();
	}

	void OnTriggerEnter(Collider collider)
	{
		//print ("enter");
		//print ("collider.gameObject.tag");
		if (collider.gameObject.CompareTag("Unit") && !isExploded) 
		{	 
			collisionCounter++;						
			victims.Add (collider.gameObject);
			StartCoroutine ("Explode");
			inCollision = true;	
		}

	}

	void OnTriggerExit(Collider collider)//OnCollisionEnter(Collision collision)
	{
		if (collider.gameObject.tag == "Unit"&&!isExploded) {	 
			collisionCounter--;	
			victims.Remove(collider.gameObject);

			if(collisionCounter == 0)
			{	
				victims.Clear ();
				StopCoroutine ("Explode"); //bomb aborts explosion if targets have moved on
				inCollision = false;
			}
		}
	}

	private IEnumerator Explode()
	{
		yield return new WaitForSeconds (1.0f);
		isExploded = true;
		soundFX.BuildingExplode ();
		ExplosionPf.SetActive (true);	

		foreach (var unit in victims) {
			if (unit != null) 
			{
				_fighterController = unit.GetComponent<FighterController>();
				_positionForInstantiation = unit.transform.position;
				_index = unit.GetComponent<Selector>().index;
				_fighterController.Hit(100, Instantiate);
			}
		}
		victims.Clear ();
	}

	private void Instantiate()
	{

		Instantiate (Vortex, new Vector3 (_positionForInstantiation.x, _positionForInstantiation.y, explosionZ), Quaternion.identity);
		Instantiate (Grave, new Vector3 (_positionForInstantiation.x, _positionForInstantiation.y, zeroZ), Quaternion.identity);
		Instantiate (Ghost, new Vector3 (_positionForInstantiation.x, _positionForInstantiation.y, ghostZ), Quaternion.identity);

		helios.KillUnit (_fighterController.assignedToGroup, _index);
	}

	private IEnumerator DestroySelf()
	{
		yield return new WaitForSeconds (1.0f);
		Destroy (transform.parent.gameObject);		
	}


}
