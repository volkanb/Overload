using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverloadController : MonoBehaviour
{

    public GameController gController;

    // Use this for initialization
    void Start()
    {
        gController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Tile")
        {
            TileController tc = other.gameObject.GetComponent<TileController>();

            if (tc.isOverloading)
            {
                tc.isOverloading = false;
                gController.DecreaseOverload();
            }

            tc.isInTheEdge = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Tile")
        {
            TileController tc = other.gameObject.GetComponent<TileController>();
            if(!tc.isPopping)
            {
                tc.isInTheEdge = false;
                tc.isOverloading = true;
                gController.IncreaseOverload();
            }


        }
    }
}
