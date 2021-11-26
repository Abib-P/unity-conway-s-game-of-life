using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    GameObject aliveCell;
    GameObject deadCell;
    bool isAlive;


    public Cell(GameObject aliveCell, GameObject deadCell)
    {
        this.aliveCell = aliveCell;
        this.deadCell = deadCell;
        this.isAlive = false;
        aliveCell.SetActive(false);
    }

    public void Kill()
    {
        isAlive = false;
        aliveCell.SetActive(false);
        deadCell.SetActive(true);
    }

    public void Revive()
    {
        isAlive = true;
        aliveCell.SetActive(true);
        deadCell.SetActive(false);
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
