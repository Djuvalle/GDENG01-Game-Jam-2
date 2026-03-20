using UnityEngine;
using GameEnum;
using System;

public class GlobalObjectPools : MonoBehaviour
{
    public static GlobalObjectPools Instance { get; private set; }
    private ObjectPool BatterPool { get; set; }
    private ObjectPool EggPool { get; set; }
    private ObjectPool FlourPool { get; set; }
    private ObjectPool ButterPool { get; set; }
    private ObjectPool PancakePool { get; set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        string PREFAB_PATH = "Prefabs/";
        string PREFAB_INGREDIENTS_PATH = PREFAB_PATH + "Ingredients/";
        string PREFAB_FOOD_PATH = PREFAB_PATH + "Food/";
        

        this.BatterPool = new ObjectPool(Resources.Load<GameObject>(PREFAB_INGREDIENTS_PATH + "Batter"));
        this.EggPool = new ObjectPool(Resources.Load<GameObject>(PREFAB_INGREDIENTS_PATH + "Egg"));
        this.FlourPool = new ObjectPool(Resources.Load<GameObject>(PREFAB_INGREDIENTS_PATH + "Flour"));
        this.ButterPool = new ObjectPool(Resources.Load<GameObject>(PREFAB_INGREDIENTS_PATH + "Butter"));

        this.PancakePool = new ObjectPool(Resources.Load<GameObject>(PREFAB_FOOD_PATH + "Pancake"));

        Instance = this;
    }

    public static ObjectPool GetPoolByIngredientType(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Batter:
                return Instance.BatterPool;
            case IngredientType.Egg:
                return Instance.EggPool;
            case IngredientType.Flour:
                return Instance.FlourPool;
            case IngredientType.Butter:
                return Instance.ButterPool;
            default:
                Debug.LogError($"No pool found for ingredient type: {type}");
                return null;
        }
    }

    public static ObjectPool GetPoolByFoodType(FoodType type)
    {
        switch (type)
        {
            case FoodType.Pancake:
                return Instance.PancakePool;
            default:
                Debug.LogError($"No pool found for ingredient type: {type}");
                return null;
        }
        
    }
}
