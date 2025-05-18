using UnityEngine;

public class CreateWaypoints : MonoBehaviour
{
    [SerializeField] private Sprite[] textures;
    private int[] waypointPos = {
        Screen.width/2 -180,Screen.height/2+180,
        Screen.width/2 +180,Screen.height/2-180,

        Screen.width/2 +60,Screen.height/2,
        Screen.width/2 -180,Screen.height/2-180,
                    
        Screen.width/2 +180,Screen.height/2+180,
        Screen.width/2 -60,Screen.height/2,
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject newWaypoint = new()
            {
                name = "" + (char)(i + 97)
            };
            newWaypoint.AddComponent<SpriteRenderer>().sprite = textures[i];
            newWaypoint.transform.position = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    waypointPos[i*2],
                    waypointPos[i*2+1],
                    Camera.main.nearClipPlane
                )
            );

            newWaypoint.transform.localScale = new Vector3(.2f,.2f,.2f);
            newWaypoint.AddComponent<BoxCollider2D>();
            newWaypoint.AddComponent<Waypoint>();
            newWaypoint.transform.SetParent(gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
