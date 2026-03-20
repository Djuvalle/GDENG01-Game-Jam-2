using UnityEngine;
using GameEnum;
using TMPro;
public class PlayerCanvasController : MonoBehaviour
{
    private TextMeshProUGUI tmpObjective;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.tmpObjective = this.transform.Find("ObjectiveText").gameObject.GetComponent<TextMeshProUGUI>();
        EventBroadcaster.Instance.AddObserver(ActionEvent.UpdateView.ToString(), this.HandleUpdateView);
    }
    private void HandleUpdateView(Parameters parameters)
    {
        Debug.Log("Update View");
        int cur = parameters.GetIntExtra("Current", -99);
        int max = parameters.GetIntExtra("Goal", -99);
        bool isWin = parameters.GetBoolExtra("Win", false);
        tmpObjective.text = $"Objective: {cur} / {max} Orders completed";

        if (isWin)
        {
            tmpObjective.text = $"Objective Complete!!";
        }
    }
}
