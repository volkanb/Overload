using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour {

    public GameController gController;                  // Game Controller reference
    Renderer rend;                              
    public int colorCode;
    public bool isTriggered = false;
    public float explosiveRadius = 0.6f;

    public bool isInTheEdge = false;
    public bool isOverloading = false;

    public bool isPopping = false;

    public bool isComboTile = false;
    public int comboCounter = 0;                        // Counter for unique combo tiles
    public Text textComponent = null;                   // Text component for unique combo tiles

    // Use this for initialization
    void Start()
    {
        gController = GameObject.Find("GameController").GetComponent<GameController>();
        rend = GetComponent<Renderer>();

        Invoke("ActivateEdgeBoolean", 2);
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

        if (GameController.currentChainRequiredCombo < comboCounter)                    // set the combo counter
            GameController.currentChainRequiredCombo = comboCounter;

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

    public void ActivateEdgeBoolean()
    {
        if (!isInTheEdge)
        {            
            isOverloading = true;
            gController.IncreaseOverload();
        }        
    }
}
