using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UIControllersAndData.Store.Categories.Weapon;
using UnityEngine.EventSystems;

public class TurretControllerBase : MonoBehaviour {

	public GameObject projectilePf;
	private Projectile _projectile;
	private StructureSelector _structureSelector;
	private BaseStoreItemData _structure;
	
	protected Transform 
		turretTr,
		projectileSpawnPointTr;

	[HideInInspector]
	public GameObject targetOb;//try to remove this var

	//Bullet shooting rate
	protected float 
		fireRate,
		elapsedTime;

	private float turretRotSpeed = 2.0f, turretSearchSpeed =0.05f;


	private int currentTargetIndex;
	protected Vector3 currentTargetPosition;	
	public List<GameObject> targetList = new List<GameObject> ();

	//[HideInInspector]
	public bool fire = false;

	private bool randomRotationFinished = true;
	private Vector3 randomTargetPosition;

	//Searchlight

	private bool focus = false, finished = true;//refers to flashlight zoom in/out
	private float startPoint, endPoint,startTime, transitionLength, threshold = 0.99f, transitionSpeed = 0.1f;
	public GameObject flashLight;

	protected Component soundFx;

	protected void InitializeComponents()
	{
		_structureSelector = GetComponent<StructureSelector>();
		
		var levels = ShopData.Instance.GetLevels(_structureSelector.Id, _structureSelector.CategoryType);
		if (levels != null)
		{
			_structure = levels.FirstOrDefault(x => ((ILevel) x).GetLevel() == _structureSelector.Level);
			if (_structure != null)
			{
				fireRate = ((IFireRate) _structure).GetFireRate();
				
			}
			else
			{
				Debug.LogError("Can't get structure");
			}
		}
		else
		{
			Debug.LogError("Can't get levels for structure");
		}
		
		
		turretTr = transform.Find("Turret");
		projectileSpawnPointTr = turretTr.Find("Tip");
		soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();
	}
		
	public void Aim()
	{		
		Rotate();
	}

	protected void Search()
	{
		SearchRotate ();
		Focus();
	}

	protected void UpdateTarget()
	{
		if (targetList.Count == 0)
		{
			fire = false;
			return;
		}

		if (targetList [0] != null)
			currentTargetPosition = targetList [0].transform.position;
		else
			targetList.Remove (targetList [0]);		
	}


	private void Rotate()				//this rotates the 3d Turret
	{
		Vector3 targetDir = turretTr.position - currentTargetPosition;
		
		if (targetDir != Vector3.zero)
		{
			
			float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg + 10;// 
			
			//turretTr.rotation = Quaternion.AngleAxis(angle, Vector3.forward); 								//instant rotation
			
			turretTr.rotation = Quaternion.Slerp(turretTr.rotation, 
			                                    Quaternion.AngleAxis(angle, Vector3.forward), 
			                                     Time.deltaTime * turretRotSpeed);						//slow gradual rotation					
		}
	}

	private void SearchRotate()
	{	
		if (randomRotationFinished) 
		{
			float rnd = Random.Range (-2000, 2000);
			randomTargetPosition = turretTr.position + new Vector3(rnd,rnd,rnd);
			randomRotationFinished = false;
			StartCoroutine ("NewRandomSearch");
		}
		else
		{
			float angle = Mathf.Atan2(randomTargetPosition.y, randomTargetPosition.x) * Mathf.Rad2Deg + 10;//
			//print (angle.ToString ());
			//turretTr.rotation = Quaternion.AngleAxis(angle, Vector3.forward); 								//instant rotation

			turretTr.rotation = Quaternion.Slerp(turretTr.rotation, 
				Quaternion.AngleAxis(angle, Vector3.forward), 
				Time.deltaTime * turretSearchSpeed);						//slow gradual rotation					
		}

	}

	private void Focus()
	{
		if (finished) 
		{			
			if (focus) 
			{
				startPoint = 1.0f;
				endPoint = 0.4f;

			} 
			else 
			{
				startPoint = 0.4f;
				endPoint = 1.0f;
			}

			transitionLength = Mathf.Abs (startPoint - endPoint);
			startTime = Time.time;
			//InvokeRepeating ("RunTransition", 0.02f, 0.02f);
			finished = false;
			//print ("focus");
		} 
		else 
		{
			RunTransition ();
		}


	}

	private void RunTransition()
	{
		float transitionCovered = (Time.time - startTime) * transitionSpeed;//startTime = Time.time;
		float transitionFraction = transitionCovered / transitionLength;
		float v =  Mathf.Lerp (startPoint, endPoint, transitionFraction);//lerp is clamped to 0,1

		Vector3 currentScale = flashLight.transform.localScale;
		flashLight.transform.localScale = new Vector3( currentScale.x,v,currentScale.z);
		if (transitionFraction >= threshold)
		{
			focus = !focus;
			finished = true;
		}
	}

	private IEnumerator NewRandomSearch()// the coroutine won't survive the the editor window loosing focus 
	{
		yield return new WaitForSeconds (10);
		randomRotationFinished = true;	
	}

	public void Fire()
	{
		((SoundFX)soundFx).CannonFire();   
		LaunchProjectile ();
	}

	protected void LaunchProjectile()
	{
		((SoundFX)soundFx).CannonFire();

		GameObject projectile = Instantiate(projectilePf, projectileSpawnPointTr.position, projectileSpawnPointTr.rotation);
		_projectile = projectile.GetComponent<Projectile>();
		_projectile.DamagePoints = ((IDamagePoints) _structure).GetDamagePoints();
		_projectile.DamageType = ((WeaponCategory) _structure).GetDamageType();
	}
		
	public void AddTarget(GameObject target)
	{
		targetList.Add (target);
	}
	
	public void RemoveTarget(GameObject target)
	{
		targetList.Remove (target);
	}
}
