using UnityEngine;
using GameEnum;

public class ScoreManager : MonoBehaviour
{
    private const int NEW_ROAD_ENTERED_SCORE = 100;
    private const int COIN_COLLECTED_SCORE = 10;
    private int Score = 0;

    private void UpdateView()
    {
        Debug.Log("Score: " + Score);
        // TODO: Update the score view here
    }
    private void Start()
    {
        EventBroadcaster.Instance.AddObserver(Notifications.NewRoadEntered.ToString(), () =>
        {
            Score += NEW_ROAD_ENTERED_SCORE;
            UpdateView();
        });
        EventBroadcaster.Instance.AddObserver(Notifications.CoinCollected.ToString(), () =>
        {
            Score += COIN_COLLECTED_SCORE;
            UpdateView();
        });
    }
}