using UnityEngine;

public class Laser : MonoBehaviour
{
    public float duration = 2.0f; // 射线存在的时间

    void Start()
    {
        // 在一段时间后销毁射线
        Destroy(gameObject, duration);
    }
}
