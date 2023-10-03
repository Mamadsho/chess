using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public bool moved = false;
    public override int[,] _validMoves(Piece[,] board, int x, int y)
    {
        int[,] r = new int[8, 8];

        isMovableCell(board, x + 1, y, ref r); // up
        isMovableCell(board, x - 1, y, ref r); // down
        isMovableCell(board, x, y - 1, ref r); // left
        isMovableCell(board, x, y + 1, ref r); // right
        isMovableCell(board, x + 1, y - 1, ref r); // up left
        isMovableCell(board, x - 1, y - 1, ref r); // down left
        isMovableCell(board, x + 1, y + 1, ref r); // up right
        isMovableCell(board, x - 1, y + 1, ref r); // down right

        return r;
    }
}
