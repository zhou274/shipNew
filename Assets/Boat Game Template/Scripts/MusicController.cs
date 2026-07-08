using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicController : MonoBehaviour {
    AudioSource myMusic;
    bool started = false;

    void Start () {
        myMusic = transform.GetComponent<AudioSource>();
        // 跨场景保留背景音乐
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        // 进入 Game 场景后播放一次
        if (myMusic && SceneManager.GetActiveScene().name == "Game" && !started)
        {
            myMusic.Play();
            started = true;
        }
    }
}
