using UnityEngine;
using System.Collections.Generic;  // Necessário para usar Queue

public class LampExpander : MonoBehaviour
{
    public GameObject lampPrefab;     
    public Transform player;            
    public float expandThreshold = 100f; 
    public float destroyThreshold = 300f; 
    public float spacing = 20f; // Novo: variável para definir o espaçamento entre as lamps
    private float nextSpawnPositionX; 
    private float previousSpawnPositionX; 
    private Queue<GameObject> spawnedTiles = new Queue<GameObject>();  

    void Start()
    {
        nextSpawnPositionX = transform.position.x;
        previousSpawnPositionX = transform.position.x;

        // Gerar inicialmente 5 tiles para frente
        for (int i = 0; i < 5; i++)  
        {
            ExpandLampForward();
        }

        // Gerar inicialmente 5 tiles para trás
        for (int i = 0; i < 5; i++)  
        {
            ExpandLampBackward();
        }
    }

    void Update()
    {
        // Expandir para frente se o jogador estiver perto do próximo tile
        if (player.position.x > nextSpawnPositionX - expandThreshold)
        {
            ExpandLampForward();
        }

        // Destruir os tiles antigos que estão muito longe do jogador
        if (spawnedTiles.Count > 0 && player.position.x > spawnedTiles.Peek().transform.position.x + destroyThreshold)
        {
            DestroyOldTile();
        }
    }

    void ExpandLampForward()
    {
        // Defina a rotação que você quer (X = 0, Y = 180, Z = 0)
        Quaternion rotation = Quaternion.Euler(0, 180, 0);

        // Instancie o tile com a rotação definida
        GameObject newTile = Instantiate(lampPrefab, new Vector3(nextSpawnPositionX, 13.6f, 30.2f), rotation);

        // Adicionar o novo tile à fila
        spawnedTiles.Enqueue(newTile);

        // Obtenha o comprimento real do tile e adicione o espaçamento
        float tileRealLength = lampPrefab.GetComponent<Renderer>().bounds.size.x;
        nextSpawnPositionX += tileRealLength + spacing; // Adicionando o espaçamento aqui
    }

    void ExpandLampBackward()
    {
        // Defina a rotação que você quer (X = 0, Y = 180, Z = 0)
        Quaternion rotation = Quaternion.Euler(0, 180, 0);

        // Instancie o tile com a rotação definida
        GameObject newTile = Instantiate(lampPrefab, new Vector3(previousSpawnPositionX, 13.6f, 30.2f), rotation);

        // Adicionar o novo tile à fila
        spawnedTiles.Enqueue(newTile);

        // Obtenha o comprimento real do tile e adicione o espaçamento
        float tileRealLength = lampPrefab.GetComponent<Renderer>().bounds.size.x;
        previousSpawnPositionX -= tileRealLength + spacing; // Adicionando o espaçamento aqui
    }

    void DestroyOldTile()
    {
        // Remove o tile mais antigo da fila e destrói o objeto
        GameObject oldTile = spawnedTiles.Dequeue();
        Destroy(oldTile);
    }
}
