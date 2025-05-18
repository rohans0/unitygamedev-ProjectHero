using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject waypoints;
    [SerializeField] private GameObject chaseCam;

	private Sprite defaultSprite;
    [SerializeField] private Sprite eggSprite;

    private int timesHitByEgg = 0;

    private Vector2 targetPos;
    private int curWaypoint = 0;

	public float lookingAngleStart = -1; // when -1, plane is looking
	private bool lookingClockwise = false;
	public bool chasing = false;

	private Color c;
    
    void Start()
    {
		c = chaseCam.GetComponent<Camera>().backgroundColor;
		defaultSprite = GetComponent<SpriteRenderer>().sprite;
        targetPos = transform.position;
        curWaypoint = Random.Range(0, 5);
    }
    
    void Update()
    {
		switch (timesHitByEgg)
		{
			case 0: // default state
				if (lookingAngleStart != -1 && Time.time > 1)
				{
					// look
					transform.eulerAngles +=
						new Vector3(
							0,0,
							100 * (lookingClockwise? -1 : 1) * Time.deltaTime 
						);
					float diff = (transform.eulerAngles.z - lookingAngleStart) % 360;
					if (diff < -180) diff += 360;
					else if (diff > 180) diff -= 360;

					if (diff > 90) lookingClockwise = true;

					if (diff >= 0) break;

					// switch state to chasing
					lookingClockwise = false;
					lookingAngleStart = -1;
					chasing = true;
				}
				else
				{
					// direction vector to waypoint
					Vector2 nDirVec = targetPos - (Vector2)transform.position;
					float dirVecDist = Mathf.Sqrt( nDirVec.x*nDirVec.x + nDirVec.y*nDirVec.y );

					if (chasing)
					{
						targetPos = arrowPrefab.transform.position;
						chaseCam.GetComponent<Camera>().backgroundColor = c;
						chaseCam.transform.position = new Vector3(
							transform.position.x + nDirVec.x / 2,
							transform.position.y + nDirVec.y / 2,
							Camera.main.nearClipPlane - 20
						);
						chaseCam.GetComponent<Camera>().orthographicSize = dirVecDist+.5f;
					}

					if ((dirVecDist > .1f && !chasing) || (chasing && dirVecDist < 4) )
					{
						// normalize the direction vector to waypoint
						nDirVec /= dirVecDist;
						
						float targetAngle = 
							(Mathf.Atan2(nDirVec.y, nDirVec.x) // angle
							- Mathf.PI /2 // since image is already upright
							) *Mathf.Rad2Deg;

						transform.eulerAngles = new Vector3(
							0,0,
							Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, 1.2f)
						);
					}
					else // change waypoint
					{
						chasing = false;
						chaseCam.GetComponent<Camera>().backgroundColor = Color.black;
						chaseCam.transform.position = Vector3.zero;
						ChangeWaypoint();
					}

					transform.localPosition += 
						new Vector3(
							-Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad),
							Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad),
							0
						) * Time.deltaTime;
				}
				break;

			case 1: // rotate only state
				transform.localRotation =
					Quaternion.Euler(
						0,0,
						transform.eulerAngles.z - 100 * Time.deltaTime
					);
				break;

			case 2: // egg state
				break;
		}
    }
    
    public void BulletHit()
    {
        // GetComponent<SpriteRenderer>().color *= new Color(
        //     1,1,1,
        //     .8f
        // );
        if (++timesHitByEgg >= 3)
		{
			lookingAngleStart = -1;
			Move();
		}
		GetComponent<SpriteRenderer>().sprite = (timesHitByEgg == 2? eggSprite : defaultSprite);
    }

	public void PlayerHit()
	{
		// islooking, ignore
		if (lookingAngleStart != -1) return;
		else if (chasing)
		{
			chaseCam.GetComponent<Camera>().backgroundColor = Color.black;
			chaseCam.transform.position = Vector3.zero;
			ChangeWaypoint();
			Move();
		}
		else
		{
			lookingAngleStart = transform.eulerAngles.z;
		}
	}

	private void ChangeWaypoint()
	{
		if (arrowPrefab.GetComponent<Arrow>().randomWaypoints)
			curWaypoint = Random.Range(0, 5);
		else
			if (curWaypoint == 5) curWaypoint = 0; else curWaypoint ++;
			
		
		targetPos = waypoints.transform.GetChild(curWaypoint).transform.position;
	}

    private void Move()
    {
        arrowPrefab.GetComponent<Arrow>().numEnemyDestroyed++;
        arrowPrefab.GetComponent<Arrow>().updateText();
        timesHitByEgg = 0;
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
                    Random.Range(.05f, .95f) * Screen.width,
                    Random.Range(.05f, .95f) * Screen.height,
                    Camera.main.nearClipPlane
                )
            );
		chasing = false;
    }
}
