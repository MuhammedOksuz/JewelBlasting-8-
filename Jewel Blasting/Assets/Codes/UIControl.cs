using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public static UIControl instance;
    Board board;

    [SerializeField] TMP_Text ramainderTimeText;//kalan zaman
    [SerializeField] TMP_Text scorText;
    public int ramainderTime = 60;
    public int validScor;
    [SerializeField] GameObject levelComplatePanel;
    public bool isItLevelComplate;

    [SerializeField] GameObject pausePanel;
    private void Awake()
    {
        instance = this;
        board = Object.FindObjectOfType<Board>();
    }
    private void Start()
    {
        StartCoroutine(CountDown());
        isItLevelComplate = false;
    }
    IEnumerator CountDown()//geri say
    {
        while (ramainderTime > 0)
        {
            yield return new WaitForSeconds(1);
            ramainderTime--;
            ramainderTimeText.text = ramainderTime + " sn.";
            if (ramainderTime <= 0)
            {
                isItLevelComplate = true;
                levelComplatePanel.SetActive(true);
                SoundsManager.instance.PlaySound(2, Random.Range(0.8f, 1.2f));
            }
        }
    }
    public void IncreaseTheScore(int scoreFromOutside)//puaný artýr
    {
        validScor += scoreFromOutside;
        scorText.text = validScor.ToString() + "";
    }
    public void Mix()
    {
        board.MixBoard();
    }
    public void Pause()
    {
        if(!pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
}
