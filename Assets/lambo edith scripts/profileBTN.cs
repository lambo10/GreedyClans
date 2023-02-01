using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class profileBTN : MonoBehaviour
{
    public profileWindowManager _profileWindowManager;

    public void click()
    {
        _profileWindowManager.diisplayProfile();
    }
}
