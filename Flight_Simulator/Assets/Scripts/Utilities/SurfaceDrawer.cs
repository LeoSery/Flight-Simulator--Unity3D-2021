using UnityEditor;
using UnityEngine;

public class SurfaceDrawer : MonoBehaviour
{
    static Color m_wingColor = new Color(0.199f, 0.254f, 0.981f, 0.235f);
    static Color m_stallWingColor = new Color(0.988f, 0.0f, 0.0f, 0.235f);
    static Color m_flapColor = new Color(1.000f, 0.633f, 0.241f, 0.325f);

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    public static void SurfaceGizmos(AeroSurface surf, GizmoType gizmoType)
    {
        Rigidbody rb = surf.GetComponentInParent<Rigidbody>();
        if (surf.Config == null || rb == null) return;

        Gizmos.color = Color.clear;
        Gizmos.matrix = surf.transform.localToWorldMatrix;
        Gizmos.DrawCube(-Vector3.right * 0.25f * surf.Config.Chord, new Vector3(surf.Config.Chord, 0.1f, surf.Config.Chord * surf.Config.AspectRatio));

        DrawSurface(surf.transform, surf.Config, surf.FlapAngle, surf.IsAtStall);
    }

    private static void DrawSurface(Transform transform, AeroSurfaceConfig config, float flapAngle, bool isAtStall = false)
    {
        float mainChord = config.Chord * (1 - config.FlapFraction);
        float flapChord = config.Chord * config.FlapFraction;
        float span = config.Chord * config.AspectRatio;

        DrawRectangle(transform.position + transform.right * (0.25f * config.Chord - 0.5f * mainChord),
                transform.rotation,
                span,
                mainChord,
                isAtStall ? m_stallWingColor : m_wingColor);

        if (config.FlapFraction > 0)
            DrawRectangle(transform.position + transform.right * (0.25f * config.Chord - mainChord - 0.02f - 0.5f * flapChord * Mathf.Cos(flapAngle)) - transform.up * 0.5f * Mathf.Sin(flapAngle) * flapChord,
                    transform.rotation * Quaternion.AngleAxis(flapAngle * Mathf.Rad2Deg, Vector3.forward),
                    span,
                    flapChord,
                    m_flapColor);
    }

    private static void DrawRectangle(Vector3 position, Quaternion rotation, float width, float height, Color color)
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        Handles.DrawSolidRectangleWithOutline(
            GetRectangleVertices(position,
                rotation,
                width,
                height),
            color,
            Color.black);
    }

    private static Vector3[] GetRectangleVertices(Vector3 position, Quaternion rotation, float width, float height)
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(height, 0, -width) * 0.5f;
        vertices[1] = new Vector3(height, 0, width) * 0.5f;
        vertices[2] = new Vector3(-height, 0, width) * 0.5f;
        vertices[3] = new Vector3(-height, 0, -width) * 0.5f;

        for (int i = 0; i < 4; i++)
            vertices[i] = rotation * vertices[i] + position;

        return vertices;
    }
}
