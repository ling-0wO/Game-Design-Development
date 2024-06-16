using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10.0f; // WASD键移动速度
    public float sensitivity = 1.0f; // 鼠标灵敏度

    private Vector3 currentRotation;
    private bool isCursorLocked = true;
    private bool isRightMouseDown = false;
    private void Start()
    {
        // 在游戏开始时锁定鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void FixedUpdate()
    {        
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
        }
        // 鼠标移动视角
        if (isCursorLocked)
        {
            // WASD键移动
            float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float z = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.Translate(x, 0, z);

            // 空格键和Ctrl键控制y轴的高度
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.Translate(0, -speed * Time.deltaTime, 0);
            }
            if (isRightMouseDown)
            {
                currentRotation.x -= Input.GetAxis("Mouse Y") * sensitivity;
                currentRotation.y += Input.GetAxis("Mouse X") * sensitivity;
                currentRotation.x = Mathf.Clamp(currentRotation.x, -90, 90);
                transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
            }
        }
       

    }
    private void Update()
    {
        // 按下esc键切换鼠标状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            Cursor.visible = !isCursorLocked;
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        
    }
}
