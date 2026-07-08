using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour {
    [HideInInspector]
	public bool start = false;
    [HideInInspector]
    public float fadeDamp = 0.0f;
    [HideInInspector]
    public string fadeScene;
    [HideInInspector]
    public float alpha = 0.0f;
    [HideInInspector]
    public Color fadeColor;
    [HideInInspector]
    public bool isFadeIn = false;

    //Set callback
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    //Remove callback
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    // 用一张纯色贴图铺满屏幕，通过 alpha 做整屏渐变
    void OnGUI () {
        if (!start)
            return;

        // 把当前 alpha 应用到绘制颜色上
        GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);

        // 生成 1x1 的纯色贴图并拉伸铺满屏幕
        Texture2D tex = new Texture2D (1, 1);
        tex.SetPixel (0, 0, fadeColor);
        tex.Apply ();
        GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), tex);

        // 淡入朝 0 逼近，淡出朝 1 逼近
        if (isFadeIn)
            alpha = Mathf.Lerp (alpha, -0.1f, fadeDamp * Time.deltaTime);
        else
            alpha = Mathf.Lerp (alpha, 1.1f, fadeDamp * Time.deltaTime);

        // 全黑后切场景，完全淡入后销毁自身
        if (alpha >= 1 && !isFadeIn) {
            SceneManager.LoadScene(fadeScene);
            DontDestroyOnLoad(gameObject);
        } else
        if (alpha <= 0 && isFadeIn) {
            Destroy(gameObject);
        }

    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // 新场景加载完成，开始淡入
        isFadeIn = true;
    }

}
