using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] float sensivity;
    [SerializeField, Min(0.01f)] float smoothing;
    [SerializeField] float maxRot = 89.9f;

    float targetYRotation;
    float targetCameraXRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            targetYRotation += InputReader.Instance.LookDelta.x * sensivity;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * targetYRotation), Time.deltaTime / smoothing);

            targetCameraXRotation += InputReader.Instance.LookDelta.y * sensivity;
            targetCameraXRotation = Mathf.Clamp(targetCameraXRotation, -maxRot, maxRot);

            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(Vector3.left * targetCameraXRotation), Time.deltaTime / smoothing);
        }
    }
}
