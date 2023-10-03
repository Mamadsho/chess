using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Piece [,] BoardState = new Piece[8,8];
    public Piece selectedPiece;
    public Transform boardCoordinateSystem;
    public float pieceLerping = 0.01f;
    public int promotionPreference = 0; // 0 Queen, 1 Rook, 2 Knight, 3 Bishop
    [SerializeField] private Piece enPassant;
    static public GameManager Singleton;
    public bool isWhiteTurn = true;
    public bool AI = false;
    [SerializeField] Piece[] whitePromotionPrefabs;
    [SerializeField] Piece[] blackPromotionPrefabs;
    
    void Awake()
    {
        Singleton = this;
    }

    // Should be reconsidered
    // some critical parts of an abstract move happen inside this gameManager related function.
    // e.g. promotion, turn, (passant?)...
    //   ideal would be to have a _move function that
    //   takes a BoardState, Move,
    //   and return a new BoardState
    public int Move( Piece piece, int x, int y)
    {
        // Validation
        if (x < 0 || x > 7 || y < 0 || y > 7 || GetValidMoves(piece)[x, y] == 0) return 0;

        // King, Rook first move registration
        if (piece.type == 'k') ((King)piece).moved = true;
        if (piece.type == 'r') ((Rook)piece).moved = true;

        // MoveState
        MoveState newState = _move(BoardState, piece, x, y);
        BoardState = newState.board;

        enPassant = newState.enPassant;

        // Destroy piece
        Destroy(newState.pieceToDestroy);

        // Promotion:
        if (piece.type == 'p' && piece.color == 'w' && y == 7)
        {
            Destroy(piece);
            Piece newQueen = Instantiate(whitePromotionPrefabs[promotionPreference], boardCoordinateSystem, false);
            BoardState[x, 7] = newQueen;
            newQueen.transform.localPosition = new Vector3(x + .5f, 7.5f, 0);
            newQueen.pos = new Vector2Int(x, 7);
        }
        if (piece.type == 'p' && piece.color == 'b' && y == 0)
        {
            Destroy(piece);
            Piece newQueen = Instantiate(blackPromotionPrefabs[promotionPreference], boardCoordinateSystem, false);
            BoardState[x, 0] = newQueen;
            newQueen.transform.localPosition = new Vector3(x + .5f, .5f, 0);
            newQueen.pos = new Vector2Int(x, 0);
        }

        // Update the visuals
        piece.pos = new Vector2Int(x, y);
        if(newState.rookToMove) newState.rookToMove.pos = newState.rookNewPos;

        // Changing Turn
        isWhiteTurn = !isWhiteTurn;

        return 1;
    }

    class MoveState
    {
        public Piece[,] board;
        public Piece rookToMove;
        public Vector2Int rookNewPos;
        public Piece pieceToDestroy;
        public Piece enPassant;
    }

    // returns Class { Piece[,] state after move,Piece[] to move, Piece to destroy)
    // this implementation does not contain promotion.
    // a better implementation probably should.
    private MoveState _move( Piece[,] boardState, Piece piece, int x, int y )
    {
        MoveState result = new MoveState();

        Piece[,] board = (Piece[,]) boardState.Clone();

        // Kill piece if its in [x,y]
        if (board[x, y])
        {
            //Destroy(board[x, y]);
            result.pieceToDestroy = board[x,y];
        }

        // Castling
        if (piece.type == 'k' && Mathf.Abs(piece.pos.x - x) > 1) // if king jumps farther than 1 cell
        {
            if( x == 2 ) // Moves Left
            {
                if (board[0, y] &&               // check possible rook position
                    board[0, y].type == 'r' &&   // if there is a rook on left end
                    board[3, y] == null          // wether rook new position is empty
                    )     
                {
                    // rook goes to [3, y]
                    board[3, y] = board[0, y];
                    board[0, y] = null;
                    result.rookToMove = board[3, y];
                    result.rookNewPos = new Vector2Int(3,y);

                    // king is the active piece. so ...
                    // ... its pos is updated somewhere below in this code!
                }
            }
            else // pos == 6
            {
                if (board[7, y] &&               // check possible rook position
                    board[7, y].type == 'r' &&   // if there is a rook on right end
                    board[5, y] == null          // wether rook new position is empty
                    )     
                {
                    // rook goes to [5, piece.pos.y]
                    board[5, y] = board[7, y];
                    board[7, y] = null;
                    result.rookToMove = board[5, y];
                    result.rookNewPos = new Vector2Int(5, y);

                    // king is the active piece. so ...
                    // ... its pos is updated somewhere below in this code!
                }
            }
        }
        // enPassant killing
        if (enPassant) {
            if (enPassant.color == 'w' && x == enPassant.pos.x && y == 2 ||
                enPassant.color == 'b' && x == enPassant.pos.x && y == 5) {
                board[enPassant.pos.x, enPassant.pos.y] = null;
                result.pieceToDestroy = enPassant;
            }
        }
        // enPassant storing
        if (piece.type == 'p' && piece.color == 'w' && y == 3 ||
            piece.type == 'p' && piece.color == 'b' && y == 4) {
            result.enPassant = piece;
        } else {
            result.enPassant = null;
        }

        // Update board (Empty Move)
        board[piece.pos.x, piece.pos.y] = null;
        board[x, y] = piece;

        result.board = board;
        return result;
    }

    public int [,] GetValidMoves(Piece piece)
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
        };
        validMoves[piece.pos.x, piece.pos.y]= 0;*/
        int[,] validMoves = piece.ValidMoves();
        

        // Invalidate every valid move after which the state has king of piece color in check
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (validMoves[x,y]!= 0)
                {
                    Piece[,] state = _move(BoardState, piece, x, y).board;
                    string check = _kingInCheck(state, piece.color) ? "check" : "free";
                    _logBoard(state, $"Checking State: {check}");
                    if (_kingInCheck(state, piece.color)) validMoves[x, y] = 0;
                }
            }
        }

        // Validate enPassant capturing
        // 1. low level check
        // 2. check for king in check after possible capturing enPassant
        if (enPassant && piece.type == 'p' &&
            piece.pos.y == enPassant.pos.y &&
            Mathf.Abs(piece.pos.x - enPassant.pos.x) == 1)
        {
            int y = piece.color == 'w' ? 5 : 2;
            validMoves[enPassant.pos.x, y] = 3; // can be combined with last line of this block

            Piece[,] state = _move(BoardState, piece, enPassant.pos.x, y).board;
            string check = _kingInCheck(state, piece.color) ? "check" : "free";
            _logBoard(state, $"Checking enPassant State: {check}");
            if (_kingInCheck(state, piece.color)) validMoves[enPassant.pos.x, y] = 0;
        }

        // Validate Castling
        if (piece.type == 'k')
        {
            int y = piece.pos.y;
            King king = (King) piece;
            if (!king.moved && !_kingInCheck(BoardState, king.color))
            {
                //validate left side rook
                if (BoardState[0, y] &&
                    BoardState[0, y].type == 'r')
                {
                    Rook rook = (Rook) BoardState[0, y];
                    if (!rook.moved && !BoardState[1, y] && !BoardState[2, y] && !BoardState[3, y])
                    {
                        validMoves[2, y] = 4;
                        Piece[,] state = _move(BoardState, king, 2, y).board;
                        string check = _kingInCheck(state, king.color) ? "check" : "free";
                        _logBoard(state, $"Checking Castling State: {check}");
                        if (_kingInCheck(state, king.color)) validMoves[2, y] = 0;
                        state = _move(BoardState, king, 3, y).board;
                        check = _kingInCheck(state, king.color) ? "check" : "free";
                        _logBoard(state, $"Checking Castling State: {check}");
                        if (_kingInCheck(state, king.color)) validMoves[2, y] = 0;
                    }
                }
                //validate right side rook
                if (BoardState[7,y] &&
                    BoardState[7,y].type == 'r')
                {
                    Rook rook = (Rook)BoardState[7,y];
                    if(!rook.moved && !BoardState[5,y] && !BoardState[6, y])
                    {
                        validMoves[6, y] = 4;
                        Piece[,] state = _move(BoardState, king, 5, y).board;
                        string check = _kingInCheck(state, king.color) ? "check" : "free";
                        _logBoard(state, $"Checking Castling State: {check}");
                        if (_kingInCheck(state, king.color)) validMoves[6, y] = 0;
                        state = _move(BoardState, king, 6, y).board;
                        check = _kingInCheck(state, king.color) ? "check" : "free";
                        _logBoard(state, $"Checking Castling State: {check}");
                        if (_kingInCheck(state, king.color)) validMoves[6, y] = 0;
                    }
                }
            }
        }

        // Promotion:
        if (piece.type == 'p')
        {
            if(piece.pos.y == 6 && piece.color == 'w')
            {
                for (int x = 0; x < 8; x++)
                {
                    validMoves[x, 7] = validMoves[x, 7] == 0 ? 0 : 5;
                }
            }
            if(piece.pos.y==1 && piece.color == 'b')
            {
                for (int x = 0; x < 8; x++)
                {
                    validMoves[x, 0] = validMoves[x, 0] == 0 ? 0 : 5;
                }
            }
        }
        /*if (piece.type == 'p' && piece.color == 'w' && piece.pos.y == 6
            && validMoves[piece.pos.x, 7] != 0)
            validMoves[piece.pos.x, 7] = 5;
        if (piece.type == 'p' && piece.color == 'b' && piece.pos.y == 1
            && validMoves[piece.pos.x, 0] != 0)
            validMoves[piece.pos.x, 0] = 5;*/

        // 0 invalid, 1 empty move, 2 kill, 3 enPassant, 4 castling, 5 promotion
        return validMoves;
    }

    public bool KingInCheck(char color) {
        return _kingInCheck(BoardState, color);
    }
    private bool _kingInCheck(Piece[,] board, char color)
    {
        // Algorithm:
        // 1. Find The Kings and Store their positions
        // 2. Check whether their positions are in valid moves of any piece
        // definitely algorithm can be improved)

        // 1. Find The King and store its position
        Vector2Int king = new Vector2Int(-1, -1);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (board[x, y] == null) continue;
                if (board[x, y].type == 'k' && board[x, y].color == color) {
                    king = new Vector2Int(x, y);
                }
            }
        }

        // 2. Test wether position of the king is in validMoves of any piece.
        for (int y = 0; y<8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (board[x, y] == null) continue;
                int[,] validMoves = board[x, y]._validMoves(board, x, y);
                if (validMoves[king.x, king.y] != 0) return true;
            }
        }
        return false;
    }

    public bool CheckMate()
    {
        char color = isWhiteTurn ? 'w' : 'b';
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (BoardState[x, y] && BoardState[x, y].color == color)
                {
                    int[,] validMoves = GetValidMoves(BoardState[x, y]);
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (validMoves[i, j] != 0) return false;
                        }
                    }
                }
            }
        }
        
        return true;
    }
    public void LogBoard()
    {
        _logBoard(BoardState, "BoardState:");
    }

    private void _logBoard(Piece [,] board, string message)
    {
        string result = message;
        for (int y = 7; y >= 0; y--)
        {
            result += "\n";
            for (int x = 0; x < 8; x++)
            {
                if (board[x, y])
                {
                    result += (board[x, y].color == 'w') ? board[x, y].type.ToString().ToUpper() + " " : board[x, y].type + " ";
                }
                else
                {
                    result += "- ";
                }
            }
        }
        Debug.Log(result);
    }

}
