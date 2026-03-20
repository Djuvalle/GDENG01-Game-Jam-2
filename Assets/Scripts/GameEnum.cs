namespace GameEnum
{
    public enum ActionEvent
    {
        IngredientAdded,
        Interacted,
        CustomerServed,
        UpdateView
    }
    public enum ParameterKey
    {
        IngredientType,
    }
    public enum IngredientType
    {
        Flour,
        Egg,
        Batter,
        Butter
    }
    public enum FoodType
    {
        Pancake,
        ButteredPancake
    }
    public enum FoodState
    {
        Raw,
        Cooked,
        Burnt
    }
    public enum CustomerState
    {
        Cooldown,
        Idle,
        Order,
        Leave,
    }
    public enum OrderState
    {
        Success,
        Failure
    }
}