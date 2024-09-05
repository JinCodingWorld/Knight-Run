using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;
    //지금은 CONGRATS
    public GameObject FinalScore;
    AudioSource audioSound;

    private void Awake()
    {
        audioSound = GetComponent<AudioSource>();

        if(instance == null )
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 두 개 이상의 게임 매니저가 존재합니다.");
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        // Change Stage
        if(stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
            audioSound.Play();
        }
        else
        {
            //Game Clear
            //Player Control Lock
            Time.timeScale = 0;
            //Result UI

            //Restart Button UI
            UIRestartBtn.SetActive(true);
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";

            //score board
            FinalScore.SetActive(true);
            //Text fscore = FinalScore.GetComponentInChildren<Text>();
            //fscore.text = "Final Socre : " + totalPoint;

            //UIRestartBtn.SetActive(true);
        }

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //All Health UI off
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            //Player Die Effect
            player.OnDie();
            //Result UI
            Debug.Log("죽었습니다.");
            //Retry Button UI
            UIRestartBtn.SetActive(true);
            //실험용
            audioSound.Stop();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            HealthDown();

            //player Reposition 
            if (health > 1)
            {
                PlayerReposition();
            }

            //Health Down
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }


}
