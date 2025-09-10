using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [Header("Background Settings")]
    public float scrollSpeed = 2f;          // tốc độ di chuyển (trái)
    public bool isHorizontal = true;        // true = cuộn ngang, false = cuộn dọc

    private Transform[] backgrounds;
    private float spriteLength;

    void Start()
    {
        // Lấy 2 sprite con
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            backgrounds[i] = transform.GetChild(i);

        if (backgrounds.Length < 2)
        {
            Debug.LogError("Cần ít nhất 2 sprite con đặt cạnh nhau để loop.");
            enabled = false;
            return;
        }

        // Lấy kích thước sprite theo world unit
        var sr = backgrounds[0].GetComponent<SpriteRenderer>();
        if (isHorizontal)
            spriteLength = sr.bounds.size.x;
        else
            spriteLength = sr.bounds.size.y;
    }

    void Update()
    {
        Vector3 movement = isHorizontal ? Vector3.left : Vector3.down;
        movement *= scrollSpeed * Time.deltaTime;

        // Move cả nhóm
        transform.position += movement;

        // Nếu background đầu đã ra khỏi màn thì dời nó ra sau cùng
        foreach (var bg in backgrounds)
        {
            if (isHorizontal)
            {
                if (bg.position.x < Camera.main.transform.position.x - spriteLength)
                {
                    float rightMostX = GetRightMostX();
                    bg.position = new Vector3(rightMostX + spriteLength, bg.position.y, bg.position.z);
                }
            }
            else
            {
                if (bg.position.y < Camera.main.transform.position.y - spriteLength)
                {
                    float topMostY = GetTopMostY();
                    bg.position = new Vector3(bg.position.x, topMostY + spriteLength, bg.position.z);
                }
            }
        }
    }

    private float GetRightMostX()
    {
        float maxX = float.MinValue;
        foreach (var bg in backgrounds)
            if (bg.position.x > maxX) maxX = bg.position.x;
        return maxX;
    }

    private float GetTopMostY()
    {
        float maxY = float.MinValue;
        foreach (var bg in backgrounds)
            if (bg.position.y > maxY) maxY = bg.position.y;
        return maxY;
    }
}
