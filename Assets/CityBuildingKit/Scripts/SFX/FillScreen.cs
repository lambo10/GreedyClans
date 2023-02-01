using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FillScreen : MonoBehaviour {

	private Image sprite;

	// Use this for initialization
	void Start ()
    {
		sprite = GetComponent<Image> ();
		SizeToFit ();
	}

	public void SizeToFit()
	{
		sprite.transform.localScale = new Vector3 (Screen.width, Screen.height, 1);//+Screen.width*0.1f +Screen.height*0.1f
	}

	/*
	void OnGUI() 
	{
		if (GUI.Button (new Rect (100, 200, 60, 30), "Resize"))
			SizeToFit ();
	}
	*/
}
