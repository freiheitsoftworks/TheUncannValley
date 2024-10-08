using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;           // Velocidade máxima ao caminhar
    public float gravity = -9.81f;         // Gravidade aplicada
    public float sensitivity = 100f;       // Sensibilidade do mouse
    public float rotationSpeed = 10f;      // Velocidade de rotação do player

    private CharacterController characterController;  // Componente CharacterController
    private Transform cameraTransform;                // Referência à Transform da câmera
    private Transform cameraHolder;                   // Holder da câmera para controlar a rotação vertical
    private float verticalSpeed = 0f;                 // Controle de velocidade vertical
    private Transform feet;                           // Posição dos pés
    private LayerMask floor;                          // Máscara do chão para detecção de colisão
    private bool isGrounded;                          // Verifica se o player está no chão
    private float xRotation = 0f;                     // Controle da rotação vertical da câmera

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        cameraHolder = cameraTransform.parent;  // Pegando o CameraHolder que controla a rotação vertical
        feet = transform.Find("Feet").transform;  // Ajuste conforme o nome do transform no seu objeto
        floor = LayerMask.GetMask("Floor");
        Cursor.lockState = CursorLockMode.Locked; // Esconde e trava o cursor
    }

    void Update()
    {
        RotateCamera();
        Movement();
    }

    // Controle da rotação da câmera (vertical) e do jogador (horizontal)
    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotaciona o jogador no eixo Y (olhar para os lados)
        transform.Rotate(Vector3.up * mouseX);

        // Controla a rotação vertical (para cima e para baixo) usando o cameraHolder
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limita a rotação vertical

        // Aplica a rotação vertical ao CameraHolder
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Controle de movimentação do player
    private void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A e D
        float vertical = Input.GetAxis("Vertical");     // W e S

        // Cria vetores de direção para os movimentos relativos à câmera
        Vector3 forward = cameraTransform.forward;  // Direção da frente da câmera
        Vector3 right = cameraTransform.right;      // Direção da direita da câmera

        // Garantir que o movimento seja no plano horizontal (ignorar eixo Y)
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Movimentação do jogador (W/S movem para frente/trás, A/D movem para esquerda/direita)
        Vector3 movement = (forward * vertical + right * horizontal).normalized;

        // Movimenta o player usando o CharacterController
        characterController.Move(movement * moveSpeed * Time.deltaTime);

        // Gravidade e pulo
        isGrounded = Physics.CheckSphere(feet.position, 0.3f, floor);

        if (isGrounded && verticalSpeed < 0)
        {
            verticalSpeed = 0f;  // Reseta a velocidade vertical ao tocar o chão
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalSpeed = 5f;  // Velocidade do pulo
        }

        // Aplica a gravidade continuamente
        verticalSpeed += gravity * Time.deltaTime;

        // Movimenta o player verticalmente (gravidade/pulo)
        characterController.Move(new Vector3(0, verticalSpeed, 0) * Time.deltaTime);
    }
}
