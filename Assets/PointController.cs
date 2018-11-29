using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour {

    public char type;
    private Renderer _matColor;
    private ControllerManager cm;
    private bool recent = false;

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
            recent = true;
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
        }
    }

    public char ReturnType()
    {
        return type;
    }

    public bool ReturnRecent()
    {
        if (recent == true)
        {
            recent = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
