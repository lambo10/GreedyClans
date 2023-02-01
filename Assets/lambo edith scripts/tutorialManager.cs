using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialManager : MonoBehaviour
{
    public GameObject[] tutorialPointers;
    int previous_pointerIndex = 0;
    private bool checked_first_time_run = false;
    private int previousIndex = 0;
    public int noOfTimeSquenceStarted = 0;
    public bool[] accessed;

    void Start()
    {
        accessed = new bool[tutorialPointers.Length];
        StartCoroutine(WaitAndExecute(100f));
    }

    public void showTutorialPointer(int pointerIndex)
    {
        var isFirstRun = PlayerPrefs.GetInt("tutorial_firstTime_run", 1) == 1;

        if (isFirstRun && pointerIndex != previous_pointerIndex)
        {
            for (int i = 0; i < tutorialPointers.Length; i++)
            {
                GameObject _pointer = tutorialPointers[i];
                if (i == pointerIndex)
                {
                    _pointer.SetActive(true);
                    accessed[pointerIndex] = true;
                }
                else
                {
                    _pointer.SetActive(false);
                }
            }
            previous_pointerIndex = pointerIndex;
        }
        previousIndex = pointerIndex;
        noOfTimeSquenceStarted++;
    }

    public void clearAllPointers()
    {
        for (int i = 0; i < tutorialPointers.Length; i++)
        {
            GameObject _pointer = tutorialPointers[i];
            _pointer.SetActive(false);
        }
    }

    private void Update()
    {
        if (!checked_first_time_run)
        {
            var isFirstRun = PlayerPrefs.GetInt("tutorial_firstTime_run", 1) == 1;
            if (!isFirstRun)
            {
                clearAllPointers();
            }
            checked_first_time_run = true;
        }

        if (AllElementsAccessed())
        {
            PlayerPrefs.SetInt("tutorial_firstTime_run", 1);
        }
    }

    public bool AllElementsAccessed()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game"))
        {

            for (int i = 0; i < accessed.Length; i++)
            {
                if (i != 22 && i != 23 && i != 24 && i != 25 && i != 26 && i != 0)
                {
            
                    if (accessed[i] != true)
                    {
                        return false;
                    }
                }

            }
        }
        else
        {
            return false;
        }
        return true;
    }



    IEnumerator WaitAndExecute(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PlayerPrefs.SetInt("tutorial_firstTime_run", 0);
    }


}
