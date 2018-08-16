using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIncrementNotificationController : MonoBehaviour {

    public Animator animator;
    public Text textToDisplay;
    public GameObject rewardNotificationObj;


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
        // Trigger reward notifications
        if (increment > 100)
        {
            string rewardText = "";
            if (increment > 450)
                rewardText = "GODLIKE!";
            else if (increment > 300)
                rewardText = "PERFECT!";
            else if (increment > 150)
                rewardText = "NICE!";
            else if (increment > 100)
                rewardText = "GOOD!";

            Vector3 rewardPos = new Vector3(0.0f, -3.5f, 0.0f);
            RewardNotificationsController rewardNotifier = Instantiate(rewardNotificationObj, rewardPos, Quaternion.identity).GetComponentInChildren<RewardNotificationsController>();
            rewardNotifier.TriggerRewardNotification(rewardText);
                       
        }
        


        animator.Play("ScalingOut");
        yield return new WaitForSeconds(0.275f);
        Destroy(transform.parent.parent.gameObject);
    }
}
