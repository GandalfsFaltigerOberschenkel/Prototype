using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public Texture2D cursorTextureInvalid;
    public Texture2D cursorTextureValid;

    public static CursorChanger Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

    public void SetCursor(int cursorType)
    {
        switch (cursorType)
        {
            case 0:
                Cursor.SetCursor(cursorTextureInvalid, Vector2.zero, CursorMode.Auto);
                break;
            case 1:
                Cursor.SetCursor(cursorTextureValid, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
}
