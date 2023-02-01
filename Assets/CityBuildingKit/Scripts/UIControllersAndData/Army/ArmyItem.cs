/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using UnityEngine;
using UnityEngine.UI;

namespace Army
{
	public class ArmyItem:MonoBehaviour
	{
        [SerializeField] private Text _count;

        public void Initialize(int count)
        {
            _count.text = count.ToString();
        }
    }
}
