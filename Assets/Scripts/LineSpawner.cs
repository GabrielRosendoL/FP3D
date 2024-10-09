using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    public GameObject linePrefab;   // Prefab das linhas
    public Transform player;        // Referência ao cubo (personagem)
    public float lineSpawnDistance = 20f; // Distância entre cada linha gerada no eixo X
    public float fixedYPosition = -44f;   // Posição fixa no eixo Y
    public float initialXPosition = 30f;  // Posição X inicial para a primeira linha
    public float destroyDistance = 30f;   // Distância atrás do jogador para destruir linhas

    private float nextXPosition;    // Próxima posição X para gerar uma linha

    void Start()
    {
        // Definir a posição inicial para a primeira linha
        nextXPosition = initialXPosition;
    }

    void Update()
    {
        // Verifica se o jogador está se aproximando da próxima posição de spawn
        if (player.position.x + lineSpawnDistance > nextXPosition)
        {
            SpawnLine();
        }

        // Destruir linhas que o jogador já passou
        DestroyPassedLines();
    }

    void SpawnLine()
    {
        if (linePrefab == null)
        {
            Debug.LogError("Prefab de linha não atribuído no Inspector!");
            return;
        }

        // Gera uma linha na próxima posição X
        Vector3 spawnPosition = new Vector3(nextXPosition, fixedYPosition, 0);
        GameObject newLine = Instantiate(linePrefab, spawnPosition, Quaternion.identity);

        // Atualiza a posição para a próxima linha
        nextXPosition += lineSpawnDistance;
    }

    void DestroyPassedLines()
    {
        // Encontrar todas as linhas na cena
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");

        foreach (GameObject line in lines)
        {
            // Verifica se a linha está atrás do jogador pela distância de destruição
            if (line.transform.position.x < player.position.x - destroyDistance)
            {
                Destroy(line); // Destrói a linha
            }
        }
    }
}
