using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override int[,] _validMoves(Piece[,] board, int x, int y)
    {
        int[,] r = new int[8, 8];

        int i, j;

        // Top left
        i = x;
        j = y;
        while (true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8) break;

            if (!isMovableCell(board, i, j, ref r)) break;
        }

        // Top right
        i = x;
        j = y;
        while (true)
        {
            i++;
            j++;
            if (i >= 8 || j >= 8) break;

            if (!isMovableCell(board, i, j, ref r)) break;
        }

        // Down left
        i = x;
        j = y;
        while (true)
        {
            i--;
            j--;
            if (i < 0 || j < 0) break;

            if (!isMovableCell(board, i, j, ref r)) break;
        }

        // Down right
        i = x;
        j = y;
        while (true)
        {
            i++;
            j--;
            if (i >= 8 || j < 0) break;

            if (!isMovableCell(board, i, j, ref r)) break;
        }

        return r;
    }
}
