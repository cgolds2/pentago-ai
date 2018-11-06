using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour {

    public char type;
    private Renderer _matColor;
    private ControllerManager cm;

    private void Start()
    {
        type = ' ';
        _matColor = GetComponent<Renderer>();
        cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerManager>();
    }

    void OnMouseDown()
    {
        if (!GameController.isPointFrozen && type == ' ')
        {
            if (GameController.isPlayer1)
            {
                type = 'X';
                _matColor.material.color = Color.green;
                GameController.isRotating = true;
                GameController.isPointFrozen = true;
            }
            else
            {
                type = 'O';
                _matColor.material.color = Color.red;
                GameController.isRotating = true;
                GameController.isPointFrozen = true;
            }
            cm.ScanBlocks();
        }
    }

    public char ReturnType()
    {
        return type;
    }
}
