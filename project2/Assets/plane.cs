using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject waypoints;

    private float speed = 1f;
    private int timesHit = 0;
    private Vector2 targetPos;
    private int curWaypoint = 0;
    
    void Start()
    {
        targetPos = transform.position;
        curWaypoint = Random.Range(0, 5);
    }
    
    void Update()
    {
        // normalized direction vector to waypoint
        Vector2 nDirVec = targetPos - (Vector2)transform.position;
        float dirVecDist = Mathf.Sqrt( nDirVec.x*nDirVec.x + nDirVec.y*nDirVec.y );
        if (dirVecDist > .1f)
        {
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
        else
        {
            if (arrowPrefab.GetComponent<Arrow>().randomWaypoints)
                curWaypoint = Random.Range(0, 5);
            else
                if (curWaypoint == 5) curWaypoint = 0; else curWaypoint ++;
                
            
            targetPos = waypoints.transform.GetChild(curWaypoint).transform.position;
        }

        gameObject.transform.localPosition += 
            new Vector3(
                -Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad),
                Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad),
                0
            ) * Time.deltaTime * speed;
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
        arrowPrefab.GetComponent<Arrow>().numEnemyDestroyed++;
        arrowPrefab.GetComponent<Arrow>().updateText();
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
                    Random.Range(.05f, .95f) * Screen.width,
                    Random.Range(.05f, .95f) * Screen.height,
                    Camera.main.nearClipPlane
                )
            );
    }
}
