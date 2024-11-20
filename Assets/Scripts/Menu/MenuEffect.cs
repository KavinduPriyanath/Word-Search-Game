using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuEffect : MonoBehaviour
{
    public List<RectTransform> letters; 
    public float speed = 5f;
    private Vector2[] velocities; 
    private Vector2 screenBounds; 

    void Start()
    {

        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        screenBounds = canvasRect.rect.size / 2f;

        velocities = new Vector2[letters.Count];
        for (int i = 0; i < letters.Count; i++)
        {
            velocities[i] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
        }
    }

    void Update()
    {
        for (int i = 0; i < letters.Count; i++)
        {
            if (letters[i] == null) continue;

            letters[i].anchoredPosition += velocities[i] * Time.deltaTime;

            Vector2 pos = letters[i].anchoredPosition;

            if (pos.x < -screenBounds.x || pos.x > screenBounds.x)
            {
                velocities[i].x = -velocities[i].x; 
                pos.x = Mathf.Clamp(pos.x, -screenBounds.x, screenBounds.x);
            }

            if (pos.y < -screenBounds.y || pos.y > screenBounds.y)
            {
                velocities[i].y = -velocities[i].y; 
                pos.y = Mathf.Clamp(pos.y, -screenBounds.y, screenBounds.y);
            }

            letters[i].anchoredPosition = pos;
        }
    }
}
