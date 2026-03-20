using UnityEngine;
using GameEnum;

public class IngredientBox : MonoBehaviour, Clickable
{
    [SerializeField] private IngredientType ingredientType;
    private ObjectPool pool;
    private void Start()
    {
        Debug.Log($"ClickableObject {this.gameObject.name} is running");
        this.pool = GlobalObjectPools.GetPoolByIngredientType(ingredientType);
    }
    public void OnClicked()
    {
        Debug.Log($"ClickableObject {this.gameObject.name} was clicked");

        //Parameters parameters = new Parameters();
        //string key = ParameterKey.IngredientType.ToString();
        //parameters.PutExtra(key, this.ingredientType.ToString());
        
        //Debug.Log($"Posting event from IngredientBox with parameter {key}: {this.ingredientType}");
        //EventBroadcaster.Instance.PostEvent(ActionEvent.IngredientAdded.ToString(), parameters);
        GameObject ingredient = pool.GetObject();
        InteractionManager.GrabObject(ingredient);
    }

    public void OnClickRelease()
    {
        // Should not be called
        Debug.Log($"ClickableObject {this.gameObject.name} click released");
    }
}