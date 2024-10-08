using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggeredViewVolume : AViewVolume
{
    public GameObject target;

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject == target)
        {
            SetActive(true);
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject == target)
        {
            SetActive(false);
        }
    }

    public void OnGUI()
    {
        foreach (Object item in FindObjectsOfType(typeof(AViewVolume)))
        {
            GUILayout.Label(item.name);
        }
    }




}
