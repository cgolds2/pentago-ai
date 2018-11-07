using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryController : MonoBehaviour {

    private void Start()
    {
        transform.Translate(0, 0, -1f);
    }

    public char DetectPoint()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f) && hit.transform.tag == "Point")
        {
            Debug.Log(hit.transform.GetComponent<PointController>().ReturnType());
            return hit.transform.GetComponent<PointController>().ReturnType();
        }
        else
        {
            return ' ';
        }
    }
}
