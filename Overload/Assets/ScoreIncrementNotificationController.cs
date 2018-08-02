using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIncrementNotificationController : MonoBehaviour {

    public Animator animator;
    public Text textToDisplay;


    public void TriggerScoreIncrementNotification(int i)
    {
        IEnumerator coroutine;
        coroutine = PlayScoreIncrementNotification(i);
        StartCoroutine(coroutine);
    }

    IEnumerator PlayScoreIncrementNotification(int increment)
    {
        textToDisplay.text = ("+" + increment.ToString());
        animator.Play("ScalingIn");
        yield return new WaitForSeconds(0.5f);
        animator.Play("ScalingOut");
        yield return new WaitForSeconds(0.275f);
        Destroy(transform.parent.parent.gameObject);
    }
}
