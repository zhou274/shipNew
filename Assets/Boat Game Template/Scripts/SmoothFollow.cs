using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
    // 跟随目标
    public Transform target;
    // 水平面上与目标保持的距离
    public float distance = 10.0f;
    // 相机相对目标的抬升高度
    public float height = 5.0f;
    // 高度平滑系数
    public float heightDamping = 2.0f;
    // 旋转平滑系数
    public float rotationDamping = 3.0f;

    void LateUpdate()
    {
        if (!target)
            return;

        // 目标当前的朝向与期望高度
        float targetYaw = target.eulerAngles.y;
        float targetHeight = target.position.y + height;

        // 分别对朝向和高度做插值，得到平滑过渡
        float yaw = Mathf.LerpAngle(transform.eulerAngles.y, targetYaw, rotationDamping * Time.deltaTime);
        float y = Mathf.Lerp(transform.position.y, targetHeight, heightDamping * Time.deltaTime);

        // 由平滑后的朝向推算相机位置：站在目标正后方 distance 处
        Quaternion rot = Quaternion.Euler(0, yaw, 0);
        Vector3 pos = target.position - rot * Vector3.forward * distance;
        pos.y = y;
        transform.position = pos;

        // 始终注视目标
        transform.LookAt(target);
    }

}
