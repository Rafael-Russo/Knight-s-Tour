using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public int boardSize = 8;
    public GameObject wSquare;
    public GameObject bSquare;

    public GameObject knightPrefab;
    public GameController gameController;
    public KnightsTour ktController;
    protected internal GameObject currentKnight;
    private TreeNode knightTreeNode;
    private GameObject[,] tiles;

    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameController.isRunning)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null)
                {
                    Generate();
                    PlaceOrMoveKnight(hit.transform.position);
                }
            }
        }
    }

    public void CenterCamera(Camera mainCamera)
    {
        Vector3 centerPoint = new Vector3(boardSize / 2f, 10f, boardSize / 2f);
        mainCamera.transform.position = centerPoint;
        mainCamera.transform.LookAt(centerPoint - new Vector3(0, 10f, 0));
    }

    public void Generate(int size)
    {
        this.boardSize = size;
        Generate();
    }
    public void Generate()
    {
        GameObject[,] tiles = new GameObject[boardSize, boardSize];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        bool white = false;
        for (int x=0; x<boardSize; x++)
        {
            for(int y=0; y<boardSize; y++)
            {
                GameObject cube;
                if (white){
                    cube = wSquare;
                }
                else
                {
                    cube = bSquare;
                }

                white = !white;
                GameObject board = Instantiate(cube, new Vector3(x, 0, y), Quaternion.identity);
                board.transform.parent = transform;
                tiles[x, y] = board;
            }
            white = !white;
        }

        ktController.SetBoardTiles(tiles);
        SetBoardTiles(tiles);
        CenterCamera(gameController.mainCamera);
    }

    public GameObject GetTileAtPosition(Vector2Int position)
    {
        if (position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize)
        {
            return tiles[position.x, position.y];
        }
        else
        {
            return null;
        }
    }

    public void SetBoardTiles(GameObject[,] newTiles)
    {
        tiles = newTiles;
    }

    public void PlaceOrMoveKnight(Vector3 position)
    {
        Vector3 knightPosition = new Vector3(position.x, position.y + 0.5f, position.z);
        Quaternion knightRotation = Quaternion.Euler(-90, 90, 0);

        if (currentKnight == null)
        {
            ktController.PrepareKnightsTour(new Vector2(position.x, position.z), this, gameController);
            ktController.visitedPositions = new bool[boardSize, boardSize];
            Debug.Log("Colocando o cavalo na posição: " + knightPosition);
            currentKnight = Instantiate(knightPrefab, knightPosition, knightRotation);
            knightTreeNode = new TreeNode(new Vector2(position.x, position.z), ktController, this);
            
            knightTreeNode.CalculateMoves();
        }
        else
        {
            Debug.Log("Movendo o cavalo para a posição: " + knightPosition);
            currentKnight.transform.position = knightPosition;
            currentKnight.transform.rotation = knightRotation;
            ktController.ChangeTileColor(new Vector2(position.x, position.z), Color.blue);
            knightTreeNode.CalculateMoves();
        }

        ktController.visitedPositions[(int)position.x, (int)position.z] = true;

        if (ktController.visitedPositions[(int)position.x, (int)position.z])
        {
            ktController.ChangeTileColor(new Vector2(position.x, position.z), Color.blue);
        }
    }
}
