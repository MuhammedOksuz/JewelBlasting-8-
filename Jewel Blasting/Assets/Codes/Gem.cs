using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [HideInInspector]public Vector2Int posIndex;
    [HideInInspector]public Board board;

    public Vector2 firstTouchPos;
    public Vector2 lastTouchPos;

    bool clickedMouse;
    float dragAngle;
    Gem otherGem;//yerine süreklediðim gem, yer deðiþtirmel istediðim gem

    public enum GemType { blue, pink, yellow, lightGreen, darkGreen, bomb };
    public GemType type;

    public bool isItMatch;//eþleþtimi ?
    Vector2Int firstPos;
    //Efects
    public GameObject particalEffect;

    public int validScore;
    private void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > 0.01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x,posIndex.y);
        }

        if (clickedMouse && Input.GetMouseButtonUp(0) && !UIControl.instance.isItLevelComplate) 
        {
            clickedMouse = false;

            if (board.validStatus == Board.BoardStatus.moveing)
            {
                lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }
    }
    public int bombValume;//bomba hacmi
    public void EditGems(Vector2Int pos, Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }
    private void OnMouseDown()
    {
        if (board.validStatus == Board.BoardStatus.moveing && !UIControl.instance.isItLevelComplate) 
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickedMouse = true;
        }
        
    }
    void CalculateAngle() //Açýyý hesapla.
    {
        float changex = lastTouchPos.x - firstTouchPos.x;//deðiþim x
        float changey = lastTouchPos.y - firstTouchPos.y;//deðiþim y
        dragAngle = Mathf.Atan2(changey, changex);
        dragAngle = dragAngle * 180 / Mathf.PI;
        if (Vector3.Distance(firstTouchPos, lastTouchPos) > 0.5f)
        {
            DragTile();
        }
    }
    void DragTile()
    {
        firstPos = posIndex;
        if (dragAngle < 45 && dragAngle > -45 && posIndex.x < board.horizontal - 1)
        {
            //iþaretlediðimiz 0,0 açýya göre x 'i 1 artýrýp o açýdaki objeyi buluyoruz.
            otherGem = board.allGems[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--; //bulduðumuz objenin x 'ini 1 azaltýp yer iþaretli konuma alýyoruz.
            posIndex.x++;//bulunduðumuz posIndex'i 1 artýrýyoruz.
        }
        else if (dragAngle > 45 && dragAngle < 135 && posIndex.y < board.vertical - 1)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y + 1];
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (dragAngle > 135 || dragAngle < -135 && posIndex.x > 0)
        {
            otherGem = board.allGems[posIndex.x - 1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--;
        }
        else if (dragAngle > -135 && dragAngle < -45 && posIndex.y > 0)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        board.allGems[posIndex.x, posIndex.y] = this;
        board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

        StartCoroutine(ControlMovement());
    }
    public IEnumerator ControlMovement()
    {
        board.validStatus = Board.BoardStatus.waiting;
        yield return new WaitForSeconds(.5f);
        board.gameManager.FindMatches();
        if (otherGem != null)
        {
            if (!isItMatch && !otherGem.isItMatch)
            {
                otherGem.posIndex = posIndex;
                posIndex = firstPos;

                board.allGems[posIndex.x, posIndex.y] = this;
                board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;
                yield return new WaitForSeconds(0.5f);
                board.validStatus = Board.BoardStatus.moveing;
            }
            else
            {
                board.DestroyAllMatchingGems();
            }
        }

    }

}
