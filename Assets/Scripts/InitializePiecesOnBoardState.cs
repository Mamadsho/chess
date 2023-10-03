using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializePiecesOnBoardState : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] Piece[] pieces;
    void Start()
    {
        gameManager = GameManager.Singleton;
        foreach (Piece piece in pieces) {
            piece.pos = new Vector2Int((int)piece.transform.localPosition.x, (int)piece.transform.localPosition.y); // get position of piece on Board coordinates
            if (piece.pos.x < 0 || piece.pos.x > 7 || piece.pos.y < 0 || piece.pos.y > 7 || // if out of bounds of board
                gameManager.BoardState[piece.pos.x, piece.pos.y])
            {  // if the board position is not empty
                Destroy(piece.gameObject);
            }
            else
            {
                gameManager.BoardState[piece.pos.x, piece.pos.y] = piece; // place piece on BoardState
            }
        }  
    }
}
