using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {
    // 该地块的结束点位置
    public Transform endPoint;
    // 障碍物的生成点集合
    public Transform[] challengeSpawnPoints;
    // 障碍物挂载的父节点
    public GameObject challengeHolder;
    // 首次循环是否跳过生成
    public bool skipFirstLoop = false;
    // 可选障碍物预制体
    GameObject[] challengesArray;

    // 在编辑器里可视化各个关键点
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
        Gizmos.color = Color.red;
        if (endPoint) {
            Gizmos.DrawWireSphere(endPoint.position, 1.0f);
        }
        Gizmos.color = Color.blue;

        for (int i = 0; i < challengeSpawnPoints.Length; i++)
        {
            if (challengeSpawnPoints[i] != null)
            {
                Gizmos.DrawWireSphere(challengeSpawnPoints[i].position, 0.5f);
            }
        }

        Gizmos.color = Color.cyan;

    }
    // 注入可用障碍物列表
    public void AssignChallengesArray(GameObject[] arr) {
        challengesArray = arr;
    }
    // 返回结束点坐标
    public Vector3 GetEndPoint() {
        if (endPoint)
            return endPoint.position;
        else
            return Vector3.zero;
    }

    // 清空旧障碍物并在各生成点随机重刷
    public void CreateChallenge() {
        if (skipFirstLoop) {
            skipFirstLoop = false;
            return;
        }

        if (!challengeHolder) {
            Debug.LogWarning("Please Assign challenge holder variable");
            return;
        }
        // 先销毁已有障碍物
        for (int i = 0; i < challengeHolder.transform.childCount; i++){
            Destroy(challengeHolder.transform.GetChild(i).gameObject);
        }
        GameObject instChallenge;
        for (int i = 0; i < challengeSpawnPoints.Length; i++){
            int pick = Random.Range(0, challengesArray.Length);
            if (challengesArray[pick] != null)
            {
                instChallenge = Instantiate(challengesArray[pick], challengeSpawnPoints[i].position, Quaternion.identity) as GameObject;
                instChallenge.transform.parent = challengeHolder.transform;
            }
        }
    }


}
