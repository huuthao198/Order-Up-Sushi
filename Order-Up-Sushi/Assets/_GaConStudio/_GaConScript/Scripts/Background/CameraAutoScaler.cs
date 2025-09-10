using UnityEngine;

public class CameraAutoScaler : MonoBehaviour
{
    [Header("Target Resolution (base design)")]
    public float targetWidth = 1080f;   // chiều ngang gốc
    public float targetHeight = 1920f;  // chiều dọc gốc

    [Header("Base Orthographic Size")]
    public float baseOrthoSize = 5f;    // size gốc camera (dùng để design scene)

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    void AdjustCamera()
    {
        float targetAspect = targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;

        float scale = currentAspect / targetAspect;

        if (scale < 1f)
        {
            // Màn hình hẹp hơn (ví dụ điện thoại dài) → zoom out để không cắt
            cam.orthographicSize = baseOrthoSize / scale;
        }
        else
        {
            // Màn hình chuẩn hoặc rộng hơn → giữ nguyên
            cam.orthographicSize = baseOrthoSize;
        }
    }

    void OnValidate()
    {
        if (cam == null) cam = GetComponent<Camera>();
        AdjustCamera();
    }
}
