using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public bool moved = false;
    public override int[,] _validMoves(Piece[,] board, int x, int y)
    {
        int[,] r = new int[8, 8];

        int i;

        // Right
        i = x;
        while (true)
        {
            i++;
            if (i >= 8) break;

            if (!isMovableCell(board, i, y, ref r)) break;
        }

        // Left
        i = x;
        while (true)
        {
            i--;
            if (i < 0) break;

            if (!isMovableCell(board, i, y, ref r)) break;
        }

        // Up
        i = y;
        while (true)
        {
            i++;
            if (i >= 8) break;

            if (!isMovableCell(board, x, i, ref r)) break;
        }

        // Down
        i = y;
        while (true)
        {
            i--;
            if (i < 0) break;

            if (!isMovableCell(board, x, i, ref r)) break;

        }

        return r;
    }
}
