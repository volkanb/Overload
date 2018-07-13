using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GameController : MonoBehaviour {
    public Material[] colors;
    public Material[] colorsHighlighted;
    public bool highlightMode = false;
    public GameObject tappedTile = null;

    // Scoring and stats
    public int score = 0;
    public static int totalPops = 0;
    public static int totalUniqueComboTiles = 0;
    public static int completedChains = 0;
    public static int currentChainRequiredCombo = 0;
    public int nextChainTileQuantity = -1;
    public int nextChainColorCode = -1;
    public int recentChainTileQuantity = 0;
    public int recentChainColorCode = -1;

    public Dictionary<int, GameObject> tiles = new Dictionary<int, GameObject>();                // collection of all the active tiles
    public Dictionary<int, GameObject> highlightedTiles = new Dictionary<int, GameObject>();     // collection of highlighted tiles

    // Use this for initialization
    void Start()
    {
        nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(4.0f, 8.0f));
        nextChainColorCode = UnityEngine.Random.Range(0, colors.Length);
    }

    // Update is called once per frame
    void Update() {
        if (tappedTile && !highlightMode)           // First (after a pop sequence) tapped tile, start dynamic checking
        {
            tappedTile.SendMessage("ProcessAndTrigger");
            InvokeRepeating("DynamicCheck", 0.01F, 0.01F);
            highlightMode = true;
        }
    }

    void DynamicCheck()
    {
        // search finished, clear highlight and triggers 
        // check no of tiles in highlighted, 
        // if less than 3, break the coroutine
        // else,start again from the tapped tile

        //Debug.Log("DynamicCheck - " + highlightedTiles.Count.ToString() + " tiles in highlighted...");                

        if (highlightedTiles.Count < 3)
            CancelHighlight();
        else
            RefreshHighlight();        
    }

    void RefreshHighlight()
    {
        foreach (KeyValuePair<int, GameObject> tile in highlightedTiles)        // clear the flags on highlighted tiles 
            tile.Value.SendMessage("UncheckTrigger");

        highlightedTiles.Clear();
        currentChainRequiredCombo = 0;

        tappedTile.SendMessage("ProcessAndTrigger");                            // restart the search and highlight process
    }

    void CancelHighlight()
    {
        CancelInvoke();                                                         // clear highlight flags and cancel the dynamic check
        tappedTile = null;
        highlightMode = false;
        currentChainRequiredCombo = 0;

        foreach (KeyValuePair<int, GameObject> tile in highlightedTiles)        // clear the flags on highlighted tiles 
            tile.Value.SendMessage("UncheckTrigger");

        highlightedTiles.Clear();
    }    

    public void InitiatePopSequence()
    {
        if (highlightedTiles.Count < currentChainRequiredCombo)                 // Check if the combo requirement is met
        {
            CancelHighlight();
        }
        else
        {
            CancelInvoke();                                                         // clear highlight flags and cancel the dynamic check
            tappedTile = null;
            highlightMode = false;

            List<GameObject> tilesToPop = new List<GameObject>();

            foreach (KeyValuePair<int, GameObject> tile in highlightedTiles)        // clear the flags on highlighted tiles and add them to pop list
            {
                tile.Value.SendMessage("UncheckTrigger");
                tilesToPop.Add(tile.Value);
            }
            highlightedTiles.Clear();                                               // remove the highlighted tiles from the list


            ScoreProcessing(tilesToPop.Count, tilesToPop[0].GetComponent<TileController>().colorCode, (currentChainRequiredCombo > 0));         // handle scoring depending on player's combo and required chain

            foreach (GameObject go in tilesToPop.ToArray())                         // Destroy the tiles
                Destroy(go);

            totalPops++;                                                            // increment of total pops counter
            highlightMode = false;                                                  // deactivate highlight mode
            currentChainRequiredCombo = 0;                                          // reset required combo counter
        }

        
    }

    public void AddNewTile(int id, GameObject tile)
    {
        tiles[id] = tile;                                  
    }

    public void AddTileToHighlighted(int id, GameObject tile)
    {
        highlightedTiles[id] = tile;
    }

    public void ScoreProcessing(int noOfTilesPopped, int cCode, bool comboMultiplier)
    {
        // Debug.Log(noOfTilesPopped.ToString() + " tiles popped. " );

        // Increase the score depending on the no of tiles popped
        score += (Mathf.CeilToInt(noOfTilesPopped / 3) + 10) * noOfTilesPopped;         

        // Check required chain, add bonus score for fulfilling the required chain, specify next chain
        if (noOfTilesPopped >= nextChainTileQuantity && cCode == nextChainColorCode)     
        {            
            completedChains++;
            if (completedChains >= 20)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(18.0f, 20.0f));
                score += 800;
            }
            else if (completedChains >= 15)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(16.0f, 18.0f));
                score += 400;
            }
            else if (completedChains >= 10)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(12.0f, 16.0f));
                score += 200;
            }
            else if (completedChains >= 5)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(8.0f, 12.0f));
                score += 100;
            }
            else
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(4.0f, 8.0f));
                score += 50;
            }

            nextChainColorCode = UnityEngine.Random.Range(0, colors.Length);
        }


        // Add bonus combo score
        if (comboMultiplier)
            score += 100;

        // Set recent chain variables
        recentChainColorCode = cCode;
        recentChainTileQuantity = noOfTilesPopped;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(100, 10, 100, 80), "Score: " + score);            // score display
        GUI.Label(new Rect(700, 10, 200, 80), "Next Chain: " + nextChainTileQuantity.ToString() + " " + ColorToString(nextChainColorCode) );      // upcoming required chain display
        GUI.Label(new Rect(1200, 10, 200, 80), "Recent Chain: " + recentChainTileQuantity.ToString() + " " + ColorToString(recentChainColorCode));      // recent completed chain display
    }

    public string ColorToString(int colorCode)
    {
        string res = "";
        switch (colorCode)
        {
            case 0:
                res = "Blue";
                break;
            case 1:
                res = "Green";
                break;
            case 2:
                res = "Red";
                break;
            default:
                res = "???";
                break;
        }

        return res;
    }


}
