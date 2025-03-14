using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public Vector2 boundMin;
    public Vector2 boundMax;
    public float speed = 5f;
    public virtual void Update()
    {
        //Lerp the position of the object to the mouse position
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Vector2.Lerp(transform.position, mousePos, speed * Time.deltaTime);
        //Clamp the position of the object to the bounds
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, boundMin.x, boundMax.x), Mathf.Clamp(transform.position.y, boundMin.y, boundMax.y));
    }
}
