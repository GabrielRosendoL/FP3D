using UnityEngine;
using System.Collections.Generic;  // Necessário para usar Queue
using System.Collections; // Necessário para usar Coroutines

public class LampExpander : MonoBehaviour
{
    public GameObject lampPrefab;     
    public Transform player;            
    public float expandThreshold = 100f; 
    public float destroyThreshold = 300f; 
    public float spacing = 60f; // Novo: variável para definir o espaçamento entre as lamps
    public float startDelay = 2f; // Novo: variável para definir o atraso antes de começar a gerar as lamps
    public float initialDistance = 100f; // Distância inicial longe do jogador para gerar os tiles

    private float nextSpawnPositionX; 
    private float previousSpawnPositionX; 
    private Queue<GameObject> spawnedTiles = new Queue<GameObject>();  
    private bool isGenerating = false; // Novo: variável para controlar se a geração já começou

    void Start()
    {
        // Iniciar a geração das lâmpadas fora do campo de visão do jogador
        nextSpawnPositionX = transform.position.x + initialDistance;
        previousSpawnPositionX = transform.position.x - initialDistance;

        // Iniciar a geração das lâmpadas com um atraso
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        // Aguardar pelo tempo definido em startDelay antes de iniciar a geração
        yield return new WaitForSeconds(startDelay);

        // Depois do atraso, mudar o estado para começar a gerar
        isGenerating = true;

        // Gerar inicialmente 5 tiles para frente, fora do campo de visão
        for (int i = 0; i < 5; i++)  
        {
            ExpandLampForward();
        }

        // Gerar inicialmente 5 tiles para trás, fora do campo de visão
        for (int i = 0; i < 5; i++)  
        {
            ExpandLampBackward();
        }
    }

    void Update()
    {
        // Só gerar e destruir tiles se já estiver no estado de geração
        if (isGenerating)
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
