using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour {

    public bool isRight;
    public int blockNumber;
    public GameObject rotation;
    private ControllerManager cm;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerManager>();
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
                GameController.lastBlockNumber = blockNumber;
                GameController.lastRotation = "right";
                rotation.transform.Rotate(0, 0, -90);
                GameController.SwitchPlayer();
            }
            else
            {
                GameController.lastBlockNumber = blockNumber;
                GameController.lastRotation = "left";
                rotation.transform.Rotate(0, 0, 90);
                GameController.SwitchPlayer();
            }
            cm.ScanBlocks();
        }
    }
}
