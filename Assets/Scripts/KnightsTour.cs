using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightsTour : MonoBehaviour
{
    public float mvSpeed = 0.5f;
    private GameController gameController;
    private BoardController boardController; // Referência ao BoardController
    private TreeNode currentNode; // Nodo atual no tour
    protected internal List<Vector2> path; // Caminho do Knight's Tour
    protected internal bool tourComplete = false; // Sinaliza se o tour está completo
    public GameObject[,] boardTiles;
    public GameObject[,] boardTilesAux;
    public bool[,] visitedPositions;

    protected internal void PrepareKnightsTour(Vector2 startPos, BoardController bController, GameController gameController)
    {
        this.gameController = gameController;
        path = new List<Vector2>();
        boardController = bController;
        if (visitedPositions == null)
        {
            visitedPositions = new bool[boardController.boardSize, boardController.boardSize];
        }
        currentNode = new TreeNode(startPos, this, boardController, null);
        path.Add(startPos);
    }

    protected internal void StartKnightsTour()
    {
        StartCoroutine(FindTour());
    }

    public void SetBoardTiles(GameObject[,] tiles)
    {
        boardTilesAux = tiles;
        boardTiles = tiles;
    }

    protected internal void ChangeTileColor(Vector2 position, Color color)
    {
        int x = (int)position.x;
        int y = (int)position.y;

        if (x >= 0 && x < boardController.boardSize && y >= 0 && y < boardController.boardSize)
        {
            Renderer tileRenderer = boardTiles[x, y].GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                Material material = tileRenderer.material;

                Color currentColor = material.color;
                Color mixedColor = Color.Lerp(currentColor, color, 0.25f);

                material.color = mixedColor;
            }
        }
    }

    IEnumerator FindTour()
    {
        Stack<Vector2> pathStack = new Stack<Vector2>();
        pathStack.Push(currentNode.position);
        visitedPositions[(int)currentNode.position.x, (int)currentNode.position.y] = true;

        while (!tourComplete)
        {
            currentNode.CalculateMoves();
            if (currentNode.Children.Count > 0)
            {
                GameObject nextTile = ChooseNextTile(currentNode.Children);
                Vector2 nextPosition = new Vector2(nextTile.transform.position.x, nextTile.transform.position.z);
                pathStack.Push(nextPosition);
                visitedPositions[(int)nextPosition.x, (int)nextPosition.y] = true;
                currentNode = new TreeNode(nextPosition, this, boardController, currentNode); // Define o pai como o nó atual

                MoveKnight(nextTile);
                yield return new WaitForSeconds(mvSpeed);

                if (IsTourComplete())
                {
                    tourComplete = true;
                    gameController.isRunning = false;
                    Debug.Log("Tour completo.");
                }
            }
            else
            {
                if (pathStack.Count > 1)
                {
                    while (pathStack.Count > 1 && currentNode.Children.Count == 0)
                    {
                        Vector2 lastPosition = pathStack.Pop();
                        visitedPositions[(int)lastPosition.x, (int)lastPosition.y] = false;
                        Renderer tileRenderer = boardTilesAux[(int)lastPosition.x, (int)lastPosition.y].GetComponent<Renderer>();
                        ChangeTileColor(new Vector2((int)lastPosition.x, (int)lastPosition.y), tileRenderer.material.color);
                        currentNode = currentNode.parent;
                        MoveKnight(boardController.GetTileAtPosition(new Vector2Int((int)lastPosition.x, (int)lastPosition.y)));
                        yield return new WaitForSeconds(mvSpeed);
                    }

                    if (currentNode.Children.Count == 0)
                    {
                        tourComplete = true;
                        gameController.isRunning = false;
                        Debug.Log("Não é possível completar o tour.");
                    }
                }
                else
                {
                    tourComplete = true;
                    gameController.isRunning = false;
                    Debug.Log("Não é possível completar o tour.");
                }
            }
        }
    }



    bool IsTourComplete()
    {
        // Verificar se todas as posições foram visitadas
        for (int x = 0; x < boardController.boardSize; x++)
        {
            for (int y = 0; y < boardController.boardSize; y++)
            {
                if (!visitedPositions[x, y])
                    return false;
            }
        }
        return true;
    }

    GameObject ChooseNextTile(List<GameObject> tiles)
    {
        // Prioriza os movimentos com menos opções de retrocesso
        tiles.Sort((tile1, tile2) =>
        {
            int count1 = currentNode.CountAvailableMoves(tile1);
            int count2 = currentNode.CountAvailableMoves(tile2);
            return count1.CompareTo(count2);
        });

        // Escolhe o próximo tile dos movimentos com menos opções de retrocesso
        return tiles[0];
    }

    void MoveKnight(GameObject tile)
    {
        // Converte a posição para 3D e move o cavalo (usando o BoardController)
        Vector3 knightPosition = tile.transform.position + new Vector3(0, 0.5f, 0);
        boardController.PlaceOrMoveKnight(knightPosition);
    }
}
