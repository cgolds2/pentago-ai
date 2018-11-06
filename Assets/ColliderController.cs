using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour {

    public char collidedType;

    private void Start()
    {
        collidedType = ' ';
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("yeeetyeeeeeeeeeeeeeet");
        if (collision.transform.tag == "Point")
        {
            Debug.Log("yeeet");
            collidedType = collision.transform.GetComponent<PointController>().ReturnType();
        }
    }
}
