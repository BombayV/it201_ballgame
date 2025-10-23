using UnityEngine;

public class DisappearingWall : MonoBehaviour
{
    public void Vanish()
    {
        Debug.Log($"Wall {name} disappearing!");
        gameObject.SetActive(false);
    }

    public void ResetWall()
    {
        gameObject.SetActive(true);
    }
}