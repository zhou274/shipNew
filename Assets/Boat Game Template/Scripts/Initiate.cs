using UnityEngine;
using System.Collections;

public static class Initiate {
    // 动态生成一个承载淡入淡出的对象，并把切场景参数交给它
    public static void Fade (string scene, Color col, float damp){
        GameObject holder = new GameObject("Fader");
        Fader fader = holder.AddComponent<Fader>();
        fader.fadeDamp = damp;
        fader.fadeScene = scene;
        fader.fadeColor = col;
        fader.start = true;
    }
}
