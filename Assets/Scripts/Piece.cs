using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector2Int pos; // !Must only be changed from GameManager;
    public char type;
    public char color;

    GameManager gameManager;
    public void Start()
    {
        gameManager = GameManager.Singleton;
    }

    void Update()
    {
        transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, pos.x+.5f, gameManager.pieceLerping),
                                              Mathf.Lerp(transform.localPosition.y, pos.y+.5f, gameManager.pieceLerping),
                                                         0);
    }
    public void Move(int x, int y)
    {
        int success = gameManager.Move(this, x, y); // 1 for success, 0 for fail
        gameManager.LogBoard();
    }

    public void OnDestroy()
    {
        //Might be sth else, like moving the piece to a side
        Destroy(gameObject);
    }

    public virtual int[,] _validMoves(Piece[,] board, int x, int y)
    {
        /*int[,] validMoves = new int[,] { // ! IMPORTANT ! Board view would seem rotated in code 
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 }
        };*/
        int[,] validMoves = new int[8, 8]; 
        validMoves[x, y] = 0;
        // 0 invalid, 1 empty move, 2 kill ...
        return validMoves;
    }

    public virtual int[,] ValidMoves() {
        return _validMoves(gameManager.BoardState, pos.x, pos.y);
    }
    public bool isMovableCell(Piece [,] board, int x, int y, ref int[,] r)
    {
        if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
        {
            Piece cell = board[x, y];
            if (cell == null)
            {
                r[x, y] = 1;
                return true;
            }
            else
            {
                if (color != cell.color)
                    r[x, y] = 2;
                return false;
            }
        }
        return false;
    }
}
