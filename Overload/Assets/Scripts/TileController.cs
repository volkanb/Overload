using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    public GameController gController;
    public int colorCode;
    public bool isTriggered = false;
    public float explosiveRadius = 0.6f;    

    // Use this for initialization
    void Start()
    {
        gController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void OnMouseDown()
    {
        isTriggered = true;
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosiveRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            //hitColliders[i].SendMessage("Damage", enemyDamage);
            if (hitColliders[i].gameObject.tag == "Tile" && !hitColliders[i].gameObject.GetComponent<TileController>().isTriggered && hitColliders[i].gameObject.GetComponent<TileController>().colorCode == colorCode)
            {
                hitColliders[i].SendMessage("DestroyAndTrigger");
            }
        }
        Destroy(this.gameObject);
    }

    public void DestroyAndTrigger()
    {
        isTriggered = true;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosiveRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {            
            if (hitColliders[i].gameObject.tag == "Tile" && !hitColliders[i].gameObject.GetComponent<TileController>().isTriggered && hitColliders[i].gameObject.GetComponent<TileController>().colorCode == colorCode)
            {
                hitColliders[i].SendMessage("DestroyAndTrigger");
            }
        }
        Destroy(this.gameObject);
    }

    public int GetColor()
    {
        gController = GameObject.Find("GameController").GetComponent<GameController>();
        colorCode = Random.Range(0, gController.colors.Length);
        
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)            
            rend.material = gController.colors[colorCode];
        
        return 0;
    }
}
