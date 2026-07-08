using UnityEngine;
using System.Collections;

public class ChallengeInfo : MonoBehaviour {
    // 道具生成点
    public Transform powerUpPoint;
    // 可随机生成的道具列表
    public GameObject[] allPowerUps;

    // 满足生成间隔时随机刷一个道具
    void Start() {
        if (!powerUpPoint) {
            return;
        }

        if (PowerUpSpawnGate.CanSpawn()) {
            int idx = Random.Range(0, allPowerUps.Length);
            if (allPowerUps[idx]) {
                GameObject inst = Instantiate(allPowerUps[idx], powerUpPoint.position, Quaternion.identity) as GameObject;
                inst.transform.parent = transform;
            }
        }
    }
    // 在编辑器里标出生成点
    void OnDrawGizmos()
    {
        if (powerUpPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(powerUpPoint.position, 0.5f);
        }
    }
}
