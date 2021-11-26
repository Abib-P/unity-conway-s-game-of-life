using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOfLive3D : MonoBehaviour
{
    [SerializeField]
    GameObject aliveCell;

    [SerializeField]
    GameObject deadCell;

    Cell[,,] maps;
    List<bool[,,]> precedentStates;

    [SerializeField]
    Texture2D cellTexture;

    [SerializeField, Range(1, 100)]
    int width = 17;

    [SerializeField, Range(1, 100)]
    int height = 17;

    [SerializeField, Range(1, 100)]
    int deppness = 17;

    float timePast = 0;

    [SerializeField, Range(0, 2)]
    float timeNeeded = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        maps = new Cell[height, width, deppness];
        precedentStates = new List<bool[,,]>();
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
                CellBehaviour3D cell = hitInfo.collider.gameObject.GetComponent<CellBehaviour3D>();
                Vector3Int pos = cell.GetPosition();
                if (maps[pos.x, pos.y, pos.z].IsAlive())
                {
                    maps[pos.x, pos.y, pos.z].Kill();
                }
                else
                {
                    maps[pos.x, pos.y, pos.z].Revive();
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
        bool[,,] lastState = new bool[height, width, deppness];
        bool[,,] newMap = new bool[height, width, deppness];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < deppness; k++)
                {
                    lastState[i, j, k] = maps[i, j, k].IsAlive();
                    newMap[i, j, k] = UpdateCell(i, j, k);
                }
            }
        }
        precedentStates.Add(lastState);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    if (newMap[i, j, k])
                    {
                        maps[i, j, k].Revive();
                    }
                    else
                    {
                        maps[i, j, k].Kill();
                    }
                }
            }
        }
    }

    private bool UpdateCell(int actualHeight, int actualWidth, int actualDeepness)
    {
        int countNeighbours = CountNeighbours(actualHeight, actualWidth, actualDeepness);
        if (maps[actualHeight, actualWidth, actualDeepness].IsAlive())
        {
            if (countNeighbours == 3 || countNeighbours == 10)
            {
                return true;
            }

        }
        else
        {
            if ( countNeighbours == 5 || countNeighbours == 7)
            {
                return true;
            }
        }
        return false;
    }

    private int CountNeighbours(int actualHeight, int actualWidth, int actualDeepness)
    {
        int neighboursCount = 0;
        for (int i = actualHeight - 1; i <= actualHeight + 1; i++)
        {
            for (int j = actualWidth - 1; j <= actualWidth + 1; j++)
            {
                for (int k = actualDeepness - 1; k <= actualDeepness + 1; k++)
                {
                    if (i < 0 || height <= i)
                    {
                        continue;
                    }
                    if (j < 0 || width <= j)
                    {
                        continue;
                    }
                    if (k < 0 || deppness <= k)
                    {
                        continue;
                    }
                    if (i == actualHeight && j == actualWidth && k == actualDeepness)
                    {
                        continue;
                    }
                    if (maps[i, j, k].IsAlive())
                    {
                        neighboursCount++;
                    } 
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
        if (GUI.Button(new Rect(10, 600, 150, 100), "2D Game of Life"))
        {
            SceneManager.LoadScene("2D Game Of Life");
        }
    }

    private void ClearMap()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < deppness; k++)
                {
                    maps[i, j, k].Kill();
                }
            }
        }
    }

    private void LoadLastState()
    {
        if (precedentStates.Count > 0)
        {
            bool[,,] lastState = precedentStates[precedentStates.Count - 1];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < deppness; k++)
                    {
                        if (lastState[i, j, k])
                        {
                            maps[i, j, k].Revive();
                        }
                        else
                        {
                            maps[i, j, k].Kill();
                        }
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
                for (int k = 0; k < deppness; k++)
                {
                    GameObject aliveObj = Instantiate(aliveCell);
                    GameObject deadObj = Instantiate(deadCell);
                    aliveObj.transform.position = new Vector3(i * 1f, j * 1f, k * 1f);
                    deadObj.transform.position = new Vector3(i * 1f, j * 1f, k * 1f);
                    CellBehaviour3D aliveCellBehaviour = aliveObj.GetComponent<CellBehaviour3D>();
                    aliveCellBehaviour.SetPosition(i, j, k);
                    CellBehaviour3D deadCellBehaviour = deadObj.GetComponent<CellBehaviour3D>();
                    deadCellBehaviour.SetPosition(i, j, k);
                    maps[i, j, k] = new Cell(aliveObj, deadObj);
                    if (Random.Range(0, 3) == 0)
                    {
                        maps[i, j, k].Revive();
                    }
                }
            }
        }
    }
}
