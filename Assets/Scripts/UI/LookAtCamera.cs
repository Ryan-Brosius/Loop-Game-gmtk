using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            transform.rotation = Quaternion.LookRotation(cameraForward);
        }
    }
}
