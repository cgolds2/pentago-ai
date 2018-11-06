using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour {

    public List<GameObject> blocks;
    public char[,] blockArray = new char[6, 6];

    public void ScanBlocks()
    {
        blockArray[0, 0] = blocks[0].transform.GetChild(1).GetChild(0).GetComponent<ColliderController>().collidedType;
        blockArray[0, 1] = blocks[0].transform.GetChild(1).GetChild(1).GetComponent<ColliderController>().collidedType;
        blockArray[0, 2] = blocks[0].transform.GetChild(1).GetChild(2).GetComponent<ColliderController>().collidedType;
        blockArray[0, 3] = blocks[1].transform.GetChild(1).GetChild(0).GetComponent<ColliderController>().collidedType;
        blockArray[0, 4] = blocks[1].transform.GetChild(1).GetChild(1).GetComponent<ColliderController>().collidedType;
        blockArray[0, 5] = blocks[1].transform.GetChild(1).GetChild(2).GetComponent<ColliderController>().collidedType;

        blockArray[1, 0] = blocks[0].transform.GetChild(1).GetChild(3).GetComponent<ColliderController>().collidedType;
        blockArray[1, 1] = blocks[0].transform.GetChild(1).GetChild(4).GetComponent<ColliderController>().collidedType;
        blockArray[1, 2] = blocks[0].transform.GetChild(1).GetChild(5).GetComponent<ColliderController>().collidedType;
        blockArray[1, 3] = blocks[1].transform.GetChild(1).GetChild(3).GetComponent<ColliderController>().collidedType;
        blockArray[1, 4] = blocks[1].transform.GetChild(1).GetChild(4).GetComponent<ColliderController>().collidedType;
        blockArray[1, 5] = blocks[1].transform.GetChild(1).GetChild(5).GetComponent<ColliderController>().collidedType;

        blockArray[2, 0] = blocks[0].transform.GetChild(1).GetChild(6).GetComponent<ColliderController>().collidedType;
        blockArray[2, 1] = blocks[0].transform.GetChild(1).GetChild(7).GetComponent<ColliderController>().collidedType;
        blockArray[2, 2] = blocks[0].transform.GetChild(1).GetChild(8).GetComponent<ColliderController>().collidedType;
        blockArray[2, 3] = blocks[1].transform.GetChild(1).GetChild(6).GetComponent<ColliderController>().collidedType;
        blockArray[2, 4] = blocks[1].transform.GetChild(1).GetChild(7).GetComponent<ColliderController>().collidedType;
        blockArray[2, 5] = blocks[1].transform.GetChild(1).GetChild(8).GetComponent<ColliderController>().collidedType;

        blockArray[3, 0] = blocks[2].transform.GetChild(1).GetChild(0).GetComponent<ColliderController>().collidedType;
        blockArray[3, 1] = blocks[2].transform.GetChild(1).GetChild(1).GetComponent<ColliderController>().collidedType;
        blockArray[3, 2] = blocks[2].transform.GetChild(1).GetChild(2).GetComponent<ColliderController>().collidedType;
        blockArray[3, 3] = blocks[3].transform.GetChild(1).GetChild(0).GetComponent<ColliderController>().collidedType;
        blockArray[3, 4] = blocks[3].transform.GetChild(1).GetChild(1).GetComponent<ColliderController>().collidedType;
        blockArray[3, 5] = blocks[3].transform.GetChild(1).GetChild(2).GetComponent<ColliderController>().collidedType;

        blockArray[4, 0] = blocks[2].transform.GetChild(1).GetChild(3).GetComponent<ColliderController>().collidedType;
        blockArray[4, 1] = blocks[2].transform.GetChild(1).GetChild(4).GetComponent<ColliderController>().collidedType;
        blockArray[4, 2] = blocks[2].transform.GetChild(1).GetChild(5).GetComponent<ColliderController>().collidedType;
        blockArray[4, 3] = blocks[3].transform.GetChild(1).GetChild(3).GetComponent<ColliderController>().collidedType;
        blockArray[4, 4] = blocks[3].transform.GetChild(1).GetChild(4).GetComponent<ColliderController>().collidedType;
        blockArray[4, 5] = blocks[3].transform.GetChild(1).GetChild(5).GetComponent<ColliderController>().collidedType;

        blockArray[5, 0] = blocks[2].transform.GetChild(1).GetChild(6).GetComponent<ColliderController>().collidedType;
        blockArray[5, 1] = blocks[2].transform.GetChild(1).GetChild(7).GetComponent<ColliderController>().collidedType;
        blockArray[5, 2] = blocks[2].transform.GetChild(1).GetChild(8).GetComponent<ColliderController>().collidedType;
        blockArray[5, 3] = blocks[3].transform.GetChild(1).GetChild(6).GetComponent<ColliderController>().collidedType;
        blockArray[5, 4] = blocks[3].transform.GetChild(1).GetChild(7).GetComponent<ColliderController>().collidedType;
        blockArray[5, 5] = blocks[3].transform.GetChild(1).GetChild(8).GetComponent<ColliderController>().collidedType;

        Debug.Log(blocks[3].transform.GetChild(1).GetChild(8).GetComponent<ColliderController>().collidedType);
        Debug.Log(blockArray[5, 5]);
    }
}
