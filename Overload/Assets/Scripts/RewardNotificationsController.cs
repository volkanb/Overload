using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardNotificationsController : MonoBehaviour
{

    public Animator animator;
    public Text textToDisplay;


    public void TriggerRewardNotification(string rewardText)
    {
        IEnumerator coroutine;
        coroutine = PlayRewardNotification(rewardText);
        StartCoroutine(coroutine);
    }

    IEnumerator PlayRewardNotification(string rewardText)
    {
        textToDisplay.text = (rewardText);
        animator.Play("ScalingIn");
        yield return new WaitForSeconds(0.5f);
        animator.Play("ScalingOut");
        yield return new WaitForSeconds(0.275f);
        Destroy(transform.parent.parent.gameObject);
    }
}