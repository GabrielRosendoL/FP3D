using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CubeController : MonoBehaviour
{
    public float flyForce = 30f;
    public float moveSpeed = 18f;
    public float dashSpeed = 28f;
    private Rigidbody rb;

    // Referências para o painel de Game Over e os elementos da UI
    public GameObject gameOverPanel;  // Painel de Game Over
    public TextMeshProUGUI gameOverText;  // Texto de Game Over
    public Button restartButton;  // Botão de reiniciar
    public TextMeshProUGUI scoreText;  // Texto da pontuação final
    public AudioClip gameOverSound;  // Som de Game Over
    public GameObject tutorialPanel;  // Painel de Tutorial
    public TextMeshProUGUI countdownText;  // Texto da contagem regressiva

    private bool isGameOver = false;
    private int score = 0;  // Pontuação começa em 0
    private Vector3 savedVelocity;  // Salva a velocidade ao pausar
    private AudioSource audioSource;

    // Renderer(s) do personagem
    private Renderer[] characterRenderers;  // Para armazenar todos os renderers
    private Color originalColor;
    private bool isDashing = false;  // Flag para saber se o dash está ativo

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        rb.mass = 5f;
        rb.useGravity = true;

        // Congela a rotação nos eixos X e Z para evitar que o cubo tombe para frente ou para os lados
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Esconder o painel de Game Over inicialmente
        gameOverPanel.SetActive(false);
        countdownText.gameObject.SetActive(false);  // Esconder o texto da contagem no início

        restartButton.onClick.AddListener(RestartGame);

        // Encontrar todos os Renderers nos sub-objetos do personagem
        characterRenderers = GetComponentsInChildren<Renderer>();

        // Salvar a cor original
        if (characterRenderers.Length > 0)
        {
            originalColor = characterRenderers[0].material.color;
        }

        // Verifica se o jogo está sendo reiniciado e pula o tutorial
        if (PlayerPrefs.GetInt("IsRestarting", 0) == 1)
        {
            StartGameWithoutTutorial();
        }
        else
        {
            ShowTutorialPanel();
        }
    }

    void Update()
    {
        if (isGameOver || Time.timeScale == 0f) return; // Adiciona verificação para pausa

        MoveRight();
        rb.AddForce(Vector3.down * 70f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fly();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartDash();
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            StopDash();
        }
    }

    void MoveRight()
    {
        float currentSpeed = Input.GetKey(KeyCode.Z) ? dashSpeed : moveSpeed;
        rb.velocity = new Vector3(currentSpeed, rb.velocity.y, 0);
    }

    void Fly()
    {
        rb.velocity = new Vector3(rb.velocity.x, flyForce, 0);
    }

    void StartDash()
    {
        rb.velocity = new Vector3(dashSpeed, rb.velocity.y, 0);
        isDashing = true;  // Ativar o dash
        StartCoroutine(ChangeColorDuringDash());  // Começar a mudar as cores
    }

    void StopDash()
    {
        isDashing = false;  // Parar o dash
        StopAllCoroutines();  // Parar a corrotina de mudar as cores
        ResetColor();  // Voltar à cor original
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.CompareTag("Obstacle") || 
            collision.gameObject.CompareTag("Teto"))
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;

        // Exibir o painel de Game Over
        gameOverPanel.SetActive(true);

        // Atualizar a pontuação no painel
        scoreText.text = "Score: " + score;

        // Tocar o som de Game Over
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
    }

    void RestartGame()
    {
        // Salva um valor para indicar que o jogo está sendo reiniciado
        PlayerPrefs.SetInt("IsRestarting", 1);
        PlayerPrefs.Save();

        // Reinicia a cena
        score = 0;  // Zera a pontuação ao reiniciar
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Método para incrementar a pontuação
    public void AddScore()
    {
        score++;
    }

    // Detecta quando o cubo passa pela linha (trigger)
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Line"))
        {
            AddScore();
        }
    }

    // Salvar e restaurar a velocidade ao pausar e retomar o jogo
    public void PauseGame()
    {
        savedVelocity = rb.velocity; // Salva a velocidade atual ao pausar
        rb.isKinematic = true; // Desativa a física enquanto pausado
    }

    public void ResumeGame()
    {
        rb.isKinematic = false; // Reativa a física
        rb.velocity = savedVelocity; // Restaura a velocidade ao retomar o jogo
    }

    // Método para resetar a cor original
    void ResetColor()
    {
        foreach (Renderer renderer in characterRenderers)
        {
            renderer.material.color = originalColor;  // Voltar à cor original
        }
    }

    // Corrotina para mudar a cor durante o dash
    IEnumerator ChangeColorDuringDash()
    {
        while (isDashing)  // Continuar enquanto o dash estiver ativo
        {
            // Gerar uma cor aleatória
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // Aplicar a cor a todos os renderers
            foreach (Renderer renderer in characterRenderers)
            {
                renderer.material.color = randomColor;
            }

            yield return new WaitForSeconds(0.1f);  // Mudar de cor a cada 0.1 segundos
        }
    }

    // Método para iniciar o jogo sem o tutorial
    void StartGameWithoutTutorial()
    {
        // Zera a flag de reinício
        PlayerPrefs.SetInt("IsRestarting", 0);
        PlayerPrefs.Save();

        // Esconder o tutorial e iniciar a contagem regressiva
        tutorialPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    // Método para mostrar o painel de tutorial
    void ShowTutorialPanel()
    {
        // Mostrar o painel de tutorial
        tutorialPanel.SetActive(true);
    }

    // Corrotina de contagem regressiva
    IEnumerator StartCountdown()
    {
        tutorialPanel.SetActive(false);  // Esconder o painel de tutorial
        countdownText.gameObject.SetActive(true);  // Ativar o texto de contagem regressiva

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();  // Mostrar a contagem regressiva
            yield return new WaitForSecondsRealtime(1f);  // Esperar 1 segundo em tempo real
        }

        countdownText.text = "GO!";  // Mostrar "GO!" no final
        yield return new WaitForSecondsRealtime(1f);  // Esperar um segundo antes de começar o jogo

        countdownText.gameObject.SetActive(false);  // Esconder o texto da contagem regressiva
        Time.timeScale = 1;  // Começar o jogo
    }
}
