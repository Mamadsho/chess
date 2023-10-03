using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class BoardMove : MonoBehaviour
{
    [SerializeField] GameObject Promotion;
    GameManager gameManager;
    float holdTime = 0;
    bool promoting = false;
    Vector3 startMousePos;
    Vector2Int startCellPos;

    private void Start()
    {
        gameManager = GameManager.Singleton;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(startMousePos);
            RaycastHit hit;
            if (gameManager.selectedPiece)
            {
                if (Physics.Raycast(ray, out hit, 100, 8)) //8 is bitmask for layer Board: 0000 1000
                {
                    Vector3 localPos = gameManager.boardCoordinateSystem.InverseTransformPoint(hit.point);
                    startCellPos = new Vector2Int((int)localPos.x, (int)localPos.y);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            holdTime += Time.deltaTime;
            
            //Activate Promotion Prefab
            if (!promoting &&
                gameManager.selectedPiece && 
                (gameManager.selectedPiece.pos.y == 6 || gameManager.selectedPiece.pos.y == 1) &&
                // wether the position is valid promotion(5) position (line below)
                gameManager.GetValidMoves(gameManager.selectedPiece)[startCellPos.x, startCellPos.y] == 5 &&
                (holdTime > .5f || Vector3.Distance(Input.mousePosition, startMousePos) > 50 ))
            {
                promoting = true;
                Promotion.transform.localPosition = new Vector3(startCellPos.x+.5f, startCellPos.y+.5f, 0);
                Promotion.transform.localRotation = 
                    gameManager.isWhiteTurn? Quaternion.identity : Quaternion.Euler(new Vector3(0, 0, 180));
                Promotion.SetActive(true);
            }
        }

        if (Input.GetMouseButtonUp(0)){

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (gameManager.selectedPiece)
            {
                if (Physics.Raycast(ray, out hit, 100, 8)) //8 is bitmask for layer Board: 0000 1000
                {
                    Vector3 localPos = gameManager.boardCoordinateSystem.InverseTransformPoint(hit.point);
                    if(promoting)
                    {
                        gameManager.promotionPreference = Promotion.GetComponent<SelectPiece>().selectedIndex;
                        gameManager.selectedPiece.Move(startCellPos.x, startCellPos.y);
                        gameManager.promotionPreference = 0;
                    }
                    else
                    {
                        gameManager.selectedPiece.Move((int)localPos.x, (int)localPos.y);
                    }
                    gameManager.selectedPiece = null;
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, 100, gameManager.isWhiteTurn ? 64 : 128)) //bit mask for layers WhitePiece and BlackPiece: 1100 0000
                {
                    gameManager.selectedPiece = hit.transform.GetComponent<Piece>();
                }
            }

            holdTime = 0;
            Promotion.SetActive(false);
            promoting = false;
        }       
    }
}
