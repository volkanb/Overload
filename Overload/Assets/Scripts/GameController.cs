using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {
    
    public GameObject cam;
    public StaticDataHolder datHolder;
    public GameObject center;

    public GameObject pausePanel;
    public bool isPaused = false;


    // Particle effects
    public GameObject popEffectObj;

    // Nudge variables
    int nudgeCounter = 0;
    bool isNudgeDisabled = false;
    float nudgeCoolDownTimer = 3.0f;
    float nudgeDisableTime = 0.0f;
    float firstNudgeTime = 0.0f;
    float secondNudgeTime = 0.0f;
    public GameObject cooldownPanel;
    public GameObject rewardNotificationObj;


    // ScoreIncrementNotification object
    public GameObject scoreIncrementObj;

    public Material[] colors;
    public Material[] colorsHighlighted;
    public bool highlightMode = false;
    public GameObject tappedTile = null;

    // Scoring and stats    
    public Text scoreText;                     // Score text container
    public int score = 0;
    public static int totalPops = 0;
    public static int totalUniqueComboTiles = 0;
    public static int completedChains = 0;
    public static int currentChainRequiredCombo = 0;
    public int nextChainTileQuantity = -1;
    public int nextChainColorCode = -1;
    public int recentChainTileQuantity = 0;
    public int recentChainColorCode = -1;

    // Overload checker
    public int noOfOverloadingTiles = 0;
    // Overload plane
    public MeshRenderer overloadPlaneRenderer;
    public float alphaLevel = 0.0f;
    public float alphaIncreaseVar = 0.001f;

    public Dictionary<int, GameObject> tiles = new Dictionary<int, GameObject>();                // collection of all the active tiles
    public Dictionary<int, GameObject> highlightedTiles = new Dictionary<int, GameObject>();     // collection of highlighted tiles

    // Use this for initialization
    void Start()
    {
        nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(4.0f, 8.0f));
        nextChainColorCode = UnityEngine.Random.Range(0, colors.Length);
        overloadPlaneRenderer = GameObject.Find("OverloadPlane").GetComponent<MeshRenderer>();
        overloadPlaneRenderer.material.color = new Color(1, 1, 1, alphaLevel);
        
        // Initialize static vars
        totalPops = 0;
        totalUniqueComboTiles = 0;
        completedChains = 0;
        currentChainRequiredCombo = 0;        
    }

    // Update is called once per frame
    void Update() {
        if (tappedTile && !highlightMode)           // First (after a pop sequence) tapped tile, start dynamic checking
        {
            tappedTile.SendMessage("ProcessAndTrigger");
            InvokeRepeating("DynamicCheck", 0.01F, 0.01F);
            highlightMode = true;
        }
        if (noOfOverloadingTiles > 0)
        {
            alphaLevel += alphaIncreaseVar;
            if (alphaLevel > 0.99f)
            {
                GameOver();
            }
        }
        if(alphaLevel >= 0.0f && noOfOverloadingTiles == 0)
        {
            alphaLevel -= alphaIncreaseVar;
            if (alphaLevel < 0.1f)
                alphaLevel = 0.0f;
        }
        
        overloadPlaneRenderer.material.color = new Color(1, 1, 1, alphaLevel);

        if (isNudgeDisabled)
        {
            if ((Time.time - nudgeDisableTime) > nudgeCoolDownTimer)
            {
                nudgeDisableTime = 0f;
                isNudgeDisabled = false;
                cooldownPanel.SetActive(false);
            }
        }
    }

    void GameOver()
    {
        datHolder = GameObject.Find("StaticDataObject").GetComponent<StaticDataHolder>();

        datHolder.LoadScoreScene(score);

        //Debug.Log("Game Over. Your score is : " + score);
        //Debug.Break();

    }

    void DynamicCheck()
    {
        // search finished, clear highlight and triggers 
        // check no of tiles in highlighted, 
        // if less than 3, break the coroutine
        // else,start again from the tapped tile

        //Debug.Log("DynamicCheck - " + highlightedTiles.Count.ToString() + " tiles in highlighted...");                

        if (highlightedTiles.Count < 3)
        {
            StartCoroutine(Shake(0.15f, 0.2f));
            CancelHighlight();
        }
            
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
            StartCoroutine(Shake(0.15f, 0.2f));
        }
        else
        {
            CancelInvoke();                                                         // clear highlight flags and cancel the dynamic check
            Vector3 notifPos = tappedTile.transform.position;                       // capture tappedtile pos before clear
            tappedTile = null;
            highlightMode = false;

            List<GameObject> tilesToPop = new List<GameObject>();

            // Combo variables
            int comboTotal = 0;
            int noOfComboTiles = 0;

            foreach (KeyValuePair<int, GameObject> tile in highlightedTiles)        // clear the flags on highlighted tiles and add them to pop list
            {
                tile.Value.SendMessage("UncheckTrigger");

                TileController tc = tile.Value.GetComponent<TileController>();
                if (tc.isOverloading)        // check for overloading tiles
                    DecreaseOverload();

                // Combo checker
                if (tc.comboCounter > 0)
                {
                    comboTotal += tc.comboCounter;
                    noOfComboTiles++;
                }
                    
                tc.isPopping = true;

                tilesToPop.Add(tile.Value);
            }
            highlightedTiles.Clear();                                               // remove the highlighted tiles from the list

            int poppedColorCode = tilesToPop[0].GetComponent<TileController>().colorCode;

            int scoreIncrement = ScoreProcessing(tilesToPop.Count, poppedColorCode, comboTotal, noOfComboTiles);         // handle scoring depending on player's combo and required chain

            foreach (GameObject go in tilesToPop.ToArray())                         // Loop the tiles to destroy
            {
                // Create particle effects
                Vector3 psp = go.transform.position;
                psp.z = -2;
                ParticleSystemRenderer psr = Instantiate(popEffectObj, psp, go.transform.rotation).GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>();
                psr.material = Resources.Load<Material>("Materials/" + ColorToString(poppedColorCode));

                // Remove popped tile from tiles dictionary
                RemoveTile(go.transform.GetInstanceID());

                // Destroy the tile
                Destroy(go);
            }

            // Create score notifications           
            notifPos.z = -3;
            ScoreIncrementNotificationController scoreNotifier = Instantiate(scoreIncrementObj, notifPos, Quaternion.identity).GetComponentInChildren<ScoreIncrementNotificationController>();
            scoreNotifier.TriggerScoreIncrementNotification(scoreIncrement);

            totalPops++;                                                            // increment of total pops counter
            highlightMode = false;                                                  // deactivate highlight mode
            currentChainRequiredCombo = 0;                                          // reset required combo counter
        }

        
    }

    public void AddNewTile(int id, GameObject tile)
    {
        tiles[id] = tile;                                  
    }

    public void RemoveTile(int key)
    {
        tiles.Remove(key);
    }

    public void AddTileToHighlighted(int id, GameObject tile)
    {
        highlightedTiles[id] = tile;
    }

    public int ScoreProcessing(int noOfTilesPopped, int cCode, int comboTotal, int noOfComboTiles)
    {
        // Debug.Log(noOfTilesPopped.ToString() + " tiles popped. " );

        int scoreToAdd = 0;

        // Increase the score depending on the no of tiles popped
        scoreToAdd += noOfTilesPopped * 10;        
                

        // Check required chain, add bonus score for fulfilling the required chain, specify next chain
        if (noOfTilesPopped >= nextChainTileQuantity && cCode == nextChainColorCode)
        {
            completedChains++;
            if (completedChains >= 20)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(18.0f, 20.0f));                
            }
            else if (completedChains >= 15)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(16.0f, 18.0f));                
            }
            else if (completedChains >= 10)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(12.0f, 16.0f));                
            }
            else if (completedChains >= 5)
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(8.0f, 12.0f));                
            }
            else
            {
                nextChainTileQuantity = Mathf.CeilToInt(UnityEngine.Random.Range(4.0f, 8.0f));                
            }

            nextChainColorCode = UnityEngine.Random.Range(0, colors.Length);           
        }

        // Add combo tile bonus
        if (comboTotal > 0)
        {
            scoreToAdd += comboTotal * 10;
            // TODO: Toggle combo bonus notification
        }

        if (noOfComboTiles > 1)
        {
            scoreToAdd *= noOfComboTiles;
            // TODO: Toggle combo multiplier bonus notification
        }       

        // Set recent chain variables
        recentChainColorCode = cCode;
        recentChainTileQuantity = noOfTilesPopped;

        // Add the score to total score
        score += scoreToAdd;

        // Adjust score display
        scoreText.text = score.ToString();

        return scoreToAdd;
    }

    // Legacy Score Processing
    /*
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

        scoreText.text = score.ToString();
    }
    */

    // Legacy GUI 
    /* 
    private void OnGUI()
    {
        // TEMPORARY
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(100, 10, 100, 80), "Score: " + score);            // score display
        GUI.Label(new Rect(700, 10, 200, 80), "Next Chain: " + nextChainTileQuantity.ToString() + " " + ColorToString(nextChainColorCode));      // upcoming required chain display
        GUI.Label(new Rect(1200, 10, 200, 80), "Recent Chain: " + recentChainTileQuantity.ToString() + " " + ColorToString(recentChainColorCode));      // recent completed chain display


        // Overload Display
        if (noOfOverloadingTiles > 0)
            GUI.Label(new Rect(100, 50, 100, 80), "OVERLOAD: ENABLED");
        else
            GUI.Label(new Rect(100, 50, 100, 80), "OVERLOAD: DISABLED");
    }
    */
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

    public void IncreaseOverload()
    {
        if (noOfOverloadingTiles == 0)
        {
            // Start border flashing
        }
        ++noOfOverloadingTiles;
        //Debug.Log("OVERLOADING TILES: INCREASED: " + noOfOverloadingTiles.ToString());
    }

    public void DecreaseOverload()
    {
        if (noOfOverloadingTiles == 1)
        {
            // Stop border flashing
        }
        --noOfOverloadingTiles;
        //Debug.Log("OVERLOADING TILES DECREASED: " + noOfOverloadingTiles.ToString());
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = cam.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            cam.transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        cam.transform.position = orignalPosition;
    }

    public void NudgeTiles()
    {
        
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    isIgnored = true;
        //}
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //{
        //    // Check if finger is over a UI element
        //    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //    {
        //        isIgnored = true;
        //    }
        //}
        if (!isNudgeDisabled)
        {
            // First nudge of the game
            if (nudgeCounter == 0)
            {
                nudgeCounter++;
                firstNudgeTime = Time.time;
                StartCoroutine(ShakeCenter(0.5f, 0.2f));
            }            
            else if (nudgeCounter == 1)
            {
                // If previous nudge happened more than 3 seconds ago, replace the first nudge with second nudge
                if ((Time.time - firstNudgeTime) >= 3f)
                    firstNudgeTime = Time.time;
                // Else, add the second nudge and warn the player
                else
                {
                    secondNudgeTime = Time.time;
                    nudgeCounter++;
                    Vector3 rewardPos = new Vector3(0.0f, -3.5f, 0.0f);
                    RewardNotificationsController rewardNotifier = Instantiate(rewardNotificationObj, rewardPos, Quaternion.identity).GetComponentInChildren<RewardNotificationsController>();
                    rewardNotifier.TriggerRewardNotification("Careful!");
                }

                StartCoroutine(ShakeCenter(0.5f, 0.2f));
            }
            else if (nudgeCounter == 2)
            {
                if ((Time.time - firstNudgeTime) < 3f)
                {
                    isNudgeDisabled = true;
                    nudgeDisableTime = Time.time;
                    nudgeCounter = 0;
                    firstNudgeTime = 0f;
                    secondNudgeTime = 0f;

                    cooldownPanel.SetActive(true);
                }
                else if ((Time.time - firstNudgeTime) >= 3f && (Time.time - secondNudgeTime) < 3f)
                {
                    firstNudgeTime = secondNudgeTime;
                    secondNudgeTime = 0f;
                    nudgeCounter = 1;
                }
                else
                {
                    firstNudgeTime = Time.time;
                    secondNudgeTime = 0f;
                    nudgeCounter = 1;
                }
                StartCoroutine(ShakeCenter(0.5f, 0.2f));
            }
        }
        
    }

    public IEnumerator ShakeCenter(float duration, float magnitude)
    {
        Vector3 orignalPosition = center.transform.position;
        float elapsed = 0f;

        float torque;
        if (UnityEngine.Random.value > 0.5f)
            torque = UnityEngine.Random.Range(-15f, -20f);
        else
            torque = UnityEngine.Random.Range(15f, 20f);

        while (elapsed < duration)
        {
            if(tiles.Count > 6)
                center.GetComponent<Rigidbody2D>().AddTorque(torque);

            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            center.transform.position = new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        center.transform.position = orignalPosition;
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            //enable the scripts again
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            //Disable scripts that still work while timescale is set to 0
        }
    }
}
