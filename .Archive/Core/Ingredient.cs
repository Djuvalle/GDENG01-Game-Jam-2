using GameEnum;
using UnityEngine;
public class Ingredient : MonoBehaviour
{
    [SerializeField] private IngredientType ingredientType;
    public IngredientType IngredientType { get {return ingredientType; } set { ingredientType = value; }}
    public void ReturnIngredient()
    {
        ObjectPool pool = GlobalObjectPools.GetPoolByIngredientType(ingredientType);
        pool.ReturnObject(this.gameObject);
    }
}