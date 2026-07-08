using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterOrLava : MonoBehaviour
{
    // 可选的水面材质，启动时随机挑一个
    public Material[] waterTypes;

    void Start()
    {
        var rend = transform.GetComponent<Renderer>();
        if (rend && waterTypes.Length > 0)
        {
            int pick = Random.Range(0, waterTypes.Length);
            if (waterTypes[pick])
            {
                rend.material = waterTypes[pick];
            }
        }
    }
}
