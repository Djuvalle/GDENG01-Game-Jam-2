using System.Collections.Generic;
using UnityEngine;
using GameEnum;
using DG.Tweening;
[DefaultExecutionOrder(1000)]
public class PanController : MonoBehaviour, Clickable
{
    private static float MIN_COOK_TIME = 5f;
    private static float MAX_COOK_TIME = 20f;
    private static List<IngredientType> ACCEPTABLE_INGREDIENTS = new List<IngredientType>() { IngredientType.Batter };
    
    private List<IngredientType> currentIngredients = new List<IngredientType>();
    private GameObject pancake;
    private Pancake pancakeView;
    private ObjectPool pancakePool;
    private float cookTime1 = 0;
    private float cookTime2 = 0;
    private bool isFlipped = false;
    private bool debounceFlip = false;
    private ProximityPrompt proximityPromptView;
    private void Start()
    {
        //Debug.Log($"PanController {this.gameObject.name} is running");
        this.pancake = this.transform.Find("Pancake").gameObject;
        this.pancakeView = this.pancake.GetComponent<Pancake>();
        this.pancakePool = GlobalObjectPools.GetPoolByFoodType(FoodType.Pancake);
        this.proximityPromptView = this.transform.Find("ProximityPrompt").gameObject.GetComponent<ProximityPrompt>();
        this.proximityPromptView.SetText("[E] Flip Pancake");
        this.proximityPromptView.OnInteract += HandleInteract;
        this.ResetPan();
    }
    private void Update()
    {
        if (!currentIngredients.Contains(IngredientType.Batter)) {return;}
        if (!isFlipped)
        {
            cookTime1 += Time.deltaTime;
        }
        else
        {
            cookTime2 += Time.deltaTime;
        }

        if (!isFlipped && cookTime1 >= MAX_COOK_TIME || isFlipped && cookTime2 >= MAX_COOK_TIME)
        {
            //Debug.Log("Pancake is burnt!");
            this.pancakeView.SetFoodState(FoodState.Burnt, isFlipped);
        }
        else if (!isFlipped && cookTime1 >= MIN_COOK_TIME || isFlipped && cookTime2 >= MIN_COOK_TIME)
        {
            //Debug.Log("Pancake is cooked!");
            this.pancakeView.SetFoodState(FoodState.Cooked, isFlipped);
        }
    }
    private void ResetPan()
    {
        currentIngredients.Clear();
        this.pancakeView.Reset();
        this.pancake.transform.eulerAngles = new Vector3(0, 0, 0);
        this.proximityPromptView.SetEnabled(false);
        this.cookTime1 = 0;
        this.cookTime2 = 0;
        this.isFlipped = false;
    }
    private void HandleIngredientAdded(IngredientType ingredientType)
    {
        if (ingredientType == IngredientType.Batter)
        {
            this.pancakeView.SetActive(true);
        }
        else if (ingredientType == IngredientType.Butter)
        {
            // TODO: Add butter visual effect
        }
    }   
    private void OnCollisionEnter(Collision collision)
    {
        Ingredient ingredient = collision.gameObject.GetComponent<Ingredient>();
        // Do not accept if ingredient is null, not acceptable, or already in the pan
        if (
            ingredient == null ||
            !ACCEPTABLE_INGREDIENTS.Contains(ingredient.IngredientType) ||
            currentIngredients.Contains(ingredient.IngredientType)
        )
        {
            return;
        }
        currentIngredients.Add(ingredient.IngredientType);
        this.HandleIngredientAdded(ingredient.IngredientType);
    }
    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.gameObject.GetComponent<Ingredient>();
        // Do not accept if ingredient is null, not acceptable, or already in the pan
        if (
            ingredient == null ||
            !ACCEPTABLE_INGREDIENTS.Contains(ingredient.IngredientType) ||
            currentIngredients.Contains(ingredient.IngredientType)
        )
        {
            return;
        }
        ingredient.ReturnIngredient();
        currentIngredients.Add(ingredient.IngredientType);
        this.proximityPromptView.SetEnabled(true);
        this.HandleIngredientAdded(ingredient.IngredientType);
    }
    public void OnClicked()
    {
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
        InteractionManager.GrabObject(obj);
        this.ResetPan();
    }
    public void OnClickRelease()
    {
        
    }
    private void HandleInteract(Vector3 vector3)
    {
        if (currentIngredients.Contains(IngredientType.Batter) && !this.debounceFlip)
        {
            this.debounceFlip = true;
            isFlipped = !isFlipped;
            this.pancake.transform.DORotate(new Vector3(isFlipped ? 180 : 0, 0,  0), 0.2f).SetLoops(3, LoopType.Incremental);
            this.pancake.transform.DOMoveY(this.pancake.transform.position.y + 0.5f, 0.35f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => this.debounceFlip = false);
        }
    }

    
}