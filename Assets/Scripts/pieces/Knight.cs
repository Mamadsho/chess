using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override int[,] _validMoves(Piece[,] board, int x, int y)
    {
        int[,] r = new int[8, 8];

        // Up left
        isMovableCell(board, pos.x - 1, pos.y + 2, ref r);

        // Up right
        isMovableCell(board, pos.x + 1, pos.y + 2, ref r);

        // Down left
        isMovableCell(board, pos.x - 1, pos.y - 2, ref r);

        // Down right
        isMovableCell(board, pos.x + 1, pos.y - 2, ref r);


        // Left Down
        isMovableCell(board, pos.x - 2, pos.y - 1, ref r);

        // Right Down
        isMovableCell(board, pos.x + 2, pos.y - 1, ref r);

        // Left Up
        isMovableCell(board, pos.x - 2, pos.y + 1, ref r);

        // Right Up
        isMovableCell(board, pos.x + 2, pos.y + 1, ref r);

        return r;
    }
}
