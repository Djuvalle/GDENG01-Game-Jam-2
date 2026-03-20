using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayController : MonoBehaviour
{
    public void OnPlay()
    {
        Debug.Log("Pressed play");
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuit()
    {
        Debug.Log("Pressed quit");
        Application.Quit();
    }
}
