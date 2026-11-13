using UnityEngine;
using UnityEngine.UI;

/*
 * Draws padding, spacing and child boundaries of whatever
 * layout group (horizontal, vertical, grid) you attach it to.
 * 
 * Make sure to enable Gizmos in the upper right corner of scene view.
 * 
 * Wrapped in #if UNITY_EDITOR just to be sure no code executes if the component
 * is left attached in a build.
 */


[RequireComponent(typeof(RectTransform))]
public class LayoutGroupVisualizer : MonoBehaviour
{
#if UNITY_EDITOR
    public Color containerColor = new Color(0f, 1f, 0f, 0.25f);//Bright Green
    public Color paddingColor = new Color(0f, 0.5f, 1f, 0.2f);//Blue
    public Color childBoundsColor = new Color(1f, 0.8f, 0f, 0.6f);//Yellow

    private void OnDrawGizmosSelected()
    {
        LayoutGroup layoutGroup = GetComponent<LayoutGroup>();
        if (layoutGroup == null) return;

        RectTransform rt = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        //Draw outer container bounds
        Gizmos.color = containerColor;
        DrawRectOutline(corners);

        //Draw inner padded area
        RectOffset pad = layoutGroup.padding;
        Vector3 lossyScale = rt.lossyScale;
        Vector3[] innerCorners = new Vector3[4];
        innerCorners[0] = corners[0] + new Vector3(pad.left * lossyScale.x, pad.bottom * lossyScale.y, 0);
        innerCorners[1] = corners[1] + new Vector3(pad.left * lossyScale.x, -pad.top * lossyScale.y, 0);
        innerCorners[2] = corners[2] + new Vector3(-pad.right * lossyScale.x, -pad.top * lossyScale.y, 0);
        innerCorners[3] = corners[3] + new Vector3(-pad.right * lossyScale.x, pad.bottom * lossyScale.y, 0);

        Gizmos.color = paddingColor;
        DrawRectOutline(innerCorners);

        //Draw individual active child bounds
        Gizmos.color = childBoundsColor;
        for (int i = 0; i < rt.childCount; i++)
        {
            RectTransform child = rt.GetChild(i) as RectTransform;
            if (child == null || !child.gameObject.activeInHierarchy) continue;

            Vector3[] childCorners = new Vector3[4];
            child.GetWorldCorners(childCorners);
            DrawRectOutline(childCorners);
        }
    }

    private void DrawRectOutline(Vector3[] c)
    {
        Gizmos.DrawLine(c[0], c[1]);
        Gizmos.DrawLine(c[1], c[2]);
        Gizmos.DrawLine(c[2], c[3]);
        Gizmos.DrawLine(c[3], c[0]);
    }
#endif
}
