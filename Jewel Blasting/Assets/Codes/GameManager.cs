using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    Board board;
    public List<Gem> FoundGemList = new List<Gem>();//found = bulunan
    private void Awake()
    {
        board = Object.FindObjectOfType<Board>();
    }
    public void FindMatches()//eþleþmeleri bul
    {
        FoundGemList.Clear();
        for (int x = 0; x < board.horizontal; x++)
        {
            for (int y = 0; y < board.vertical; y++)
            {
                Gem validGem = board.allGems[x, y];
                if (validGem != null)
                {
                    //x'deki Matches(Eþleþmeler)
                    if (x > 0 && x < board.horizontal-1) 
                    {
                        Gem leftGem = board.allGems[x - 1, y];
                        Gem rightGem = board.allGems[x + 1, y];
                        if (leftGem != null && rightGem != null)
                        {
                            if (leftGem.type == validGem.type && rightGem.type == validGem.type)
                            {
                                validGem.isItMatch = true;
                                leftGem.isItMatch = true;
                                rightGem.isItMatch = true;
                                FoundGemList.Add(validGem);
                                FoundGemList.Add(leftGem);
                                FoundGemList.Add(rightGem);
                            }
                        }
                    }
                    //y'deki Matches(Eþleþmeler)
                    if (y > 0 && y < board.vertical- 1)
                    {
                        Gem downGem = board.allGems[x, y - 1];
                        Gem upGem = board.allGems[x, y + 1];
                        if (downGem != null && upGem != null)
                        {
                            if (downGem.type == validGem.type && upGem.type == validGem.type)
                            {
                                validGem.isItMatch = true;
                                downGem.isItMatch = true;
                                upGem.isItMatch = true;
                                FoundGemList.Add(validGem);
                                FoundGemList.Add(downGem);
                                FoundGemList.Add(upGem);
                            }
                        }
                    }
                }
            }
        }//döngüler bitti
        if (FoundGemList.Count > 0)
        {
            FoundGemList = FoundGemList.Distinct().ToList();
        }
        FindBomb();
    }
    public void FindBomb()//Önce bombayý buluyoruz
    {
        for (int i = 0; i < FoundGemList.Count; i++)
        {
            Gem gem = FoundGemList[i];
            int x = gem.posIndex.x;
            int y = gem.posIndex.y;
            if (gem.posIndex.x > 0)
            {
                if (board.allGems[x - 1, y] != null)
                {
                    if (board.allGems[x - 1, y].type == Gem.GemType.bomb)
                    {
                        MarkTheBombSite(new Vector2Int(x-1,y),board.allGems[x-1,y]);
                    }
                }
            }
            if (gem.posIndex.x < board.horizontal - 1) 
            {
                if (board.allGems[x + 1, y] != null) 
                {
                    if (board.allGems[x + 1, y].type == Gem.GemType.bomb) 
                    {
                        MarkTheBombSite(new Vector2Int(x + 1, y), board.allGems[x + 1, y]);
                    }
                }
            }
            if (gem.posIndex.y > 0)
            {
                if (board.allGems[x , y - 1] != null)
                {
                    if (board.allGems[x, y - 1].type == Gem.GemType.bomb)  
                    {
                        MarkTheBombSite(new Vector2Int(x , y - 1), board.allGems[x , y - 1]);
                    }
                }
            }
            if (gem.posIndex.y < board.vertical - 1)
            {
                if (board.allGems[x, y + 1] != null)
                {
                    if (board.allGems[x , y + 1].type == Gem.GemType.bomb)
                    {
                        MarkTheBombSite(new Vector2Int(x , y + 1), board.allGems[x , y + 1]);
                    }
                }
            }
        }
    }
    void MarkTheBombSite(Vector2Int bombPos, Gem bomb) //Bomba bölgesini iþaretle
    {
        for (int x = bombPos.x - bomb.bombValume; x <= bombPos.x+bomb.bombValume; x++)
        {
            for (int y = bombPos.y - bomb.bombValume; y <= bombPos.y + bomb.bombValume; y++)
            {
                if (x > 0 && x < board.horizontal - 1 && y >= 0 && y < board.vertical - 1)
                {
                    if(board.allGems[x,y]!=null)
                    {
                        board.allGems[x, y].isItMatch = true;
                        FoundGemList.Add(board.allGems[x, y]);
                    }
                }
            }
        }
        if (FoundGemList.Count > 0)
        {
            FoundGemList = FoundGemList.Distinct().ToList();
        }
    }
    
}
