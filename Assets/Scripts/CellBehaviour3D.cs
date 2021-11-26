using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour3D : MonoBehaviour
{
    Vector3Int position;

    public void SetPosition(int x, int y, int z)
    {
        position.x = x;
        position.y = y;
        position.z = z;
    }

    public Vector3Int GetPosition()
    {
        return position;
    }
}
