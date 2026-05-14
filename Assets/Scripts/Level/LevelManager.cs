using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        GameTimer.Instance?.StartTimer();

        int scene = SceneManager.GetActiveScene().buildIndex;
        if (scene == 1) AudioManager.Instance?.PlayLevel1Music();
        else if (scene == 2) AudioManager.Instance?.PlayLevel2Music();
        else if (scene == 3) AudioManager.Instance?.PlayLevel3Music();
    }
}