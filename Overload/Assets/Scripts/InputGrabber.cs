using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputGrabber : MonoBehaviour {

    public GameController gController;

    int frame = 0;
    bool activated = false;

    public void OnMouseDown()
    {
        if (!gController.isPaused)
        {
            activated = true;
            StartCoroutine("StartNudge");
        }
        
        //gController.NudgeTiles();

        //if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null)
        //{            
        //}
                    
    }

    private void Update()
    {
        if (activated && frame <= 10)
        {
            frame++;
        }
    }

    public IEnumerator StartNudge()
    {        
        yield return new WaitUntil(() => frame >= 10);

        if (!gController.isPaused)
            gController.NudgeTiles();

        activated = false;
        frame = 0;        
    }
}
