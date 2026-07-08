using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BoatBuyingSelection : MonoBehaviour {
    // 所有可选船只
    public GameObject[] allBoats;
    // 每艘船对应的购买信息
    BoatBuyingInfo[] allBoatInfos;
    // 各船只初始位置缓存
    Vector3[] boatPositions;
    // 用于移动到选中船只的展示相机
    public Transform viewCamera;
    // 水波高度
    public float waveHeight = 5.0f;
    // 水波频率
    public float waveFrequency = 2.0f;
    // 遍历计数器
    int i = 0;
    // 当前选中的船只索引
    public int currentSelected = 0;
    // 锁定状态提示框
    public GameObject isLockedBox;
    // 购买按钮框
    public GameObject isBuyBox;
    // 已选中提示框
    public GameObject isSelectedBox;
    // 价格文本
    public TextMeshProUGUI buyValue;
    // 金币数量文本
    public TextMeshProUGUI coinCount;
    // 宝石数量文本
    public TextMeshProUGUI gemCount;

    void Start () {
        // 读取上次选中的船只
        currentSelected = PlayerPrefs.GetInt("SelectedBoat", 0);

        boatPositions = new Vector3[allBoats.Length];
        allBoatInfos = new BoatBuyingInfo[allBoats.Length];
        // 缓存各船只的位置与购买信息
        for (int i = 0; i < allBoats.Length; i++) {
            if (allBoats[i]) {
                boatPositions[i] = allBoats[i].transform.localPosition ;
                allBoatInfos[i] = allBoats[i].GetComponent<BoatBuyingInfo>(); 
            }
        }
	}

    void Update()
    {
        // 刷新金币与宝石数量
        if (gemCount && coinCount) {
            gemCount.text = PlayerPrefs.GetInt("Gems", 0) + " X";
            coinCount.text =  PlayerPrefs.GetInt("Coins", 0) + " X";
        }

        // 让选中船只浮动并高亮，其余置灰
        i = 0;
        Renderer[] allRenderers;
        foreach (GameObject boat in allBoats)
        {
            if (boat)
            {
                boat.transform.localPosition = new Vector3(boat.transform.localPosition.x, boatPositions[i].y + Mathf.Sin(Time.time * waveFrequency) * waveHeight, boat.transform.localPosition.z);
                allRenderers = allBoats[i].GetComponentsInChildren<Renderer>();
                if (i == currentSelected && viewCamera)
                {
                    viewCamera.transform.position = new Vector3(viewCamera.transform.position.x, viewCamera.transform.position.y, Mathf.Lerp(viewCamera.transform.position.z, allBoats[i].transform.position.z, Time.deltaTime * 5.0f));
                    foreach (Renderer currentRenderer in allRenderers)
                    {
                        //Debug.Log(currentRenderer.gameObject.name);
                        currentRenderer.material.SetColor("_tint",Color.Lerp(currentRenderer.material.GetColor("_tint"), Color.white, Time.deltaTime * 5.0f));
                    }
                }
                else {
                    foreach (Renderer currentRenderer in allRenderers)
                    {
                        currentRenderer.material.SetColor ("_tint" , Color.Lerp(currentRenderer.material.GetColor("_tint"), Color.black, Time.deltaTime * 5.0f));
                    }
                }

                if (allBoatInfos[i] && isLockedBox && isBuyBox && buyValue && isSelectedBox && i == currentSelected)
                {

                    if (!allBoatInfos[i].isLocked)
                    {
                        isLockedBox.SetActive(false);

                        isBuyBox.SetActive(false);

                        isSelectedBox.SetActive(true);
                        
                        if (PlayerPrefs.GetInt("SelectedBoat", 0) != currentSelected)
                            PlayerPrefs.SetInt("SelectedBoat", currentSelected);
                    }
                    else {
                        
                        isLockedBox.SetActive(true);
                        
                        if (allBoatInfos[i].canBuy)
                        {
                            isBuyBox.SetActive(true);
                        }
                        if (buyValue)
                            buyValue.text = "X " + allBoatInfos[i].cost.ToString();

                        isSelectedBox.SetActive(false);

                    }
                }
            }

            i++;

        }
    }

    // 向左切换选择
    public void SelectOnLeft() {
        currentSelected--;
        if (currentSelected < 0) {
            currentSelected = allBoats.Length - 1;
        }

        
    }

    // 向右切换选择
    public void SelectOnRight()
    {
        currentSelected++;
        if (currentSelected > allBoats.Length-1) {
            currentSelected = 0;
        }

        
    }
    // 购买当前选中的船
    public void BuyCall() {
        if (allBoatInfos[currentSelected])
            allBoatInfos[currentSelected].BuyTheBoat();
    }
    // 返回游戏场景
    public void Back() {
        Initiate.Fade("Game", Color.white, 2.0f);
    }
}
