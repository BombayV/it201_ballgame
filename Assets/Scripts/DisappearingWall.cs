using UnityEngine;

public class DisappearingWall : MonoBehaviour
{
    // Called by the GameManager when the stage is complete
    public void Vanish()
    {
        Debug.Log($"Wall {name} disappearing!");
        gameObject.SetActive(false);
    }

    // Called by the GameManager to reset the level
    public void ResetWall()
    {
        gameObject.SetActive(true);
    }
}