/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2019.
 * All rights reserved.
 */

using UnityEngine;

namespace Weapons.Projectiles
{
	public class ActivatorDestrorProjectile:MonoBehaviour 
	{
		void OnTriggerEnter(Collider target)
		{
			if (target.gameObject.CompareTag("Unit"))
			{
				Destroy(gameObject.transform.parent.gameObject);
			}
		}
	}
}
