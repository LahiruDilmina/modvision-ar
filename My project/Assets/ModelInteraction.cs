using UnityEngine;

public class ModelInteraction : MonoBehaviour
{
    [Header("Interaction Speeds")]
    public float moveSpeed = 0.002f;
    public float rotationSpeed = 0.05f;
    public float zoomSpeed = 0.001f;

    private Transform model;

    public void SetModel(Transform newModel)
    {
        model = newModel;
    }

    void Update()
    {
        if (model == null)
            return;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 move = (Camera.main.transform.right * touch.deltaPosition.x +
                                Camera.main.transform.up * touch.deltaPosition.y) * moveSpeed;

                model.position += move;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            float prevDist = (t1.position - t1.deltaPosition - (t2.position - t2.deltaPosition)).magnitude;
            float currDist = (t1.position - t2.position).magnitude;
            float diff = currDist - prevDist;

            Vector3 scale = model.localScale + Vector3.one * diff * zoomSpeed;
            scale = Vector3.Max(scale, Vector3.one * 0.05f);
            scale = Vector3.Min(scale, Vector3.one * 5f);

            model.localScale = scale;
        }
        else if (Input.touchCount == 3)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotX = touch.deltaPosition.x * rotationSpeed;
                float rotY = touch.deltaPosition.y * rotationSpeed;

                model.Rotate(Vector3.up, -rotX, Space.World);
                model.Rotate(Camera.main.transform.right, rotY, Space.World);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 move = (Camera.main.transform.right * Input.GetAxis("Mouse X") +
                            Camera.main.transform.up * Input.GetAxis("Mouse Y")) * moveSpeed * 50f;
            model.position += move;
        }
        else if (Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * 10f;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * 10f;
            model.Rotate(Vector3.up, -rotX, Space.World);
            model.Rotate(Camera.main.transform.right, rotY, Space.World);
        }
    }
}