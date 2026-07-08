using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public class PopLink : MonoBehaviour
{
    // WebGL 平台下用外链跳转，可在 Inspector 中自行配置目标地址
    [SerializeField] private string homeUrl = "";
    [SerializeField] private string moreUrl = "";

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void Website() {
#if !UNITY_EDITOR
        if (!string.IsNullOrEmpty(homeUrl))
            openWindow(homeUrl);
#endif
    }

    public void Assetstore() {
#if !UNITY_EDITOR
        if (!string.IsNullOrEmpty(moreUrl))
            openWindow(moreUrl);
#endif
    }

    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
