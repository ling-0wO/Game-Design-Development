using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10.0f; // WASD���ƶ��ٶ�
    public float sensitivity = 1.0f; // ���������

    private Vector3 currentRotation;
    private bool isCursorLocked = true;
    private bool isRightMouseDown = false;
    private void Start()
    {
        // ����Ϸ��ʼʱ�������
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
        // ����ƶ��ӽ�
        if (isCursorLocked)
        {
            // WASD���ƶ�
            float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float z = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.Translate(x, 0, z);

            // �ո����Ctrl������y��ĸ߶�
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
        // ����esc���л����״̬
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            Cursor.visible = !isCursorLocked;
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        
    }
}
