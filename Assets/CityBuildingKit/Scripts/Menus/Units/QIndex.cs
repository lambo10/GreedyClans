using UnityEngine;
using System.Collections;

public class QIndex : MonoBehaviour {//attached to the small unit under construction buttons/icons
	
	private int 
		qindex = 50,				//position in list - 0(first), 1, 2
		objindex;					//type of unit - from 0(first) to 10(last)

	public int Objindex
	{
		get => objindex;
		set => objindex = value;
	}

	private int id;

	public int Id
	{
		get => id;
		set => id = value;
	}

	public int Count
	{
		get => count;
		set => count = value;
	}

	private int count;
	
	public int Qindex
	{
		get { return qindex; }
		set { qindex = value; }
	}

	public bool inque = false;		//is this unit under construction or not
	
}
