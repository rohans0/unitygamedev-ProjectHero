using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Vector2 startPos;
    private int timesHit = 0;
    
    void Start()
    {
        startPos = Camera.main.WorldToScreenPoint( gameObject.transform.position );
    }
    
    public void BulletHit()
    {
        GetComponent<SpriteRenderer>().color *= new Color(
            1,1,1,
            .8f
        );
        if (++timesHit >= 4) Move();
    }
    
    public void Move()
    {
        timesHit = 0;
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(
            color.r,
            color.g,
            color.b,
            1
        );
        transform.position = 
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    Random.Range(-15, 15) + startPos.x,
                    Random.Range(-15, 15) + startPos.y,
                    Camera.main.nearClipPlane
                )
            );
    }
}
