using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 scale = Vector2.one;
    Camera cam;

    private void OnEnable()
    {
        HandleCamera();
    }

    void HandleCamera()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.Log("didnt find a camera");
            return;
        }

        Matrix4x4 proj;

        if (cam.orthographic)
        {
            proj = Matrix4x4.Ortho(-cam.orthographicSize * scale.x,
                cam.orthographicSize * scale.x,
                -cam.orthographicSize * scale.y,
                cam.orthographicSize * scale.y,
                cam.nearClipPlane, cam.farClipPlane);
        }
        else
        {
            proj = Matrix4x4.Perspective(cam.fieldOfView, scale.x / scale.y, cam.nearClipPlane, cam.farClipPlane);
        }

        cam.projectionMatrix = proj;
    }

    private void Awake()
    {
        //HandleCamera();
    }

    private void Update()
    {
        HandleCamera();
    }

    private void OnDisable()
    {
        if (cam)
            cam.ResetProjectionMatrix();
    }

}
