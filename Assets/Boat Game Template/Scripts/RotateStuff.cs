using UnityEngine;
using System.Collections;

public class RotateStuff : MonoBehaviour {
    public Vector3 rotationVector;

    void OnEnable () {
        StartCoroutine(RotateMe());
    }
    // 只要对象存活就持续自转
    IEnumerator RotateMe() {
        while (true) {
            transform.Rotate(rotationVector);
            yield return null;
        }
    }
}
