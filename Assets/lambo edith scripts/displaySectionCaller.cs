using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displaySectionCaller : MonoBehaviour
{
    GameObject SectionObj;
    public displaySection section;
    //public string sectionTag;
    //public int sectionTagIndex;
    
    //private void Awake()
    //{
    //    var objs = GameObject.FindGameObjectsWithTag(sectionTag);
    //    if (objs.Length > 0)
    //    {
    //        SectionObj = objs[sectionTagIndex];
    //        section = SectionObj.GetComponent<displaySection>();
    //        Debug.Log("here");
    //    }
    //}

    public void display()
    {
        if (section != null)
        {
            section.opensection();
        }
        else
        {
            Debug.Log("Section gameObject could not be located");
        }
    }


    public void close()
    {
        if (section != null)
        {
            section.closesection();
        }
        else
        {
            Debug.Log("Section gameObject could not be located");
        }
    }
}
