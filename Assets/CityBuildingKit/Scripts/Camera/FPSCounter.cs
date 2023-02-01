// Attach this to a GUIText to make a frames/second indicator.
//
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10).
// We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with
// correct overall FPS even if the interval renders something like
// 5.5 frames.

using UnityEngine;

class FPSCounter:MonoBehaviour
{
	float updateInterval = 1.0f;
	private float accum; 		// FPS accumulated over the interval
	private int frames; 		// Frames drawn over the interval
	private float timeleft;	// Left time for current interval
	private float fps = 15.0f; 		// Current FPS
	private float lastSample;
	private float gotIntervals;

	private int hours; 
	private int minutes;
	private int seconds;

	private float realTimeFloat;
	private string realTimeText;

	void Start()
	{
	    timeleft = updateInterval;
	    lastSample = Time.realtimeSinceStartup;
	    InvokeRepeating("UpdateTimeCounter", 1.0f , 1.0f);
	}

	private float GetFPS()
	{
		return fps;
	}

	private bool HasFPS()
	{
		return gotIntervals > 2;
	}
	 
	void Update()
	{
	    ++frames;
	    var newSample = Time.realtimeSinceStartup;
	    var deltaTime = newSample - lastSample;
	    lastSample = newSample;

	    timeleft -= deltaTime;
	    accum += 1.0f / deltaTime;
	    
	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0 )
	    {
	        // display two fractional digits (f2 format)
	        fps = accum/frames;
			// guiText.text = fps.ToString("f2");
	        timeleft = updateInterval;
	        accum = 0.0f;
	        frames = 0;
	        ++gotIntervals;
	    }
	}

	void OnGUI()
	{
		// GUI.Box(new Rect(Screen.width/2 - 75, Screen.height -20, 70, 25), fps.ToString("f2") + " fps");
		// | QSetting: " + QualitySettings.currentLevel

		// GUI.Box(new Rect(Screen.width/2 + 5, Screen.height -20, 70, 25), realTimeText);
	}



	private void UpdateTimeCounter()				//calculate remaining time
	{
		realTimeFloat = Time.realtimeSinceStartup;

		hours = (int) (realTimeFloat / 3600) ; 
		minutes = (int) (realTimeFloat / 60);
		seconds= (int) (realTimeFloat % 60);

		if (minutes == 60) minutes = 0;
		if (seconds == 60) seconds = 0;

		UpdateTimeLabel ();
	}

	private void UpdateTimeLabel()									//update the time labels on top
	{
		if(hours>0 && minutes >0 && seconds>=0 )
		{			

			realTimeText = 
				hours +" h " +
					minutes +" m " +
					seconds +" s";			
		}
		else if(minutes > 0 && seconds >= 0)
		{
			realTimeText = 
				minutes +" m " +
					seconds +" s";
			
		}
		else if(seconds > 0 )
		{
			realTimeText  = 
				seconds +" s";
		}

	}
}
