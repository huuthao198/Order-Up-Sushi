using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    [Header("Base setup (design reference)")]
    public float baseWidth = 1080f;
    public float baseHeight = 1920f;
    public float baseOrthoSize = 5f; // Ortho size khi bạn design ở 1080x1920

    void Start()
    {
        ScaleWithCamera();
    }

    void ScaleWithCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // World size hiện tại (theo ortho)
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        // World size ở base (1080x1920 với ortho base)
        float baseWorldHeight = baseOrthoSize * 2f;
        float baseWorldWidth = baseWorldHeight * (baseWidth / baseHeight);

        // Tính tỉ lệ scale giữa current world size và base world size
        float scaleX = worldWidth / baseWorldWidth;
        float scaleY = worldHeight / baseWorldHeight;

        // Luôn phủ kín -> chọn tỉ lệ lớn hơn
        float finalScale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(finalScale, finalScale, 1f);

        // Giữ background ở giữa camera
        Vector3 camCenter = cam.transform.position;
        camCenter.z = transform.position.z;
        transform.position = camCenter;
    }
}
