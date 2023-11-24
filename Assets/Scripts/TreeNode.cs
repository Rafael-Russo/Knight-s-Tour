using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{
    public TreeNode parent;
    public List<GameObject> Children;
    public Vector2 position;
    private KnightsTour ktController;
    private BoardController boardController;

    public TreeNode(Vector2 initialPosition, KnightsTour ktController, BoardController bController, TreeNode parentNode = null)
    {
        this.position = initialPosition;
        this.ktController = ktController;
        Children = new List<GameObject>();
        boardController = bController;
        this.parent = parentNode;
    }

    public void CalculateMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>()
        {
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, 2),
            new Vector2Int(-1, -2)
        };

        foreach (Vector2Int move in possibleMoves)
        {
            Vector2Int newPosition = new Vector2Int((int)position.x + move.x, (int)position.y + move.y);
            if (IsPositionValid(newPosition))
            {
                GameObject tile = boardController.GetTileAtPosition(newPosition);
                if (tile != null)
                {
                    TreeNode childNode = new TreeNode(newPosition, ktController, boardController, this);
                    Children.Add(tile);
                }
            }
        }
    }

    public int CountAvailableMoves(GameObject tile)
    {
        Vector2 tilePosition = new Vector2(tile.transform.position.x, tile.transform.position.z);
        int count = 0;

        List<Vector2Int> possibleMoves = new List<Vector2Int>()
    {
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1),
        new Vector2Int(1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-1, 2),
        new Vector2Int(-1, -2)
    };

        foreach (Vector2Int move in possibleMoves)
        {
            Vector2Int newPosition = new Vector2Int((int)tilePosition.x + move.x, (int)tilePosition.y + move.y);
            if (IsPositionValid(newPosition) && !ktController.visitedPositions[newPosition.x, newPosition.y])
            {
                count++;
            }
        }

        return count;
    }


    private bool IsPositionValid(Vector2 position)
    {
        return position.x >= 0 && position.x < boardController.boardSize && position.y >= 0 && position.y < boardController.boardSize
               && !ktController.visitedPositions[(int)position.x, (int)position.y];
    }
}