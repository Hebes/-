using UnityEngine;

/// <summary>
/// 线条绘制
/// 需要继承
/// </summary>
[ExecuteInEditMode]
public class DrawGizmos : MonoBehaviour
{
    public virtual void OnDrawGizmos()
    {
    }

    public static void DrawCube(Vector3 pos, Color col, Vector3 scale)
    {
        Vector3 vector = scale * 0.5f;
        Vector3[] array = new Vector3[]
        {
            pos + new Vector3(vector.x, vector.y, vector.z),
            pos + new Vector3(-vector.x, vector.y, vector.z),
            pos + new Vector3(-vector.x, -vector.y, vector.z),
            pos + new Vector3(vector.x, -vector.y, vector.z),
            pos + new Vector3(vector.x, vector.y, -vector.z),
            pos + new Vector3(-vector.x, vector.y, -vector.z),
            pos + new Vector3(-vector.x, -vector.y, -vector.z),
            pos + new Vector3(vector.x, -vector.y, -vector.z)
        };
        UnityEngine.Debug.DrawLine(array[0], array[1], col);
        UnityEngine.Debug.DrawLine(array[1], array[2], col);
        UnityEngine.Debug.DrawLine(array[2], array[3], col);
        UnityEngine.Debug.DrawLine(array[3], array[0], col);
        UnityEngine.Debug.DrawLine(array[4], array[5], col);
        UnityEngine.Debug.DrawLine(array[5], array[6], col);
        UnityEngine.Debug.DrawLine(array[6], array[7], col);
        UnityEngine.Debug.DrawLine(array[7], array[4], col);
        UnityEngine.Debug.DrawLine(array[0], array[4], col);
        UnityEngine.Debug.DrawLine(array[1], array[5], col);
        UnityEngine.Debug.DrawLine(array[2], array[6], col);
        UnityEngine.Debug.DrawLine(array[3], array[7], col);
    }

    public static void DrawRect(Rect rect, Color col, float z = 0f)
    {
        Vector3 pos = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, 0f);
        Vector3 scale = new Vector3(rect.width, rect.height, z);
        DrawRect(pos, col, scale);
    }

    public static void DrawRect(Vector3 pos, Color col, Vector3 scale)
    {
        Vector3 vector = scale * 0.5f;
        Vector3[] array = new Vector3[]
        {
            pos + new Vector3(vector.x, vector.y, vector.z * 2f),
            pos + new Vector3(-vector.x, vector.y, vector.z * 2f),
            pos + new Vector3(-vector.x, -vector.y, vector.z * 2f),
            pos + new Vector3(vector.x, -vector.y, vector.z * 2f)
        };
        UnityEngine.Debug.DrawLine(array[0], array[1], col);
        UnityEngine.Debug.DrawLine(array[1], array[2], col);
        UnityEngine.Debug.DrawLine(array[2], array[3], col);
        UnityEngine.Debug.DrawLine(array[3], array[0], col);
    }

    public static void DrawPoint(Vector3 pos, Color col, float scale)
    {
        Vector3[] array = new Vector3[]
        {
            pos + Vector3.up * scale,
            pos - Vector3.up * scale,
            pos + Vector3.right * scale,
            pos - Vector3.right * scale,
            pos + Vector3.forward * scale,
            pos - Vector3.forward * scale
        };
        UnityEngine.Debug.DrawLine(array[0], array[1], col);
        UnityEngine.Debug.DrawLine(array[2], array[3], col);
        UnityEngine.Debug.DrawLine(array[4], array[5], col);
        UnityEngine.Debug.DrawLine(array[0], array[2], col);
        UnityEngine.Debug.DrawLine(array[0], array[3], col);
        UnityEngine.Debug.DrawLine(array[0], array[4], col);
        UnityEngine.Debug.DrawLine(array[0], array[5], col);
        UnityEngine.Debug.DrawLine(array[1], array[2], col);
        UnityEngine.Debug.DrawLine(array[1], array[3], col);
        UnityEngine.Debug.DrawLine(array[1], array[4], col);
        UnityEngine.Debug.DrawLine(array[1], array[5], col);
        UnityEngine.Debug.DrawLine(array[4], array[2], col);
        UnityEngine.Debug.DrawLine(array[4], array[3], col);
        UnityEngine.Debug.DrawLine(array[5], array[2], col);
        UnityEngine.Debug.DrawLine(array[5], array[3], col);
    }

    protected static void DrawEdgeCollider2D(EdgeCollider2D edgeCollider, Color col)
    {
        Vector2[] points = edgeCollider.points;
        for (int i = 0; i < points.Length - 1; i++)
            Debug.DrawLine(edgeCollider.transform.TransformPoint(points[i]), edgeCollider.transform.TransformPoint(points[i + 1]), col);
        // 绘制最后一条边
        //Debug.DrawLine(edgeCollider.transform.TransformPoint(points[^1]), edgeCollider.transform.TransformPoint(points[0]), Color.black);
    }

    protected static void DrawPoint(Vector3 start, Vector3 end, Color color)
    {
        UnityEngine.Debug.DrawLine(start,end,color);
    }

    // [HideInInspector] public static Rect MapRange;
    // [HideInInspector] public static Rect CameraRange;
    // [HideInInspector] public static Rect PlayerRange;
    // [HideInInspector] public static Rect EnemyRange;
    // [Header("左下角")] [SerializeField] private Transform bottomLeft;
    // [Header("右上角")] [SerializeField] private Transform TopRight;
    // private Color _mapRangeColor = Color.red;
    // [ContextMenu("Init")]
    // private void Init()
    // {
    // 	MapRange.min = bottomLeft.position;
    // 	MapRange.max = TopRight.position;
    // 	CameraRange = MapRange;
    // 	PlayerRange.min = new Vector2(CameraRange.min.x - 2f, CameraRange.min.y);
    // 	PlayerRange.max = new Vector2(CameraRange.max.x + 2f, CameraRange.max.y);
    // 	EnemyRange = CameraRange;
    // }
}