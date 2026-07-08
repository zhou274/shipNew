using UnityEngine;
using System.Collections;

public class BoatBuyingInfo : MonoBehaviour {
    // 船只编号
    public int boatNumber = 0;
    // 解锁所需金币
    public int cost = 5000;
    // 是否处于锁定状态
    public bool isLocked = true;
    // 当前金币是否足够购买
    [HideInInspector]
    public bool canBuy = false;

    void Start () {
        AssesInfo();
    }
    // 根据存档刷新船只的锁定/可购买状态
    public void AssesInfo() {
        isLocked = true;

        if (PlayerPrefs.GetInt("UnlockedBoat" + boatNumber.ToString(), 0) == 1)
        {
            isLocked = false;
        }


        if (boatNumber == 0)
        {
            isLocked = false;
        }

        canBuy = false;

        if (isLocked && PlayerPrefs.GetInt("Coins", 0) >= cost)
        {
            canBuy = true;
        }

    }
    // 扣金币并写入解锁记录
    public void BuyTheBoat() {
        if (isLocked && canBuy) {
            PlayerPrefs.SetInt("Coins" , PlayerPrefs.GetInt("Coins", 0) - cost);
            PlayerPrefs.SetInt("UnlockedBoat" + boatNumber.ToString(), 1);

        }
        AssesInfo();
    }
}
