using UnityEngine;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        GameTimer.Instance?.StartTimer();
    }
}