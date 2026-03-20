using UnityEngine;
using GameEnum;
public class Pancake: MonoBehaviour
{
    private static string MATERIAL_PATH = "Materials/";
    private static Material matRaw;
    private static Material matCooked;
    private static Material matBurnt;
    private GameObject MainObject;
    private GameObject Top;
    private GameObject Bot;
    public FoodState TopState { get; private set; }
    public FoodState BotState { get; private set; }
    private void Awake()
    {
        matRaw = Resources.Load<Material>(MATERIAL_PATH + "pancake_raw");
        matCooked = Resources.Load<Material>(MATERIAL_PATH + "pancake_cooked");
        matBurnt = Resources.Load<Material>(MATERIAL_PATH + "pancake_burnt");

        this.MainObject = gameObject;
        this.Top = gameObject.transform.Find("Top").gameObject;
        this.Bot = gameObject.transform.Find("Bot").gameObject;
        this.Reset();
    }
    public void Reset()
    {
        this.SetActive(false);
        this.SetFoodState(FoodState.Raw, false);
        this.SetFoodState(FoodState.Raw, true);
    }
    public void SetActive(bool state)
    {
        this.MainObject.SetActive(state);
    }
    public void SetFoodState(FoodState state, bool isFlipped)
    {
        Material targetMat;

        switch (state)
        {
            case FoodState.Raw:
                targetMat = matRaw;
                break;
            case FoodState.Cooked:
                targetMat = matCooked;
                break;
            case FoodState.Burnt:
                targetMat = matBurnt;
                break;
            default:
                targetMat = matRaw;
                break;
        }

        if (!isFlipped)
        {
            this.TopState = state;
            this.Top.GetComponent<Renderer>().material = targetMat;
        }
        else
        {
            this.BotState = state;
            this.Bot.GetComponent<Renderer>().material = targetMat;
        }
    }
    public bool IsCookedProperly()
    {
        return this.TopState == FoodState.Cooked && this.BotState == FoodState.Cooked;
    }
    public void ReturnToPool()
    {
        ObjectPool pool = GlobalObjectPools.GetPoolByFoodType(FoodType.Pancake);
        pool.ReturnObject(this.gameObject);
    }
}