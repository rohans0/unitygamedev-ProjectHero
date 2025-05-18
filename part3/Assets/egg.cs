using UnityEngine;

public class Egg : MonoBehaviour
{
    [SerializeField] public GameObject arrowPrefab;
    void Update()
    {
        Vector3 newPos = new Vector3(
            -Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad),
            Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad),
            Camera.main.nearClipPlane
        ) * Time.deltaTime * 5.0f;
        
        newPos = Camera.main.WorldToScreenPoint(newPos + transform.position);

        if (newPos.x < -2 || newPos.x > Screen.width + 2 ||
            newPos.y < -2 || newPos.y > Screen.height + 2)
        {
            arrowPrefab.GetComponent<Arrow>().numEggOnScreen--;
            arrowPrefab.GetComponent<Arrow>().updateText();
            Destroy(gameObject);
        }
        
        transform.position = Camera.main.ScreenToWorldPoint(newPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Plane planeScript = other.GetComponent<Plane>();
        
        if (planeScript != null)
        {
            planeScript.BulletHit();
            arrowPrefab.GetComponent<Arrow>().numEggOnScreen--;
            arrowPrefab.GetComponent<Arrow>().updateText();
            Destroy(gameObject);
            return;
        }

        Waypoint waypointScript = other.GetComponent<Waypoint>();

        if (waypointScript != null)
        {
            waypointScript.BulletHit();
            arrowPrefab.GetComponent<Arrow>().numEggOnScreen--;
            arrowPrefab.GetComponent<Arrow>().updateText();
            Destroy(gameObject);
            return;
        }
    }
}
