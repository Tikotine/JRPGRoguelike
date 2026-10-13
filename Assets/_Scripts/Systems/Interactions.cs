using UnityEngine;

public class Interactions : Singleton<Interactions>
{
    //This script is the centralised place to enable and disable interactions

    public bool PlayerIsDragging { get; set; } = false;

    //Returns true when action system is not perofrming
    public bool PlayerCanInteract()
    {
        if (!ActionSystem.Instance.IsPerforming)
        {
            return true;
        }

        else 
        {
            return false;
        }
    }

    //Will return true if the player is not dragging
    public bool PlayerCanHover()
    {
        if (PlayerIsDragging)
        {
            return false;
        }

        else
        {
            return true;
        }    
    }
}
