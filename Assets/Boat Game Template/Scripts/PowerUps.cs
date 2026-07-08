using UnityEngine;
using System.Collections;

public class PowerUps : MonoBehaviour {
    // 拾取时的爆裂特效
    public GameObject burstEffects;
    // 拾取后是否销毁自身
    public bool destroyMe = true;
    // 磁铁吸附时朝向的目标
    Transform target;

    // 道具类型
    public enum powerUpTypes {
        Shield, Gasoline, Coin, Magnet,Gem
    };

    public powerUpTypes powerUpType;

    // 根据道具类型向船体派发对应效果
    public void InitializePowerUp(Transform other) {
        switch (powerUpType) {
            case powerUpTypes.Shield: other.SendMessage("EnableShield", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Gasoline: other.SendMessage("FillUpGas", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Coin: other.SendMessage("AddCoin", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Magnet: other.SendMessage("EnableMagnet", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Gem: other.SendMessage("AddGem", SendMessageOptions.RequireReceiver); break;
        }
        if (burstEffects) {
            GameObject instBurst = Instantiate(burstEffects, transform.position, Quaternion.identity) as GameObject;
            instBurst.transform.parent = transform.parent;
        }
        if (destroyMe)
            Destroy(gameObject);
    }

    // 金币被磁铁吸引时持续飞向目标
    IEnumerator MoveCoin()
    {
        while (true) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z), Time.deltaTime * 10.0f);
            yield return null;
        }
    }

    public void SetCoinMagnet(Transform other) {
        if (target || powerUpType !=  powerUpTypes.Coin) {
            return;
        }
        target = other;
        StartCoroutine(MoveCoin());
    }
}
