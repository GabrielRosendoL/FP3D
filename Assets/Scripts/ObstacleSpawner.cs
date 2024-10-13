using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;   // Prefab dos obstáculos
    public Transform player;            // Referência ao cubo (personagem)
    public float obstacleSpawnDistance = 20f; // Distância entre cada obstáculo gerado
    public float minYPosition = -6f;    // Posição mínima no eixo Y
    public float maxYPosition = 2f;     // Posição máxima no eixo Y
    public int initialObstacles = 10;    // Quantidade de obstáculos gerados previamente
    public float destroyDistanceBehindPlayer = 300f; // Distância atrás do jogador para destruir obstáculos

    private float nextXPosition;        // Próxima posição X para gerar um obstáculo
    private List<GameObject> obstacles = new List<GameObject>();  // Lista para armazenar os obstáculos gerados

    void Start()
    {
        // Definir a posição inicial para o primeiro obstáculo
        nextXPosition = player.position.x + 25f + obstacleSpawnDistance;

        // Gerar os obstáculos iniciais
        for (int i = 0; i < initialObstacles; i++)
        {
            SpawnObstacle();
        }
    }

    void Update()
    {
        // Verifica se o jogador está se aproximando da próxima posição de geração de obstáculo
        if (player.position.x + (obstacleSpawnDistance * 4) > nextXPosition) // Gera com margem de distância
        {
            SpawnObstacle();
        }

        // Verifica e destrói os obstáculos que estão muito atrás do jogador
        DestroyPassedObstacles();
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("Prefab de obstáculo não atribuído no Inspector!");
            return;
        }

        // Gera um obstáculo na próxima posição X com uma posição Y aleatória
        Vector3 spawnPosition = new Vector3(nextXPosition, Random.Range(minYPosition, maxYPosition), 0);
        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

        // Adiciona o obstáculo à lista de obstáculos gerados
        obstacles.Add(newObstacle);

        // Atualiza a próxima posição X para o próximo obstáculo
        nextXPosition += obstacleSpawnDistance;
    }

    void DestroyPassedObstacles()
    {
        // Cria uma lista temporária para armazenar obstáculos que serão destruídos
        List<GameObject> obstaclesToRemove = new List<GameObject>();

        foreach (GameObject obstacle in obstacles)
        {
            // Verifica se o obstáculo está atrás do jogador por uma certa distância
            if (obstacle != null && player.position.x - obstacle.transform.position.x > destroyDistanceBehindPlayer)
            {
                // Destrói o obstáculo e o marca para remoção
                Destroy(obstacle);
                obstaclesToRemove.Add(obstacle);
            }
        }

        // Remove os obstáculos destruídos da lista
        foreach (GameObject obstacle in obstaclesToRemove)
        {
            obstacles.Remove(obstacle);
        }
    }
}
