using UnityEngine;
using System.Collections.Generic;  // Necessário para usar Queue

public class RoofExpander : MonoBehaviour
{
    public GameObject roofPrefab;      // Prefab do chão (roof)
    public Transform player;            // Referência ao cubo que está se movendo
    public float expandThreshold = 100f;  // Aumenta o valor para gerar o chão antes
    public float destroyThreshold = 300f; // Distância do cubo em relação ao início do plano para destruir a parte de trás
    private float nextSpawnPositionX;   // Posição X onde o próximo tile será gerado à frente
    private float previousSpawnPositionX; // Posição X onde o próximo tile será gerado atrás
    private Queue<GameObject> spawnedTiles = new Queue<GameObject>();  // Fila para gerenciar os tiles gerados

    void Start()
    {
        // Inicializa as posições iniciais de tiles à frente e atrás do jogador
        nextSpawnPositionX = transform.position.x;
        previousSpawnPositionX = transform.position.x;

        // Gera múltiplos tiles à frente do jogador no início
        for (int i = 0; i < 5; i++)  // Ajuste o valor para gerar mais ou menos tiles inicialmente
        {
            ExpandRoofForward();
        }

        // Gera múltiplos tiles atrás do jogador no início
        for (int i = 0; i < 5; i++)  // Ajuste o valor para gerar mais ou menos tiles atrás
        {
            ExpandRoofBackward();
        }
    }

    void Update()
    {
        // Verifica se o cubo está se aproximando do final do plano à frente
        if (player.position.x > nextSpawnPositionX - expandThreshold)
        {
            ExpandRoofForward();
        }

        // Verifica se o cubo se afastou o suficiente para destruir os tiles antigos
        if (spawnedTiles.Count > 0 && player.position.x > spawnedTiles.Peek().transform.position.x + destroyThreshold)
        {
            DestroyOldTile();
        }
    }

    void ExpandRoofForward()
    {
        // Instancia um novo tile de chão à frente do último gerado, com Y = -28.54799 e Z = 0
        GameObject newTile = Instantiate(roofPrefab, new Vector3(nextSpawnPositionX, 27.6f, 10.2f), Quaternion.identity);

        // Adiciona o tile à fila para controlar quais tiles já foram gerados
        spawnedTiles.Enqueue(newTile);

        // Calcula o tamanho real do prefab em unidades do mundo, para espaçamento correto
        float tileRealLength = roofPrefab.GetComponent<Renderer>().bounds.size.x;

        // Atualiza a posição X para o próximo tile ser gerado à frente
        nextSpawnPositionX += tileRealLength;
    }

    void ExpandRoofBackward()
    {
        // Instancia um novo tile de chão atrás do último gerado, com Y = -28.54799 e Z = 0
        GameObject newTile = Instantiate(roofPrefab, new Vector3(previousSpawnPositionX, 27.6f, 10.2f), Quaternion.identity);

        // Adiciona o tile à fila para controlar quais tiles já foram gerados
        spawnedTiles.Enqueue(newTile);

        // Calcula o tamanho real do prefab em unidades do mundo, para espaçamento correto
        float tileRealLength = roofPrefab.GetComponent<Renderer>().bounds.size.x;

        // Atualiza a posição X para o próximo tile ser gerado atrás
        previousSpawnPositionX -= tileRealLength; // Subtrai para gerar para trás
    }

    void DestroyOldTile()
    {
        // Remove o tile mais antigo da fila e o destrói
        GameObject oldTile = spawnedTiles.Dequeue();
        Destroy(oldTile);
    }
}
