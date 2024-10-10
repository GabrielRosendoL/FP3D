using UnityEngine;
using System.Collections.Generic;  // Necessário para usar Queue

public class BackgroundExpander : MonoBehaviour
{
    public GameObject wallPrefab;      // Prefab do wall (parede de fundo)
    public Transform player;           // Referência ao cubo que está se movendo
    public float expandThreshold = 100f;  // Distância para gerar novas paredes
    public float destroyThreshold = 300f; // Distância para destruir paredes antigas
    private float nextSpawnPositionX;   // Posição X onde a próxima wall será gerada à frente
    private float previousSpawnPositionX; // Posição X onde a próxima wall será gerada atrás
    private Queue<GameObject> spawnedWalls = new Queue<GameObject>();  // Fila para gerenciar as walls geradas

    void Start()
    {
        // Inicializa as posições iniciais de walls à frente e atrás do jogador
        nextSpawnPositionX = 54.24f;  // Posição inicial para a primeira wall
        previousSpawnPositionX = 54.24f;  // Posição inicial para a primeira wall atrás do jogador

        // Gera múltiplas walls à frente do jogador no início
        for (int i = 0; i < 5; i++)  // Ajuste o valor para gerar mais ou menos walls inicialmente
        {
            ExpandBackgroundForward();
        }

        // Gera múltiplas walls atrás do jogador no início
        for (int i = 0; i < 5; i++)  // Ajuste o valor para gerar mais ou menos walls atrás
        {
            ExpandBackgroundBackward();
        }
    }

    void Update()
    {
        // Verifica se o cubo está se aproximando da próxima wall à frente
        if (player.position.x > nextSpawnPositionX - expandThreshold)
        {
            ExpandBackgroundForward();
        }

        // Verifica se o cubo se afastou o suficiente para destruir as walls antigas
        if (spawnedWalls.Count > 0 && player.position.x > spawnedWalls.Peek().transform.position.x + destroyThreshold)
        {
            DestroyOldWall();
        }
    }

    void ExpandBackgroundForward()
    {
        // Instancia um novo wall à frente do último gerado
        GameObject newWall = Instantiate(wallPrefab, new Vector3(nextSpawnPositionX, -28.44f, 30), Quaternion.identity);

        // Adiciona o wall à fila para controlar quais walls já foram geradas
        spawnedWalls.Enqueue(newWall);

        // Calcula o tamanho real do wall em unidades do mundo, para espaçamento correto
        float wallRealLength = wallPrefab.GetComponent<Renderer>().bounds.size.x;

        // Atualiza a posição X para o próximo wall ser gerado à frente
        nextSpawnPositionX += wallRealLength;
    }

    void ExpandBackgroundBackward()
    {
        // Instancia um novo wall atrás do último gerado
        GameObject newWall = Instantiate(wallPrefab, new Vector3(previousSpawnPositionX, -28.44f, 30), Quaternion.identity);

        // Adiciona o wall à fila para controlar quais walls já foram geradas
        spawnedWalls.Enqueue(newWall);

        // Calcula o tamanho real do wall em unidades do mundo, para espaçamento correto
        float wallRealLength = wallPrefab.GetComponent<Renderer>().bounds.size.x;

        // Atualiza a posição X para o próximo wall ser gerado atrás
        previousSpawnPositionX -= wallRealLength; // Subtrai para gerar para trás
    }

    void DestroyOldWall()
    {
        // Remove o wall mais antigo da fila e o destrói
        GameObject oldWall = spawnedWalls.Dequeue();
        Destroy(oldWall);
    }
}
