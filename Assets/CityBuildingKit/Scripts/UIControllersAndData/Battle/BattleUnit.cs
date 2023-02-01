/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */
 
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit:MonoBehaviour 
{
	[SerializeField] private int _index;
    [SerializeField] private Text _count;
    [SerializeField] private Text _countOfSelected;

    public void Initialize(int count)
    {
        _count.text = count.ToString();
    }
    
    [UsedImplicitly]
	public void IncreaseUnit()
    {
        if (int.Parse(Helper.chainId) == 4000)
        {
            MenuArmyBattle.instance.Commit(_index);
        }
        else
        {
            MenuArmyBattle.instance.CommitServerRpc(_index);
        }
    }
    
    [UsedImplicitly]
	public void DecreaseUnit()
    {
        if (int.Parse(Helper.chainId) == 4000)
        {
            MenuArmyBattle.instance.Cancel(_index);
        }
        else
        {
            MenuArmyBattle.instance.CancelServerRpc(_index);
        }
    }

	[UsedImplicitly]
	public void ControlUnitCount(bool isOn)
	{
        if (int.Parse(Helper.chainId) == 4000)
        {
            MenuArmyBattle.instance.CommitAll(_index, isOn);
        }
        else
        {
            MenuArmyBattle.instance.CommitAllServerRpc(_index, isOn);
        }
	}
}
