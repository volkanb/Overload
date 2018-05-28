using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTouchInput : MonoBehaviour {

    public void OnTouchDown()
    {
        this.transform.parent.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);		
	}
}
