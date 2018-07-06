using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    public GameController gController;
    Renderer rend;
    public int colorCode;
    public bool isTriggered = false;
    public float explosiveRadius = 0.6f;    

    // Use this for initialization
    void Start()
    {
        gController = GameObject.Find("GameController").GetComponent<GameController>();
        rend = GetComponent<Renderer>();
    }

    public void OnMouseDown()
    {
        if (isTriggered)
        {
            // Double tap - Initiate explosion, destroy the tiles, clear the dictionaries
            gController.SendMessage("InitiatePopSequence");
        }
        else
        {
            if (!gController)
                gController = GameObject.Find("GameController").GetComponent<GameController>();
            if (gController.tappedTile == null)
            {
                gController.tappedTile = this.gameObject;
            }
            else
            {
                gController.SendMessage("CancelHighlight");
                gController.tappedTile = this.gameObject;
            }            
        }            
    }

    public void ProcessAndTrigger()
    {
        isTriggered = true;
        if (!gController.highlightedTiles.ContainsKey(gameObject.GetInstanceID()))      // add selected tile to highlight collection (no duplicates)
            gController.AddTileToHighlighted(transform.GetInstanceID(), gameObject);        
        GetHighlightedColor();                                                          // get highlight color       

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosiveRadius);    // get tiles in explosive radius

        
        for (int i = 0; i < hitColliders.Length; i++)                // find matching tiles in explosive radius
        {
            if (hitColliders[i].gameObject.tag == "Tile" && !hitColliders[i].gameObject.GetComponent<TileController>().isTriggered && hitColliders[i].gameObject.GetComponent<TileController>().colorCode == colorCode)
            {
                hitColliders[i].SendMessage("ProcessAndTrigger");       // trigger the matching untriggered tiles
            }
        }
    }

    public void GetColor()
    {
        if (!gController)
            gController = GameObject.Find("GameController").GetComponent<GameController>();
        colorCode = Random.Range(0, gController.colors.Length);


        if (!rend)        
            rend = GetComponent<Renderer>();        
        rend.material = gController.colors[colorCode];
    }

    public void GetHighlightedColor()
    {
        if (!gController)
            gController = GameObject.Find("GameController").GetComponent<GameController>();

        if (!rend)
            rend = GetComponent<Renderer>();
        rend.material = gController.colorsHighlighted[colorCode];
    }

    public void UncheckTrigger()
    {
        if (isTriggered)
        {
            isTriggered = false;
            rend.material = gController.colors[colorCode];
        }        
    }
}
