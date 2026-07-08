using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LogoScene : MonoBehaviour {
    // 停留多久后进入游戏
    public float timeToLoad = 1.5f;

    void Start () {
        Application.targetFrameRate = 60;
        Invoke("LoadGame", timeToLoad);
    }
    // 淡出并加载 Game 场景
    void LoadGame() {
        Initiate.Fade("Game", Color.black, 2.0f);
    }
}
