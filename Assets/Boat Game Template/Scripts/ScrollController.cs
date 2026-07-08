using UnityEngine;
using System.Collections;
using System.Drawing;

// 控制道具刷新频率：每隔若干次才允许生成一次
public static class PowerUpSpawnGate{
    static int powerUpCounter = 0;
    public static int powerUpSpawnIn = 5;

    public static bool CanSpawn() {
        powerUpCounter++;
        if (powerUpCounter >= powerUpSpawnIn)
        {
            powerUpCounter = 0;
            return true;
        }
        else {
            return false;
        }
    }

}

public class ScrollController : MonoBehaviour {
    // 最低速度
    public float minSpeed = 8.0f;
    public float speed = 0.0f;
    // 最高速度
    public float maxSpeed = 18.0f;
    // 当前累计速度
    [HideInInspector]
    public float holdUpSpeed = 0.0f;
    // 需要滚动的所有地块
    public PlatformController[] panels;
    // 队尾地块
    public PlatformController lastPanel;
    // 所有障碍物预制体
    public GameObject[] challenges;
    // 数值越大道具越稀有
    public int powerUpSpawnIn = 5;

    // 水面材质引用
    public Renderer water;
    // 水流速度系数
    public float waterSpeedFactor;
    // 默认水流速度
    float defaultWaterSpeed;


    void Start () {
        if (water){
            defaultWaterSpeed = water.material.GetFloat("_flowSpeed");
        }

        holdUpSpeed = speed;

        PowerUpSpawnGate.powerUpSpawnIn = powerUpSpawnIn;

        foreach (PlatformController panel in panels)
        {
            if (panel)
            {
                panel.AssignChallengesArray(challenges);
                panel.CreateChallenge();
            }
        }

        speed = 0.0f;

    }
    // 滚动地块并在到头时循环回队尾、重刷障碍物
    void Update () {

        if (!lastPanel) {
            
            return;
        }

        if (water){
            water.material.SetFloat("_flowSpeed" , waterSpeedFactor);
        }

        foreach (PlatformController panel in panels) {
            if (panel) {

                panel.transform.position -= Vector3.forward * (Time.deltaTime * speed);

                if (panel.GetEndPoint().z <= -15.0f) {
                    panel.transform.position = lastPanel.GetEndPoint();
                    panel.CreateChallenge();
                    lastPanel = panel;
                }
                
            }
        }

    }
    // 设置起始速度
    public void StartSetup() {
        speed = minSpeed;
    }
    // 撞船后恢复默认水流速度
    public void Crashed (){
        if (water){
            water.material.SetFloat("_flowSpeed" , defaultWaterSpeed);
        }
    }
}
