using UnityEngine;

public class Inspectable : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    private bool isInspecting = false;

    public Transform inspectPoint;

    //BlurController blur;

    void Update()
    {
        if (!isInspecting) return;

        // Scroll to scale
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.localScale += Vector3.one * scroll * 2f;

        // Exit inspect
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopInspect();
        }
    }

    public void StartInspect(Transform point)
    {
        if (isInspecting) return;

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        inspectPoint = point;

        transform.SetParent(inspectPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        isInspecting = true;

        // Freeze player
        Time.timeScale = 0f;

        //blur = FindAnyObjectByType<BlurController>();
        //if (blur != null) blur.EnableBlur();
    }

    void StopInspect()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        isInspecting = false;

        Time.timeScale = 1f;

        //if (blur != null) blur.DisableBlur();
    }
}