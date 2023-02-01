using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIControllersAndData.Store;

public class Projectile : MonoBehaviour	//the yellow bullet building cannons are firing
{   
	public GameObject Vortex, Sparks, Grave, Ghost; //Effects prefabs

	private int 
		ghostZ = -9,
		explosionZ = -6,
		zeroZ = 0;

    public int speed = 600;
    public float lifeTime = 3.0f;

    public int DamagePoints { get; set; }
    public DamageType DamageType { get; set; }

    private List<GameObject> _listOfTargets = new List<GameObject>();
    
	private Component soundFx, helios;

	[SerializeField] private float sizeOfColliderForNormalAttack;
	[SerializeField] private float sizeOfColliderForSplashAttack;
	
	
    void Start()
    {
		if (int.Parse(Helper.chainId) != 4000)
		{
			ServerRpc.SpawnObjectServerRpc(this.gameObject);
		}
		else
		{
			ServerRpc.removeNetworkObject(this.gameObject);
		}
		transform.parent = GameObject.Find("GroupEffects").transform;

        Destroy(gameObject, lifeTime);
		helios = GameObject.Find ("Helios").GetComponent<Helios>();
		soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();

		if (DamageType == DamageType.Splash)
		{
			gameObject.GetComponent<SphereCollider>().radius = sizeOfColliderForSplashAttack;
		}
    }

    private FighterController _fighterController;
    private Vector3 positionForInstantiation;
    private int _index;
    
    void FixedUpdate()
    {
        transform.position += 
			transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider target)
    {
	    if (target.gameObject.CompareTag("Unit"))
	    {
		    positionForInstantiation = target.transform.position;
		    _fighterController = target.GetComponent<FighterController>();
		    _index = target.GetComponent<Selector>().index;
		    _fighterController.Hit(DamagePoints, Instantiate);
	    }
    }

    private void Instantiate()
    {
	    if (_fighterController.Life > 0)
	    {
		    Instantiate(Sparks, new Vector3(positionForInstantiation.x, positionForInstantiation.y, explosionZ), Quaternion.identity); //contact.point
		    ((SoundFX) soundFx).SoldierHit();
	    }
	    else
	    {
		    Instantiate(Vortex, new Vector3(positionForInstantiation.x, positionForInstantiation.y, explosionZ), Quaternion.identity);
		    Instantiate(Grave, new Vector3(positionForInstantiation.x, positionForInstantiation.y, zeroZ), Quaternion.identity);
		    Instantiate(Ghost, new Vector3(positionForInstantiation.x, positionForInstantiation.y, ghostZ), Quaternion.identity);

		    ((Helios) helios).KillUnit(_fighterController.assignedToGroup, _index);
		    ((SoundFX) soundFx).SoldierExplode();
	    }
    }
}