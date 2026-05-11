using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] float parallaxFactor = 0.5f;

    Transform cam;
    float spriteWidth;
    float startPosX;

    void Start()
    {
        cam = Camera.main.transform;
        startPosX = transform.position.x;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        float camMovement = cam.position.x * parallaxFactor;
        transform.position = new Vector3(startPosX + camMovement,
                                         transform.position.y,
                                         transform.position.z);

        // Tiling horizontal
        float dist = cam.position.x - transform.position.x;
        if (Mathf.Abs(dist) >= spriteWidth)
        {
            float offset = Mathf.Round(dist / spriteWidth) * spriteWidth;
            transform.position = new Vector3(transform.position.x + offset,
                                             transform.position.y,
                                             transform.position.z);
        }
    }
}