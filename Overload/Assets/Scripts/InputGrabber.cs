using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGrabber : MonoBehaviour {

    public GameController gController;

    public void OnMouseDown()
    {
        gController.NudgeTiles();
    }
}
