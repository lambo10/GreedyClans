using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deactivateAfterTime : MonoBehaviour
{
    public float deactivateTime = 100f; // 5 min in seconds
    private float timeLeft;
    private bool active = false;

    void OnEnable()
    {
        active = true;
        timeLeft = deactivateTime;
    }
    void Update()
    {
        if (active)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                gameObject.SetActive(false);
                active = false;
            }
        }
    }

    void OnDisable()
    {
        active = false;
    }
}
