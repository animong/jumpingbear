using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect
{
    public Color m_color1 = Color.white;
    public Color m_color2 = Color.white;
    [Range(-180f, 180f)]
    public float m_angle = 0f;
    public bool m_ignoreRatio = true;

	private UIGradientUtils.Matrix2x3 localPositionMatrix;
	private Rect rect;
	private Vector2 dir;
	private UIVertex vertex;
	private Vector2 localPosition;
	private int i;

    public override void ModifyMesh(VertexHelper vh)
    {
        if(enabled)
        {
            rect = graphic.rectTransform.rect;
            dir = UIGradientUtils.RotationDir(m_angle);

            if (!m_ignoreRatio)
                dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

            localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

            vertex = default(UIVertex);
            for (i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex (ref vertex, i);
                localPosition = localPositionMatrix * vertex.position;
                vertex.color *= Color.Lerp(m_color2, m_color1, localPosition.y);
                vh.SetUIVertex (vertex, i);
            }
        }
    }
}
