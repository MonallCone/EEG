using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera cam;
    public Transform inspectPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                Inspectable item = hit.collider.GetComponent<Inspectable>();

                if (item != null)
                {
                    item.StartInspect(inspectPoint);
                }
            }
        }
    }
}