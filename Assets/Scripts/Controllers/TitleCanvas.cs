
using UnityEngine;
using UnityEngine.SceneManagement;
using GameEnum;

public class TitleCanvas : MonoBehaviour
{
    public void OnStart()
    {
        Debug.Log("Pressed Start Button");
        SceneManager.LoadScene(SceneEnum.GameScene.ToString());
    }

    public void OnQuit()
    {
        Debug.Log("Pressed Quit Button");
        Application.Quit();
    }
}