using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    // private GameObject[];
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private GameObject eggCooldown;
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private GameObject textbox;
    [SerializeField] private GameObject waypoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(
                planePrefab, 
                Camera.main.ScreenToWorldPoint(
                    new Vector3(
                        Random.Range(.05f, .95f) * Screen.width,
                        Random.Range(.05f, .95f) * Screen.height,
                        Camera.main.nearClipPlane
                    )
                ),
                Quaternion.identity
            );
        }
        updateText();
    }

    private const float rotationSpeed = 100.0f;
    private float movementSpeed = 0.8f;
    private float eggReloadTimer = 0;
    private bool usingMouse = true;

    public int numEggOnScreen = 0;
    public int numEnemyDestroyed = 0;
    public int numTouchedEnemy = 0;
    public bool randomWaypoints = false;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            randomWaypoints = !randomWaypoints;
            
        if (Input.GetKeyDown(KeyCode.H))
            waypoints.SetActive(!waypoints.activeSelf);
        
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            usingMouse = !usingMouse;
            updateText();
        }

        transform.Rotate(
            new Vector3(
                0,
                0,
                (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0) - (Input.GetKey(KeyCode.RightArrow) ? 1 : 0)
            ) * Time.deltaTime * rotationSpeed
        );
        
        if (usingMouse) {
            transform.position = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    Camera.main.nearClipPlane
                )
            );
        }
        else {
            
            movementSpeed += (
                (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0)
            ) * .03f;

            Vector3 newPos = new Vector3(
                -Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad),
                Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad),
                0
            ) * Time.deltaTime * movementSpeed;
            newPos.z = Camera.main.nearClipPlane;
            transform.position += newPos;
        }

        if (Input.GetKey(KeyCode.Space) && eggReloadTimer == 0)
        {
            Egg eggScript = Instantiate(eggPrefab, transform.position, transform.rotation).AddComponent<Egg>();
            eggScript.arrowPrefab = gameObject;

            numEggOnScreen++;
            updateText();

            eggReloadTimer = 0.2f;
            eggCooldown.SetActive(true);
        }
        else if (eggReloadTimer < 0)
        {
            eggReloadTimer = 0;
            eggCooldown.SetActive(false);
        }
        else
        {
            eggReloadTimer -= Time.deltaTime;
            eggCooldown.transform.localScale = new Vector2(20 * eggReloadTimer, .2f);
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Plane planeScript = other.GetComponent<Plane>();
        
        if (planeScript != null)
        {
            planeScript.Move();
            numTouchedEnemy++;
            updateText();
        }
    }
    
    public void updateText()
    {
        numEggOnScreen = numEggOnScreen < 1 ? 0 : numEggOnScreen;
        textbox.GetComponent<TextMeshProUGUI>().text =
            "WAYPOINTS:("+
            (randomWaypoints? "Random" : "Sequence")+","+
            (waypoints.activeSelf? "show":"hide")+

            ")HERO: Drive(" +
            (usingMouse ? "Mouse" : "Key") +

            ") TouchedEnemy(" +
            numTouchedEnemy +

            ")      EGG: OnScreen(" +
            numEggOnScreen +

            ")      ENEMY: Count(10) Destroyed(" +
            numEnemyDestroyed +

            ")";
    }
}
