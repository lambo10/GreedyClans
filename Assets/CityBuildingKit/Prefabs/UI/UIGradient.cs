using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient:BaseMeshEffect
{
    public enum Type
    {
        Vertical,
        Horizontal
    }

    [SerializeField]
    public Color32 EndColor = Color.black;

    [SerializeField]
    public Type GradientType = Type.Vertical;

    [SerializeField]
    [Range(-1.5f, 1.5f)]
    public float Offset;

    [SerializeField]
    public Color32 StartColor = Color.white;

    public override void ModifyMesh(VertexHelper vh)
    {
        if(!IsActive())
        {
            return;
        }

        List<UIVertex> list = new List<UIVertex>();
        vh.GetUIVertexStream(list);

        ModifyVertices(list);
        vh.Clear();
        vh.AddUIVertexTriangleStream(list);
    }

    public override void ModifyMesh(Mesh mesh)
    {
        if(!IsActive())
        {
            return;
        }

        List<UIVertex> list = new List<UIVertex>();
        using(var vertexHelper = new VertexHelper(mesh))
        {
            vertexHelper.GetUIVertexStream(list);
        }

        ModifyVertices(list);

        using(var vertexHelper2 = new VertexHelper())
        {
            vertexHelper2.AddUIVertexTriangleStream(list);
            vertexHelper2.FillMesh(mesh);
        }
    }

    public void ModifyVertices(List<UIVertex> _vertexList)
    {
        if(!IsActive())
        {
            return;
        }

        int nCount = _vertexList.Count;
        if(nCount <= 0)
        {
            Debug.Log("Empty vertexList");
            return;
        }

        switch(GradientType)
        {
            case Type.Vertical:
            {
                float fBottomY = _vertexList[0].position.y;
                float fTopY = _vertexList[0].position.y;
                for(int i = nCount - 1; i >= 1; --i)
                {
                    float fYPos = _vertexList[i].position.y;
                    if(fYPos > fTopY)
                    {
                        fTopY = fYPos;
                    }
                    else if(fYPos < fBottomY)
                    {
                        fBottomY = fYPos;
                    }
                }

                float fUiElementHeight = 1f / (fTopY - fBottomY);
                for(int i = nCount - 1; i >= 0; --i)
                {
                    UIVertex uiVertex = _vertexList[i];
                    uiVertex.color = Color32.Lerp(EndColor, StartColor, (uiVertex.position.y - fBottomY) * fUiElementHeight - Offset);
                    _vertexList[i] = uiVertex;
                }
            }

                break;
            case Type.Horizontal:
            {
                float fLeftX = _vertexList[0].position.x;
                float fRightX = _vertexList[0].position.x;

                for(int i = nCount - 1; i >= 1; --i)
                {
                    float fXPos = _vertexList[i].position.x;
                    if(fXPos > fRightX)
                    {
                        fRightX = fXPos;
                    }
                    else if(fXPos < fLeftX)
                    {
                        fLeftX = fXPos;
                    }
                }

                float fUiElementWidth = 1f / (fRightX - fLeftX);
                for(int i = nCount - 1; i >= 0; --i)
                {
                    UIVertex uiVertex = _vertexList[i];
                    uiVertex.color = Color32.Lerp(StartColor, EndColor, (uiVertex.position.x - fLeftX) * fUiElementWidth - Offset);
                    _vertexList[i] = uiVertex;
                }
            }

                break;
        }
    }
}