using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;  // Referência ao cubo (player)
    public float offsetX = 5f;  // Distância da câmera em relação ao cubo no eixo X

    private Vector3 startPosition;

    void Start()
    {
        // Armazena a posição inicial da câmera para manter o eixo Y e Z fixos
        startPosition = transform.position;
    }

    void Update()
    {
        // Segue o cubo no eixo X, mantendo a posição Y e Z fixas
        transform.position = new Vector3(player.position.x + offsetX, startPosition.y, startPosition.z);
    }
} 
