using UnityEngine;

public class Bar : MonoBehaviour
{
    public enum FillDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    public FillDirection DirectionToFill;
    public RectTransform TransformToFill;

    public void SetValue(float value)
    {
        switch (DirectionToFill)
        {
            case FillDirection.Right:
                TransformToFill.anchorMin = new Vector2(0, 0);
                TransformToFill.anchorMax = new Vector2(value, 1);
                break;
            case FillDirection.Left:
                TransformToFill.anchorMin = new Vector2(1 - value, 0);
                TransformToFill.anchorMax = new Vector2(1, 1);
                break;
            case FillDirection.Up:
                TransformToFill.anchorMin = new Vector2(0, 0);
                TransformToFill.anchorMax = new Vector2(1, value);
                break;
            case FillDirection.Down:
                TransformToFill.anchorMin = new Vector2(0, 1 - value);
                TransformToFill.anchorMax = new Vector2(1, 1);
                break;
            default:
                Debug.LogError("Unknown fill direction: " + DirectionToFill);
                break;
        }
    }
}
