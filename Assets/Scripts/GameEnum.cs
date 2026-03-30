namespace GameEnum
{
    public enum Notifications
    {
        PlayerPositionChanged,
        PlayerDied,
        WaypointAdded,
        NewRoadEntered,
        CoinCollected,
        ScoreUpdated,
    }
    public enum ParameterKey
    {
        X,
        Y,
        Z,
        Score,
        Speed
    }
    public enum RoadDirection
    {
        General,
        Straight,
        Right,
        Left
    }
    public enum SceneEnum
    {
        Start,
        GameScene
    }
}