using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CubeController : MonoBehaviour
{
    public float flyForce = 40f;
    public float moveSpeed = 40f;
    public float dashSpeed = 70f;
    private Rigidbody rb;

    // Referências para o painel de Game Over e os elementos da UI
    public GameObject gameOverPanel;  // Painel de Game Over
    public TextMeshProUGUI gameOverText;  // Texto de Game Over
    public Button restartButton;  // Botão de reiniciar
    public TextMeshProUGUI scoreText;  // Texto da pontuação final
    public TextMeshProUGUI scoreTextInGame;  // Texto da pontuação final

    public AudioClip gameOverSound;  // Som de Game Over
    public GameObject tutorialPanel;  // Painel de Tutorial
    public TextMeshProUGUI countdownText;  // Texto da contagem regressiva
    public GameObject pauseMenu;  // Painel de Pause Menu

    private bool isGameOver = false;
    private bool isPaused = false; // Estado de pausa do jogo
    private int score = 0;  // Pontuação começa em 0
    private Vector3 savedVelocity;  // Salva a velocidade ao pausar
    private AudioSource audioSource;

    // Renderer(s) do personagem
    private Renderer[] characterRenderers;  // Para armazenar todos os renderers
    private Color originalColor;
    private bool isDashing = false;  // Flag para saber se o dash está ativo

    void Start()
    {
        Physics.gravity = new Vector3(0, -30f, 0);  // Define a gravidade global
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        rb.mass = 5f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        gameOverPanel.SetActive(false);
        pauseMenu.SetActive(false);
        countdownText.gameObject.SetActive(false);

        scoreText.text = "SCORE: " + score;

        restartButton.onClick.AddListener(RestartGame);

        characterRenderers = GetComponentsInChildren<Renderer>();

        if (characterRenderers.Length > 0)
        {
            originalColor = characterRenderers[0].material.color;
        }

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
        // Se o jogo acabou ou se o jogo estiver pausado, verifica se é necessário reiniciar
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Z) || IsScreenTouched())  // Tecla Z ou toque reinicia o jogo
            {
                RestartGame();
            }
            return;  // Não continua processando o restante das ações quando o jogo acabou
        }

        if (Time.timeScale == 0f) return; // Adiciona verificação para pausa

        MoveRight();
        rb.AddForce(Vector3.down * 450f); // Força da Gravidade

        // Verifica se a barra de espaço ou a tela foi tocada para voar
        if (Input.GetKeyDown(KeyCode.Space) || IsScreenTouched())
        {
            Fly();  // Se o jogo não acabou, a barra de espaço ou toque faz o cubo voar
        }

        // Verifica se a tecla Z foi pressionada ou a tela foi tocada com dois dedos para iniciar o dash
        if (Input.GetKeyDown(KeyCode.Z) || IsDoubleTouch())
        {
            StartDash();
        }

        if (Input.GetKeyUp(KeyCode.Z) || Input.touchCount == 0)
        {
            StopDash();
        }

        // Toggle do menu de pausa com a tecla "Escape"
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    void MoveRight()
    {
        // Verifica se o Rigidbody NÃO é cinemático antes de ajustar a velocidade
        if (!rb.isKinematic)
        {
            float currentSpeed = Input.GetKey(KeyCode.Z) ? dashSpeed : moveSpeed;
            rb.velocity = new Vector3(currentSpeed, rb.velocity.y, 0);
        }
    }

    void Fly()
    {
        rb.velocity = new Vector3(rb.velocity.x, flyForce, 0);
    }

    void StartDash()
    {
        rb.velocity = new Vector3(dashSpeed, rb.velocity.y, 0);
        isDashing = true;  // Ativar o dash
        StartCoroutine(GlowDuringDash());  // Iniciar o brilho branco
    }

    void StopDash()
    {
        isDashing = false;  // Parar o dash
        StopAllCoroutines();  // Parar a corrotina de brilho
        ResetColor();  // Voltar à cor original
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se a tag do objeto colidido é "Ground", "Obstacle" ou "Teto"
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.CompareTag("Obstacle") || 
            collision.gameObject.CompareTag("Teto"))
        {
            GameOver();  // Chama o método de Game Over
        }
    }

    void GameOver()
    {
        isGameOver = true;

        // Exibir o painel de Game Over
        gameOverPanel.SetActive(true);

        // Atualizar a pontuação no painel
        scoreText.text = "SCORE: " + score;

        // Tocar o som de Game Over
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Congelar o jogo ao dar Game Over
        Time.timeScale = 0f;
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
        score++;  // Incrementa a pontuação
        scoreTextInGame.text = "SCORE: " + score;  // Atualiza o texto da pontuação em tempo real
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
        Time.timeScale = 0f;  // Pausa o jogo
        pauseMenu.SetActive(true);  // Ativar o painel de pausa
        isPaused = true;

        // Esconder a contagem regressiva ao pausar o jogo
        countdownText.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        rb.isKinematic = false; // Reativa a física
        rb.velocity = savedVelocity; // Restaura a velocidade ao retomar o jogo
        pauseMenu.SetActive(false);  // Desativar o painel de pausa
        isPaused = false;

        // Iniciar a contagem regressiva após retomar o jogo
        StartCoroutine(StartCountdown());
    }

    // Método para resetar a cor original e remover emissão
    void ResetColor()
    {
        foreach (Renderer renderer in characterRenderers)
        {
            renderer.material.DisableKeyword("_EMISSION");  // Desativar a emissão
            renderer.material.SetColor("_EmissionColor", Color.black);  // Zerar a emissão
        }
    }

    // Corrotina para fazer o personagem emitir um brilho branco durante o dash
    IEnumerator GlowDuringDash()
    {
        while (isDashing)
        {
            foreach (Renderer renderer in characterRenderers)
            {
                // Ativar o brilho branco
                renderer.material.EnableKeyword("_EMISSION");
                Color emissionColor = Color.white * Mathf.LinearToGammaSpace(1.5f);
                renderer.material.SetColor("_EmissionColor", emissionColor);
            }

            yield return new WaitForSeconds(0.1f);  // Manter o brilho por 0.2 segundos

            foreach (Renderer renderer in characterRenderers)
            {
                // Desativar o brilho
                renderer.material.DisableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", Color.black);
            }

            yield return new WaitForSeconds(0.1f);  // Sem brilho por 0.2 segundos
        }

        // Quando o dash acabar, garantir que a emissão será removida
        ResetColor();
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

    // Corrotina de contagem regressiva com "GO!"
    IEnumerator StartCountdown()
    {
        // Garantir que o jogo fique pausado durante a contagem
        Time.timeScale = 0f;
        countdownText.gameObject.SetActive(true);  // Ativar o texto de contagem regressiva

        for (int i = 3; i > 0; i--)
        {
            // Verifica se o jogo está pausado, se sim, esconde o countdown e para a corrotina
            if (isPaused)
            {
                countdownText.gameObject.SetActive(false);
                yield break;  // Sai da corrotina
            }

            countdownText.text = i.ToString();  // Mostrar a contagem regressiva
            yield return new WaitForSecondsRealtime(1f);  // Esperar 1 segundo em tempo real
        }

        countdownText.text = "GO!";  // Mostrar "GO!" no final
        yield return new WaitForSecondsRealtime(1f);  // Esperar um segundo antes de começar o jogo

        countdownText.gameObject.SetActive(false);  // Esconder o texto da contagem regressiva

        // Agora que a contagem acabou, retomar o jogo
        Time.timeScale = 1f;  // Começar o jogo
    }

    // Verifica se a tela foi tocada (um toque único)
    bool IsScreenTouched()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }

    // Verifica se dois dedos estão tocando a tela ao mesmo tempo (para o dash)
    bool IsDoubleTouch()
    {
        return Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began;
    }
}
