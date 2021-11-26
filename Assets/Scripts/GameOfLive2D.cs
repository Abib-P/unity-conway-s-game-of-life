using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOfLive2D : MonoBehaviour
{
    [SerializeField]
    GameObject aliveCell;

    [SerializeField]
    GameObject deadCell;

    Cell[,] maps;
    List<bool[,]> precedentStates;

    [SerializeField]
    Texture2D cellTexture;

    [SerializeField, Range(1, 100)]
    int width = 17;

    [SerializeField, Range(1, 100)]
    int height = 17;

    float timePast = 0;

    [SerializeField, Range(0, 2)]
    float timeNeeded = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        maps = new Cell[height, width];
        precedentStates = new List<bool[,]>();
        GenerateCells();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));

            if (Physics.Raycast(ray, out var hitInfo))
            {
                CellBehaviour2D cell = hitInfo.collider.gameObject.GetComponent<CellBehaviour2D>();
                Vector2Int pos = cell.GetPosition();
                if (maps[pos.x, pos.y].IsAlive())
                {
                    maps[pos.x, pos.y].Kill();
                }
                else
                {
                    maps[pos.x, pos.y].Revive();
                }
            }
        }

        timePast += Time.deltaTime;

        if (timePast > timeNeeded)
        {
            UpdateMap();
            timePast -= timeNeeded;
        }
    }

    private void UpdateMap()
    {
        bool[,] lastState = new bool[height, width];
        bool[,] newMap = new bool[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                lastState[i, j] = maps[i, j].IsAlive();
                newMap[i, j] = UpdateCell(i, j);
            }
        }
        precedentStates.Add(lastState);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (newMap[i, j])
                {
                    maps[i, j].Revive();
                }
                else
                {
                    maps[i, j].Kill();
                }
            }
        }
    }

    private bool UpdateCell(int i, int j)
    {
        int countNeighbours = CountNeighbours(i, j);
        if (maps[i, j].IsAlive())
        {
            if (countNeighbours == 2 || countNeighbours == 3)
            {
                return true;
            }

        }
        else
        {
            if (countNeighbours == 3)
            {
                return true;
            }
        }
        return false;
    }

    private int CountNeighbours(int i, int j)
    {
        int neighboursCount = 0;
        for (int k = i - 1; k <= i + 1; k++)
        {
            for (int l = j - 1; l <= j + 1; l++)
            {
                if (k < 0 || height <= k)
                {
                    continue;
                }
                if (l < 0 || width <= l)
                {
                    continue;
                }
                if (k == i && l == j)
                {
                    continue;
                }
                if (maps[k, l].IsAlive())
                {
                    neighboursCount++;
                }
            }
        }
        return neighboursCount;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Stop time / Resume"))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }
        if (GUI.Button(new Rect(10, 150, 150, 100), "Update"))
        {
            UpdateMap();
        }
        if (GUI.Button(new Rect(10, 300, 150, 100), "Load Last State"))
        {
            LoadLastState();
        }
        if (GUI.Button(new Rect(10, 450, 150, 100), "Clear"))
        {
            ClearMap();
        }
        if (GUI.Button(new Rect(10, 600, 150, 100), "3D Game of Life"))
        {
            SceneManager.LoadScene("3D Game Of Life");
        }
    }

    private void ClearMap()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                maps[i, j].Kill();
            }
        }
    }

    private void LoadLastState()
    {
        if (precedentStates.Count > 0)
        {
            bool[,] lastState = precedentStates[precedentStates.Count - 1];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (lastState[i, j])
                    {
                        maps[i, j].Revive();
                    }
                    else
                    {
                        maps[i, j].Kill();
                    }
                }
            }
            precedentStates.RemoveAt(precedentStates.Count - 1);
        }
    }

    void GenerateCells()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject aliveObj = Instantiate(aliveCell);
                GameObject deadObj = Instantiate(deadCell);
                aliveObj.transform.position = new Vector3(i * 1f, j * 1f, 0);
                deadObj.transform.position = new Vector3(i * 1f, j * 1f, 0);
                CellBehaviour2D aliveCellBehaviour = aliveObj.GetComponent<CellBehaviour2D>();
                aliveCellBehaviour.SetPosition(i, j);
                CellBehaviour2D deadCellBehaviour = deadObj.GetComponent<CellBehaviour2D>();
                deadCellBehaviour.SetPosition(i, j);
                maps[i, j] = new Cell(aliveObj, deadObj);
                if (Random.Range(0, 3) == 0)
                {
                    maps[i, j].Revive();
                }
            }
        }
    }
}
