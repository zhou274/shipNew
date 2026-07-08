using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 粒子播放结束后自动销毁所在对象
public class ParticleKiller : MonoBehaviour {
    ParticleSystem ps;

    void Start () {
        ps = transform.GetComponent<ParticleSystem>();
        if (ps)
            StartCoroutine(Kill());
    }
    // 轮询粒子是否还存活，播完即销毁
    IEnumerator Kill() {
        while (ps) {
            if (!ps.IsAlive())
                Destroy(gameObject);
            yield return null;
        }
        yield return null;
    }
}
