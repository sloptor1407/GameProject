using UnityEngine;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger tocado por: {other.gameObject.name}");
        if (!other.CompareTag("Player")) return;
        Debug.Log($"LevelResultsManager: {LevelResultsManager.Instance}");
        LevelResultsManager.Instance?.ShowLevelComplete();
    }

}