using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override int[,] _validMoves(Piece[,] board, int x, int y)
    {
        int[,] r = new int[8, 8];
        if (color == 'w')
        {
            ////// White team move //////

            // Diagonal left
            if (x != 0 && y != 7)
            {

                Piece c = board[x - 1, y + 1];
                if (c != null && c.color != 'w')
                    r[x - 1, y + 1] = 2;
            }

            // Diagonal right
            if (x != 7 && y != 7)
            {

                Piece c = board[x + 1, y + 1];
                if (c != null && c.color != 'w')
                    r[x + 1, y + 1] = 2;
            }

            // Middle
            if (y != 7)
            {
                Piece c = board[x, y + 1];
                if (c == null)
                    r[x, y + 1] = 1;
            }

            // Middle on first move
            if (y == 1)
            {
                Piece c = board[x, y + 1];
                Piece c2 = board[x, y + 2];
                if (c == null && c2 == null)
                    r[x, y + 2] = 1;
            }
        }
        else
        {
            ////// Black team move //////

            // Diagonal left
            if (x != 0 && y != 0)
            {

                Piece c = board[x - 1, y - 1];
                if (c != null && c.color == 'w')
                    r[x - 1, y - 1] = 2;
            }
            // Diagonal right
            if (x != 7 && y != 0)
            {

                Piece c = board[x + 1, y - 1];
                if (c != null && c.color == 'w')
                    r[x + 1, y - 1] = 2;
            }

            // Middle
            if (y != 0)
            {
                Piece c = board[x, y - 1];
                if (c == null)
                    r[x, y - 1] = 1;
            }

            // Middle on first move
            if (y == 6)
            {
                Piece c = board[x, y - 1];
                Piece c2 = board[x, y - 2];
                if (c == null && c2 == null)
                    r[x, y - 2] = 1;
            }
            
        }
        return r;
    }
}
