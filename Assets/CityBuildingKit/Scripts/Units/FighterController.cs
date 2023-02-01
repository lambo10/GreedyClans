using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class FighterController : MonoBehaviour {  //controlls all soldiers

	public FighterPath path;
	public float speed = 30.0f, mass = 5.0f, unitZ = -3 ;

	private float curSpeed, pathLength;

	public Vector3 targetPoint, velocity; 

	public Vector3 targetCenter;

	private string direction = "N";

	public enum UnitState{PathWalk, Attack, Idle, Disperse}
	public UnitState currentState = UnitState.Idle;
	private AnimController fighterAnimController,shadowAnimController;	

	public bool hasShadow = true;//if shadow is on a separate sprite

	private bool 
		pause = false, 
		breakAction;
		//changeAnimation = false;//for update based animation change - no longer needed

		public int
			assignedToGroup = -1;//keeps track of which group this unit is assigned to. 
		
		private int life = 0;
		public int Life
		{
			get => life;
			set => life = value;
		}

		private int damagePoints = 0;

		public int DamagePoints
		{
			get => damagePoints;
			set => damagePoints = value;
		}

		private tk2dUIProgressBar healthBar;

	public int 
		maxLife = 100,//for progress bar percentage
		curPathIndex;
	
	private Shooter _shooter;

	public Shooter Shooter => _shooter;

	private void Awake()
	{
		_shooter = GetComponent<Shooter>();
		if (!_shooter)
		{
			Debug.LogError("Can't get the Shooter component'");
		}
	}

	void Start () {
		fighterAnimController = GetComponent<AnimController> ();
			
		if(hasShadow)
			shadowAnimController = transform.GetChild (2).GetComponent<AnimController> ();

		path = GetComponent<FighterPath> ();
		healthBar = GetComponentInChildren<tk2dUIProgressBar> ();

		pathLength = path.Length;
		curPathIndex = 0;						
		velocity = Vector3.zero;		//maintain position till you get the first path point
	}

	public void Hit(int damagePoints, Action callback)
	{
		life -= damagePoints;
		if (callback != null)
		{
			callback();
		}

		if (life > 0) 
		{
			healthBar.Value = life / (float)maxLife;
		}
		else
		{
			Destroy (gameObject);
		}
	}
	
	public void Pause()
	{
		pause = true;
	}
	public void UpdatePath()
	{
		pause = true;
		path.UpdatePath();
		pathLength = path.Length;

		/*
		skip path entry point because 
		1.possibly inside an obstacle block, and the units go up over the rubble until they reach the middle sprite "cut" edge
		2.units appear to "go back" when they receive a new order  

		since animations have 8 angles tops, this cam create a slight/brief "moonwalking" -
		the direction of the walk animation doesn't match perfectly the unit direction 

		since at the beginning the unit goes 2 grid squares till the direction is checked again, this is
		corrected with a delayed latechangeanimation, so the movement/direction doesn't appear all wrong
		*/

		if(pathLength>1)
		curPathIndex = 1;
		else
		curPathIndex=0;
		StartCoroutine(LateChangeAnimation(0.5f));

		pause = false;
	}

	public void lb_UpdatePathForAutoMovement()
	{
		pause = true;
        //path.UpdatePath();
        pathLength = path.Length;

		if (pathLength > 1)
			curPathIndex = 1;
		else
			curPathIndex = 0;
		StartCoroutine(LateChangeAnimation(0.5f));

		pause = false;

		RevertToPathWalk();
		PathWalk();

	}

	void FixedUpdate () {

		if (!pause) 
		{
			switch (currentState) 
			{
				case UnitState.PathWalk:
				PathWalk ();
				break;	
			}					
		}
	}

	private void PathWalk()
	{
		curSpeed = speed * 0.016f;//Time.deltaTime creates a speed hickup - replaced with constant

		//to go straight to AITarget targetPoint = path.GetEndPoint();

		targetPoint = path.GetPoint(curPathIndex);

		//if radius to path point reached, move to next point
		if (Vector3.Distance(transform.position, targetPoint) <
		    path.Radius) 
		{
			if (curPathIndex < pathLength - 1) 
				curPathIndex++;	
			else
			{
				if (SceneManager.GetActiveScene().name.Equals("Game") || SceneManager.GetActiveScene().name.Equals("Game2"))
				{
                }
                else {
					Debug.Log("Here ------------------------ "+ SceneManager.GetActiveScene().name);
					currentState = UnitState.Attack;
					GetComponent<Shooter>().shoot = true;
				}
			}
			
			StartCoroutine(LateChangeAnimation(0.1f));
		}

		velocity += Steer(targetPoint);

		transform.position += velocity;

	}

	public Vector3 Steer(Vector3 target)
	{	
		Vector3 pos = transform.position;
		Vector3 desiredVelocity = (target - pos);
		float dist = desiredVelocity.magnitude;//square root of (x*x+y*y+z*z)

		desiredVelocity.Normalize();//normalized, a vector keeps the same direction but its length is 1.0

		if (dist < 10.0f)
			desiredVelocity *= (curSpeed * (dist / 10.0f));//slow down close to target
		else 
			desiredVelocity *= curSpeed;

		Vector3 steeringForce = desiredVelocity - velocity;
		Vector3 acceleration = steeringForce / mass;

		transform.position += velocity;	//!!! Disregarding z can make character go back and forth below point, unable to "touch" it

		return (acceleration);
	}

	public void ChangeTarget()
	{
		IngameUpdatePath ();
	}

	private void  IngameUpdatePath()
	{
		
		moveRandomly mr = GetComponent<moveRandomly>();
        if (mr == null)
        {
			UpdatePath();
		}
        else
        {
			lb_UpdatePathForAutoMovement();
		}
		

		StartCoroutine(LateChangeAnimation(0.1f));
		//changeAnimation = true;
	}

	private void ChangeToPathWalk()
	{
		if (currentState != UnitState.Attack||breakAction) 
		{	
			currentState = UnitState.PathWalk;
			StartCoroutine(LateChangeAnimation(0.1f));
			//changeAnimation = true;
			breakAction = false;
		}
	}

	public void RevertToPathWalk()//called by BattleProc
	{
		breakAction = true;
		this.GetComponent<Shooter> ().shoot = false;
		ChangeToPathWalk ();
	}

	public void RevertToIdle()
	{
		currentState = UnitState.Idle;
		this.GetComponent<Shooter> ().shoot = false;
		ChangeAnimation ();
	}


	private IEnumerator LateChangeAnimation(float time)
	{
		yield return new WaitForSeconds (time);//0.1f standard, 0.5 for late
		SetDirection ();
	}


	private void ChangeAnimation()
	{
		SetDirection ();
	}

	private void SetDirection()
	{
		switch (currentState) {
		
		case UnitState.PathWalk:
			SpeedToDirection ();
			fighterAnimController.ChangeAnim ("Walk");
			if(hasShadow)
			shadowAnimController.ChangeAnim ("Walk_Shadow");
			break;		

		case UnitState.Attack:
			SetRelativeDirection();
			fighterAnimController.ChangeAnim("Attack");
			if(hasShadow)
			shadowAnimController.ChangeAnim ("Attack_Shadow");
			break;		

		case UnitState.Idle:			
			//SetRelativeDirection();
			fighterAnimController.ChangeAnim("Idle");
			if(hasShadow)
			shadowAnimController.ChangeAnim ("Idle_Shadow");
			break;
		}		
		
		fighterAnimController.Turn(direction);
		fighterAnimController.UpdateCharacterAnimation();

		if (hasShadow) 
		{
			switch (currentState) {
			case UnitState.PathWalk:				
				shadowAnimController.ChangeAnim ("Walk_Shadow");
				break;		

			case UnitState.Attack:				
				shadowAnimController.ChangeAnim ("Attack_Shadow");
				break;		

			case UnitState.Idle:			
				shadowAnimController.ChangeAnim ("Idle_Shadow");
				break;
			}	
			shadowAnimController.Turn (direction);
			shadowAnimController.UpdateCharacterAnimation ();
		}
	}

	private void SpeedToDirection()//threshdold 0.05f for "if" (if run at next update cycle) 0.2f for delayed coroutine
	{
		if(Mathf.Abs(velocity.x)>0.2f)//high X speed
		{
			if(Mathf.Abs(velocity.y)>0.2f) //high y speed
			{					
				if(velocity.x>0)
				{
					if(velocity.y>0) direction="NE";
					else direction="SE";
				}
				
				else
				{
					if(velocity.y>0) direction="NW";
					else direction="SW";
				}
			}
			else //low y speed
			{
				if(velocity.x>0) direction="E";					
				else direction="W";
			}
			
		}
		
		else 
		{
			if(Mathf.Abs(velocity.y)>0.2f) //high y speed
			{
				if(velocity.y>0) direction="N";
				else direction="S";					
			}
			else //low y speed
			{
				if(velocity.x>0)
				{
					if(velocity.y>0) direction="NE";
					else direction="SE";
				}
				
				else
				{
					if(velocity.y>0) direction="NW";
					else direction="SW";
				}
			}
		}
	}

	private void SetRelativeDirection()//the unit must face the center of the target
	{
		float xRelativePos = targetCenter.x - transform.position.x ;
		float yRelativePos = targetCenter.y - transform.position.y ;


		if (xRelativePos > 100) 
		{
			if(yRelativePos>100) direction = "NE";
			else if(yRelativePos<-100) direction = "SE";
			else direction = "E";
		}

		else if(xRelativePos < -100) //direction = "W";
		{
			if(yRelativePos>100) direction = "NW";
			else if(yRelativePos<-100) direction = "SW";
			else direction = "W";
		}

		else
		{
			if(yRelativePos>0) direction = "N";
			else direction = "S";
		}
	}
	
}

