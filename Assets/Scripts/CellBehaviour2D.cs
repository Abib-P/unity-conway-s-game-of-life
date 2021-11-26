using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour2D : MonoBehaviour
{
    Vector2Int position;

    public void SetPosition(int x, int y)
    {
        position.x = x;
        position.y = y;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

}
