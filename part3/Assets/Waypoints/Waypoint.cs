using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Vector2 startPos;
    private int timesHit = 0;
	private float shakeMag = 0;
	private Color c;
    public GameObject wayptCam;
    
    void Start()
    {
		c = wayptCam.GetComponent<Camera>().backgroundColor;
		wayptCam.GetComponent<Camera>().backgroundColor = Color.black;
        startPos = Camera.main.WorldToScreenPoint( gameObject.transform.position );
    }
    
    public void BulletHit()
    {
        GetComponent<SpriteRenderer>().color *= new Color(
            1,1,1,
            .8f
        );
		

        if (++timesHit >= 4) Move();
		else
		{
			shakeMag = timesHit;
			wayptCam.GetComponent<Camera>().backgroundColor = c;
			wayptCam.transform.position = Camera.main.ScreenToWorldPoint(startPos);
		}
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

	void Update()
	{
		if (shakeMag > 0)
		{
			transform.position =
            Camera.main.ScreenToWorldPoint(
				new Vector3(
					startPos.x + Random.Range(-shakeMag*2, shakeMag*2),
					startPos.y + Random.Range(-shakeMag*2, shakeMag*2),
					Camera.main.nearClipPlane
				)
			);

			shakeMag -= Time.deltaTime;
			if (shakeMag <= 0)
			{
				wayptCam.GetComponent<Camera>().backgroundColor = Color.black;
				wayptCam.transform.position = Vector3.zero;
			}
		}
	}
}
