using System.Collections.Generic;
using UnityEngine;
using GameEnum;
using DG.Tweening;
using System;
[DefaultExecutionOrder(1000)]
public class MixerController : MonoBehaviour, Clickable
{
    //private static float MIN_COOK_TIME = 5f;
    //private static float MAX_COOK_TIME = 10f;
    private static List<IngredientType> REQUIRED_IGREDIENTS = new List<IngredientType>() { IngredientType.Flour, IngredientType.Egg, IngredientType.Egg };

    private List<IngredientType> currentIngredients = new List<IngredientType>();
    private bool isMixing = false;
    private bool isReady = false;
    private bool hasAllIngredients = false;
    private ObjectPool batterPool;
    private ProximityPrompt billboardView;
    private ProximityPrompt proximityPromptView;
    private void Start()
    {
        Debug.Log($"MixerController {this.gameObject.name} is running");
        this.batterPool = GlobalObjectPools.GetPoolByIngredientType(IngredientType.Batter);
        this.proximityPromptView = this.transform.Find("ProximityPrompt").gameObject.GetComponent<ProximityPrompt>();
        this.proximityPromptView.SetText("[E] Mix");
        this.proximityPromptView.OnInteract += HandleInteract;
        this.billboardView = this.transform.Find("BillboardUI").gameObject.GetComponent<ProximityPrompt>();
        this.Reset();
    }
    private void Update()
    {
        
    }
    private void Reset()
    {
        this.hasAllIngredients = false;
        this.isReady = false;
        this.isMixing = false;
        this.proximityPromptView.SetEnabled(false);
        currentIngredients.Clear();
        this.HandleIngredientAdded(IngredientType.Flour);
        currentIngredients.Clear();
    }
    private void HandleIngredientAdded(IngredientType ingredientType)
    {
        HashSet<IngredientType> uniqueSet = new HashSet<IngredientType>();
        foreach (IngredientType type in REQUIRED_IGREDIENTS)
        {
            uniqueSet.Add(type);
            Debug.Log($"Adding type: {type}");
        }

        string BASE_MSG = "Missing Ingredients:\n";
        string finalMsg = BASE_MSG;
        foreach (IngredientType type in uniqueSet)
        {
            int reqCount = CountIngredient(REQUIRED_IGREDIENTS, type);
            int curCount = CountIngredient(currentIngredients, type);
            int missing = reqCount - curCount;
            Debug.Log($"Type: {type} Req: {reqCount} Cur: {curCount}");
            if (missing > 0)
            {
                finalMsg += $"{type.ToString()} (x{missing})\n";
            }
        }
        // If nothing is missing
        if (BASE_MSG.Equals(finalMsg))
        {
            this.hasAllIngredients = true;
            this.proximityPromptView.SetEnabled(true);
            this.billboardView.SetEnabled(false);
        }
        else
        {
            this.hasAllIngredients = false;
            this.proximityPromptView.SetEnabled(false);
            this.billboardView.SetEnabled(true);
            this.billboardView.SetText(finalMsg);
        }
    }
    private static int CountIngredient(List<IngredientType> list, IngredientType targetType)
    {
        int total = 0;

        foreach (IngredientType curType in list)
        {
            Debug.Log($"Checking: Current Total {total}, Target Type {targetType}, Current Type {curType}");
            if (curType == targetType)
                total++;
        }

        return total;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Ingredient ingredient = collision.gameObject.GetComponent<Ingredient>();
        IngredientType ingredientType = ingredient.IngredientType;
        // Do not accept if ingredient is null or not acceptable
        if (
            ingredient == null ||
            !REQUIRED_IGREDIENTS.Contains(ingredientType)
        )
        {
            return;
        }

        int reqCount = CountIngredient(REQUIRED_IGREDIENTS, ingredientType);
        int curCount = CountIngredient(currentIngredients, ingredientType);
        // Do not accept if ingredient exceeds required
        if (curCount >= reqCount)
            return;

        ingredient.ReturnIngredient();
        currentIngredients.Add(ingredient.IngredientType);
        this.HandleIngredientAdded(ingredient.IngredientType);
    }
    public void OnClicked()
    {
        if (!isReady) return;
        GameObject obj = this.batterPool.GetObject();
        InteractionManager.GrabObject(obj);
        this.Reset();

        /*
        if (!this.currentIngredients.Contains(IngredientType.Batter))
        {
            return;    
        }
        GameObject obj = this.pancakePool.GetObject();
        Pancake pancakeComp = obj.GetComponent<Pancake>();
        pancakeComp.Reset();
        pancakeComp.SetFoodState(this.pancakeView.TopState, false);
        pancakeComp.SetFoodState(this.pancakeView.BotState, true);
        pancakeComp.SetActive(true);
        
        */
    }
    public void OnClickRelease()
    {
        
    }
    private void HandleInteract(Vector3 vector3)
    {
        if (!this.hasAllIngredients) return;
        this.isReady = true;
        this.proximityPromptView.SetEnabled(false);
        this.billboardView.SetEnabled(true);
        this.billboardView.SetText("[Click] Grab Batter");
    }

    
}