using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController {

    public static bool isPlayer1 = true;
    public static bool isPointFrozen = false;
    public static bool isRotating = false;
    public static int xVal = -1;
    public static int yVal = -1;
    public static int lastBlockNumber;
    public static string lastRotation = "";

    public static void SwitchPlayer()
    {
        if (isPlayer1) // If first player then switch to second player.
            isPlayer1 = false;
        else
            isPlayer1 = true;
        isPointFrozen = false;
        isRotating = false;
    }
}
