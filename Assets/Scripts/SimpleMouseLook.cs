using UnityEngine;

// Look em primeira pessoa: gira o corpo (objeto pai, com o CharacterController
// e o PlayerMovement) no eixo Y (esquerda/direita), e a câmera (este objeto)
// no eixo X (olhar pra cima/baixo). Trava o cursor no centro da tela.
public class SimpleMouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;

    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (transform.parent != null)
        {
            transform.parent.Rotate(Vector3.up * mouseX);
        }

        pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
        transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}
