using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour {

    public bool isRight;
    public GameObject rotation;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        rend.enabled = GameController.isRotating;
    }

    private void OnMouseDown()
    {
        if (GameController.isRotating)
        {
            if (isRight)
            {
                rotation.transform.Rotate(0, 0, -90);
                GameController.SwitchPlayer();
                
            }
            else
            {
                rotation.transform.Rotate(0, 0, 90);
                GameController.SwitchPlayer();
            }
        }
    }
}
