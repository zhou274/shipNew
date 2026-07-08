using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;
using UnityEngine.Analytics;




using UnityEngine.SocialPlatforms;

public class GameController : MonoBehaviour {
    // 水面颜色
    public Color waterColor = Color.blue;
    // UI 动画控制器
    public Animator uiAnimator;
    // UI 文本元素
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gemText;
    public Text gemReviveText;
    public Image tiltControlIndicator;
    public Image tapControlIndicator;
    public Image soundBTN;
    public Sprite soundON;
    public Sprite soundOFF;
    public Button gemReviveButton;
    public GameObject revivePanel;
    public GameObject overPanel;
    // 当前使用的船只
    [HideInInspector]
    public BoatControllers currentBoat;
    // 相机跟随脚本
    public SmoothFollow cameraScript;
    // 场景滚动脚本
    public ScrollController scrollController;
    // 本局收集的金币
    int coins = 0;
    float lastTime = 0.0f;
    // 本局已复活次数
    int timesSaved = 0;
    // 全部船只
    public GameObject[] allBoats;
    public string clickid;

    public GameObject GameUI;

    public GameObject PopPanel;

    // 启动时激活上次选中的船只
    void Start () {
        int selectedBoat = PlayerPrefs.GetInt("SelectedBoat", 0);
        if (allBoats[selectedBoat]) {
            allBoats[selectedBoat].SetActive(true);
        }

        SetControlVisual();
        SwitchAudioVisual();
        GameUI.SetActive(false);
    }
    // 弹出提示面板
	public void ShowPopPanel()
    {
        PopPanel.SetActive(true);
    }
    // 关闭提示面板并开始游戏
    public void HidePopPanel()
    {
        PopPanel.SetActive(false);
        StartTheGame();
    }
    //Setup end panel values
    public void EnableDisableEndPanel(string state , float score , int coins){

        if (!scoreText || !bestText || !coinText || !gemText || !revivePanel || !overPanel)
        {
            Debug.LogWarning("Please assign all the variables");
            return;
        }

        if (!uiAnimator)
        {
            Debug.LogWarning("Some variables are not assigned");
            return;
        }

        if (gemReviveText && timesSaved < 3) {
            timesSaved++;
            if (PlayerPrefs.GetInt("Gems", 0) >= (timesSaved * 3)) {
                gemReviveText.text = "Required Gems : " + (timesSaved * 3).ToString() + "\n You have : " + PlayerPrefs.GetInt("Gems", 0).ToString() + " Gems";
                state = "Revive";
            }

            if (timesSaved > 1) {
                if (gemReviveButton) {
                    gemReviveButton.interactable = false;
                }
            }

            
        }

        if (!uiAnimator.enabled)
        {
            uiAnimator.enabled = true;
            return;
        }
        else {
            uiAnimator.SetTrigger("Over");
            overPanel.SetActive(true);
            GameUI.SetActive(false);
            AddCoins();
        }



        if (score > PlayerPrefs.GetFloat("Best", 0))
        {
            PlayerPrefs.SetFloat("Best", score);
        }


        bestText.text = "��߷� : " + PlayerPrefs.GetFloat("Best", 0).ToString("F0");
        scoreText.text = "�÷� : " + score.ToString("F0");
        coinText.text = "X " + PlayerPrefs.GetInt("Coins", 0);
        gemText.text = PlayerPrefs.GetInt("Gems", 0) + " X";

    }
    // 记录本局金币数
    public void SetCoinValue(int currentCoins) {
        coins = currentCoins;
    }
    // 结算时把本局金币写入存档
    public void AddCoins() {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + coins);
    }
    // 直接奖励 100 金币
    public void AddCoinByAD()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 100);
        
    }
    // 奖励 100 金币并关闭提示面板
    public void AddCoinByAD2()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 100);
        HidePopPanel();

    }
    // 暂停
    public void Pause() {
        lastTime = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    //UnPause
    public void UnPause() {
        Time.timeScale = lastTime;
    }
    // 消耗宝石复活
    public void ReviveWithGems() {

        uiAnimator.SetTrigger("Reset");
        currentBoat.Revive();
        
    }
    public void AddCoinsONE()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 100);
        
    }
    // 免费复活：直接重置并让船体复活
    public void ReviveWithVideo() {
        if (uiAnimator)
        {
            uiAnimator.SetTrigger("Reset");
        }

        if (currentBoat)
        {
            currentBoat.Revive();
        }
    }

    // 重开当前关卡
    public void Restart()
    {
        Initiate.Fade(SceneManager.GetActiveScene().name,waterColor,2.0f);
    }
    // 刷新操控方式按钮的高亮状态
    void SetControlVisual() {
        if (!tiltControlIndicator || !tapControlIndicator) {
            Debug.LogWarning("Please Assign all the variables");
            return;
        }

        switch (PlayerPrefs.GetInt("ControlType", 0))
        {

            case 0:
                tiltControlIndicator.color = Color.green;
                tapControlIndicator.color = Color.black;

                break;
            case 1:
                tiltControlIndicator.color = Color.black;
                tapControlIndicator.color = Color.green;
                break;
        }
    }
    // 切换操控方式并保存
    public void ChangeControlType(int type) {
        switch (type)
        {
            case 0:
                PlayerPrefs.SetInt("ControlType", 1);
                break;
            case 1:
                PlayerPrefs.SetInt("ControlType", 0);
                break;
        }
        SetControlVisual();
    }
    public void Update()
    {
        
        coinText.text = "X " + PlayerPrefs.GetInt("Coins", 0);
        
    }
    // 切换音效开关
    public void SwitchAudio() {
        if (PlayerPrefs.GetInt("SFX", 1) == 1) {
            PlayerPrefs.SetInt("SFX", 0);
        } else if (PlayerPrefs.GetInt("SFX", 1) == 0) {
            PlayerPrefs.SetInt("SFX", 1);
        }

        SwitchAudioVisual();
    }

    // 根据音效开关刷新按钮图标与音量
    public void SwitchAudioVisual() {
        if (!soundBTN || !soundOFF || !soundON)
        {
            Debug.LogWarning("Please Assign all the variables");
            return;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 1)
        {
            soundBTN.sprite = soundON;
            AudioListener.volume = 1.0f;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 0)
        {
            soundBTN.sprite = soundOFF;
            AudioListener.volume = 0.0f;
        }
    }

    // 开始游戏
    public void StartTheGame() {
        if (currentBoat) {
            currentBoat.StartTheGame();
            if (cameraScript) {
                cameraScript.target = currentBoat.transform;
            }
        }
        if (scrollController)
        {
            scrollController.StartSetup();
        }
        GameUI.SetActive(true);
    }

    // 进入商店场景
    public void Buy() {
        Initiate.Fade("Buy", Color.white, 2.0f);
    }

    public void Leaderboard() {
        // 通过 (int)PlayerPrefs.GetFloat("Best", 0) 取得历史最高分
        Debug.Log("Leaderboard : Put your code here , Double click to open in IDE");
    }
    // 评分按钮
    public void Rate()
    {
        //Application.OpenURL("Put your URL here then uncomment it");

        Debug.Log("Rate : Put your code here , Double click to open in IDE");
    }
    
    
}
