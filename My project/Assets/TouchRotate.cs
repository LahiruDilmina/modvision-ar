using UnityEngine;

public class TouchRotate : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    void Update()
    {
        // Check for any touch on screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                // This rotates the Barcode object itself
                // Since the model is inside it, the model MUST spin
                float rotX = touch.deltaPosition.x * rotationSpeed;
                transform.Rotate(Vector3.up, -rotX, Space.World);
            }
        }
    }
}