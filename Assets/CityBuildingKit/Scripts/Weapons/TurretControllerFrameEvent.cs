using UnityEngine;

public class TurretControllerFrameEvent : TurretControllerBase{		//frame event based - turns to a certain frame then fires
	
	private Sextant sextant;

	void Start()
	{  		
		InitializeComponents ();
		sextant = GetComponent<Sextant> ();
		Aim ();
	}

	void Update()
	{		
		if(fire)
		{
			Aim(); 
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= fireRate)
			{
				UpdateTarget ();              
				elapsedTime = 0.0f;		
				if(targetList.Count>0)
				{
					sextant.targetObject = targetList[0];
					sextant.fire = true;
				}
				else
				{
					sextant.fire = false;
				}
			}
		}
	}

}
