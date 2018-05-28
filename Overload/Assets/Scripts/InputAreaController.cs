using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAreaController : MonoBehaviour {

    public bool isLeft = false;
    public GameObject center;
    public float torque = 3.0f;




    bool isRotating = false;

	// Use this for initialization
	void Start () {
        if (isLeft)
            torque = (torque * -1);

        center = GameObject.Find("Center");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isRotating)
            center.GetComponent<Rigidbody2D>().AddTorque(torque);
	}

    public void OnMouseDown()    
    {
        isRotating = !isRotating;
        ToggleColor();
    }

    private void ToggleColor()
    {
        Material mat = GetComponent<Renderer>().material;
        if (mat.color != UnityEngine.Color.cyan)
            mat.color = UnityEngine.Color.cyan;
        else
            mat.color = UnityEngine.Color.white;
    }
}
