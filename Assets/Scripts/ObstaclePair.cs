using UnityEngine;

public class ObstaclePair : MonoBehaviour
{
    public GameObject scoreTrigger;  // Referência ao trigger de pontuação
    private bool pointCounted = false;  // Flag para garantir que o ponto seja contado apenas uma vez

    void Start()
    {
        // Criar o trigger manualmente
        if (scoreTrigger == null)
        {
            scoreTrigger = new GameObject("ScoreTrigger");
            scoreTrigger.transform.SetParent(transform);
            scoreTrigger.transform.localPosition = Vector3.zero;  // Centraliza o trigger entre os obstáculos

            // Adicionar o BoxCollider para atuar como trigger
            BoxCollider triggerCollider = scoreTrigger.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;

            // Ajustar o tamanho do trigger para cobrir toda a altura onde o cubo pode passar
            triggerCollider.size = new Vector3(1f, 30f, 1f);  // Aumentar a altura para garantir que o cubo sempre passe por ele

            // Tornar o trigger visível para debug (pode ser removido após os testes)
            MeshRenderer renderer = scoreTrigger.AddComponent<MeshRenderer>();
            renderer.material.color = Color.red;  // Cor vermelha para visualização

            // Adicionar forma visual (opcional)
            MeshFilter meshFilter = scoreTrigger.AddComponent<MeshFilter>();
            meshFilter.mesh = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshFilter>().mesh;  // Usar um cubo como forma
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o cubo passou e se ainda não contou o ponto
        if (other.CompareTag("Player") && !pointCounted)
        {
            CubeController cubeController = other.GetComponent<CubeController>();
            if (cubeController != null)
            {
                cubeController.AddScore();  // Adiciona um ponto
                pointCounted = true;  // Evita contagem duplicada
            }
        }
    }
}
