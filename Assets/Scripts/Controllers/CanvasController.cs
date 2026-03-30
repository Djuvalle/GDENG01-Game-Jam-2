using TMPro;
using GameEnum;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private RectTransform SpeedometerTicker;
    [SerializeField] private GameObject GameOverMenu;

    private void Start()
    {
        EventBroadcaster.Instance.AddObserver(Notifications.ScoreUpdated.ToString(), (param) =>
        {
            int score = param.GetIntExtra(ParameterKey.Score.ToString(), 0);
            ScoreText.text = $"Score: {score:000,000,000}";
        });
        EventBroadcaster.Instance.AddObserver(Notifications.PlayerPositionChanged.ToString(), (param) =>
        {
            const float MAX_SPEED = 30f;
            float speed = param.GetFloatExtra(ParameterKey.Speed.ToString(), 0);
            float zRotation = 90 - (speed / MAX_SPEED) * 180;
            SpeedometerTicker.DOLocalRotate(new Vector3(0, 0, zRotation), 0.1f).SetEase(Ease.OutSine);
        });
    }
    public void OnRetry()
    {
        DOTween.KillAll();
        EventBroadcaster.Instance.RemoveAllObservers();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnBack()
    {
        DOTween.KillAll();
        EventBroadcaster.Instance.RemoveAllObservers();
        SceneManager.LoadScene(SceneEnum.Start.ToString());
    }
    private void ShowGameOverMenu()
    {
        GameOverMenu.SetActive(true);
    }
}