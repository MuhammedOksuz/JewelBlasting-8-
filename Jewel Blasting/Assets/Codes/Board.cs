using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{   //Gem Back
    public GameObject gemBack;
    public int horizontal, vertical;
    //Gems
    public Gem[] gems;
    public Gem[,] allGems;

    public float gemSpeed;
    public GameManager gameManager;

    public enum BoardStatus { waiting, moveing};
    public BoardStatus validStatus = BoardStatus.moveing;

    public Gem bomb;
    public float bombChanse = 2;
    private void Awake()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        allGems = new Gem[horizontal, vertical];
        GemBackEdit();
    }
    private void Update()
    {
        //gameManager.FindMatches();
    }
    void GemBackEdit()
    {
        for (int x = 0; x < horizontal; x++)
        {
            for (int y = 0; y < vertical; y++)
            {
                Vector2 linePos = new Vector2(x, y);
                GameObject gemBackBuilder = Instantiate(gemBack, linePos, Quaternion.identity);
                gemBackBuilder.transform.parent = transform;
                gemBackBuilder.name = "Gem_Back" + x + "," + y;
                int rnd = Random.Range(0, gems.Length);
                int control = 0;
                while (IsThereAMatch(new Vector2Int(x, y), gems[rnd]) && control < 100)
                {
                    rnd = Random.Range(0, gems.Length);
                    control++;
                    //if (control > 0)
                    //{
                    //    print(control);
                    //}
                }
                GemSpawn(new Vector2Int(x, y), gems[rnd]);
                
            }
        }
    }
    void GemSpawn(Vector2Int pos, Gem jevelToBe)
    {
        if (Random.Range(1, 101) < bombChanse)
        {
            jevelToBe = bomb;
        }
        Gem gem = Instantiate(jevelToBe/*oluþacak mücevher*/ , new Vector3 (pos.x,pos.y,0),Quaternion.identity);

        gem.transform.parent = transform;
        gem.name = "gem - " + pos.x + "," + pos.y;
        allGems[pos.x, pos.y]=gem;
        gem.EditGems(pos, this);
    }
    bool IsThereAMatch(Vector2Int posControl, Gem checkedGem)//Eþleþme varmý?
    {

        if (posControl.x > 1)
        {
            if (allGems[posControl.x - 1, posControl.y].type == checkedGem.type&& allGems[posControl.x - 2, posControl.y].type == checkedGem.type)
            {
                return true;
            }

        }
        if (posControl.y > 1)
        {
            if (allGems[posControl.x, posControl.y - 1].type == checkedGem.type && allGems[posControl.x, posControl.y - 2].type == checkedGem.type) 
            {
                return true;
            }

        }


        return false;
    }
    void DestroyMatchingGems(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null)
        {
            if (allGems[pos.x, pos.y].isItMatch)
            {
                if (allGems[pos.x,pos.y].type == Gem.GemType.bomb)
                {
                    SoundsManager.instance.PlaySound(1, Random.Range(0.8f, 1.2f));
                }
                else
                {
                    SoundsManager.instance.PlaySound(0, Random.Range(0.8f, 1.2f));
                }
                Instantiate(allGems[pos.x, pos.y].particalEffect, new Vector3(pos.x, pos.y), Quaternion.identity);
                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }
    }//Eþleþeni yok et
    public void DestroyAllMatchingGems()
    {
        for (int i = 0; i < gameManager.FoundGemList.Count; i++) 
        {
            if (gameManager.FoundGemList[i] != null)
            {
                UIControl.instance.IncreaseTheScore(gameManager.FoundGemList[i].validScore);
                DestroyMatchingGems(gameManager.FoundGemList[i].posIndex);
            }
        }

                StartCoroutine(ScrollDown());
    }//Tüm eþleþenleri yok et
    IEnumerator ScrollDown() //Yok edilenlerin boþluklarýný doldur//alta kaydýr
    {
        yield return new WaitForSeconds(0.2f);
        int emptyCounter = 0;
        for (int x = 0; x < horizontal; x++)
        {
            for (int y = 0; y < vertical; y++)
            {
                if (allGems[x, y] == null)
                {
                    emptyCounter++;
                }
                else if (emptyCounter > 0)
                {
                    allGems[x, y].posIndex.y -= emptyCounter;
                    allGems[x, y - emptyCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }
            emptyCounter = 0;
        }
        StartCoroutine(FillInTheBlanks());
    }
    IEnumerator FillInTheBlanks() //Boþluklarý doldur.
    {
        yield return new WaitForSeconds(.5f);
        FillInTheTopBlanks();

        yield return new WaitForSeconds(.5f);
        gameManager.FindMatches();
        if (gameManager.FoundGemList.Count > 0)
        {
            yield return new WaitForSeconds(1.5f);
            DestroyAllMatchingGems();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            validStatus = BoardStatus.moveing;
        }
    }
    void FillInTheTopBlanks()//Üst boþluklarý doldur.
    {
        for (int x = 0; x < horizontal; x++)
        {
            for (int y = 0; y < vertical; y++)
            {
                if (allGems[x, y] == null)
                {
                    int rnd = Random.Range(0, gems.Length);
                    GemSpawn(new Vector2Int(x, y), gems[rnd]);
                }

            }
        }
        EliminateMisplacements();
    }
    void EliminateMisplacements() //Yanlýþ yerleþmeleri yok et //her hangi bir hataya karþý önllem //üst üste klon olursa yok et
    {
        List<Gem> foundGemList = new List<Gem>();
        foundGemList.AddRange(FindObjectsOfType<Gem>());
        for (int x = 0; x < horizontal; x++)
        {
            for (int y = 0; y < vertical; y++) 
            {
                if (foundGemList.Contains(allGems[x, y]))
                {
                    foundGemList.Remove(allGems[x, y]);
                }
            }
        }
        foreach (Gem gem in foundGemList)
        {
            Destroy(gem.gameObject);
        }
    }
    public void MixBoard()
    {
        if (validStatus != BoardStatus.waiting)
        {
            validStatus = BoardStatus.waiting;
            List<Gem> gemListInTheScene = new List<Gem>();
            for (int x = 0; x < horizontal; x++)
            {
                for (int y = 0; y < vertical; y++)
                {
                    gemListInTheScene.Add(allGems[x, y]);
                    allGems[x, y] = null;
                }
            }
            for (int x = 0; x < horizontal; x++)
            {
                for (int y = 0; y < vertical; y++)
                {
                    int rnd = Random.Range(0, gemListInTheScene.Count);
                    int kontrolCounter = 0;
                    while (IsThereAMatch(new Vector2Int(x, y), gemListInTheScene[rnd]) && kontrolCounter < 100 && gemListInTheScene.Count > 1) 
                    {
                        rnd = Random.Range(0, gemListInTheScene.Count);
                        kontrolCounter++;
                    }

                    gemListInTheScene[rnd].EditGems(new Vector2Int(x, y), this);
                    allGems[x, y] = gemListInTheScene[rnd];
                    gemListInTheScene.RemoveAt(rnd);
                }
            }
            StartCoroutine(ScrollDown());
        }
    }
}
