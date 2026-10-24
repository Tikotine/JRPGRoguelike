[System.Serializable]
public abstract class Effect
{
    //Able to be editted in the inspector and hold data for things like the targeting model
    //Creates a gameaction to perform the effect
    public abstract GameAction GetGameAction();

}
