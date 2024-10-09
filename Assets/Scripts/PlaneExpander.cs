using UnityEngine;

public class PlaneExpander : MonoBehaviour
{
    public Transform player;            // Referência ao cubo que está se movendo
    public float expandThreshold = 5f;  // Distância do cubo em relação ao fim do plano para expandir
    public float expandAmount = 10f;    // Quanto expandir o plano quando o cubo se aproximar do final
    public float destroyThreshold = 10f; // Distância do cubo em relação ao início do plano para destruir a parte de trás

    private Vector3 initialScale;        // Armazena a escala inicial do plano
    private float totalLength;           // Mantém o comprimento total do plano

    void Start()
    {
        // Armazena a escala inicial do plano
        initialScale = transform.localScale;
        totalLength = initialScale.x;  // Definimos o comprimento inicial
    }

    void Update()
    {
        // Verifica se o cubo está se aproximando do final do plano
        if (player.position.x > transform.position.x + (transform.localScale.x * 5f) - expandThreshold)
        {
            ExpandPlane();
        }

        // Verifica se o cubo se afastou o suficiente para destruir a parte de trás do plano
        if (player.position.x > transform.position.x + (destroyThreshold * 2))
        {
            DestroyPlaneBack();
        }
    }

    void ExpandPlane()
    {
        // Aumenta a escala do plano no eixo X para expandir o chão
        transform.localScale = new Vector3(transform.localScale.x + expandAmount, transform.localScale.y, transform.localScale.z);

        // Reposiciona o plano para mantê-lo centralizado conforme ele cresce
        transform.position = new Vector3(transform.position.x + (expandAmount / 2), transform.position.y, transform.position.z);

        // Atualiza o comprimento total do plano
        totalLength += expandAmount;
    }

    void DestroyPlaneBack()
    {
        // Reduz a escala do plano no eixo X, "removendo" a parte de trás
        transform.localScale = new Vector3(transform.localScale.x - expandAmount, transform.localScale.y, transform.localScale.z);

        // Move o plano para a frente para simular que a parte de trás foi removida
        transform.position = new Vector3(transform.position.x + (expandAmount / 2), transform.position.y, transform.position.z);

        // Atualiza o comprimento total do plano
        totalLength -= expandAmount;

        // Evita que o comprimento total fique menor que o comprimento original
        if (totalLength < initialScale.x)
        {
            totalLength = initialScale.x;
        }
    }
}
