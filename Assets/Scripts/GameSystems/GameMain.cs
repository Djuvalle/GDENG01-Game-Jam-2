using UnityEngine;
using GameEnum;
using UnityEngine.SceneManagement;
[DefaultExecutionOrder(999999)] // We want this last, and get order systems ready
public class GameMain: MonoBehaviour
{
    private static int TOTAL_TO_SERVE = 10;
    private int amountServed = -1;
    private bool isWin = false;
    private float timeBeforeReturning = 5;
    private void Start()
    {
        // Set up pan
        // Set up ingredients
        // Set up player
        // Set up customers
        // Start countdown
        EventBroadcaster.Instance.AddObserver(ActionEvent.CustomerServed.ToString(), this.HandleCustomerServed);
        HandleCustomerServed();
        CustomerManager.SetResumeCustomerOrdering(true);
    }
    private void Update()
    {
        if (!isWin)
            return;

        if (timeBeforeReturning > 0)
            timeBeforeReturning -= Time.deltaTime;
        else
            SceneManager.LoadScene("StartScene");
    }

    private void HandleCustomerServed()
    {
        Debug.Log("Objective updated");
        amountServed++;
        Parameters parameterToSend = new Parameters();
        parameterToSend.PutExtra("Current", amountServed);
        parameterToSend.PutExtra("Goal", TOTAL_TO_SERVE);
        
        if (amountServed >= TOTAL_TO_SERVE)
        {
            parameterToSend.PutExtra("Win", true);
            this.isWin = true;
        }

        EventBroadcaster.Instance.PostEvent(ActionEvent.UpdateView.ToString(), parameterToSend);
    }
}