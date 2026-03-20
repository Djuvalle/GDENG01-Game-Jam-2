using UnityEngine;

public class Grabber : MonoBehaviour, Clickable
{
    public void OnClicked()
    {
        //Debug.Log($"Grabber: {this.gameObject.name} was clicked");
        InteractionManager.GrabObject(this.gameObject);
    }

    public void OnClickRelease()
    {
        //Debug.Log($"Grabber: {this.gameObject.name} click released");
        InteractionManager.ReleaseObject(this.gameObject);
    }
}