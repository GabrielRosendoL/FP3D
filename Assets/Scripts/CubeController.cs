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

    // Referências para os elementos de Game Over na UI
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    private bool isGameOver = false;

    // Sistema de pontuação
    private int score = 0;  // Pontuação começa em 0

    private Vector3 savedVelocity; // Salva a velocidade ao pausar

    // Áudio
    public AudioClip jumpSound;  // Som do pulo
    public AudioClip dashSound;  // Som do dash
    private AudioSource audioSource;

    // Renderer(s) do personagem
    private Renderer[] characterRenderers;  // Para armazenar todos os renderers

    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        rb.mass = 5f;
        rb.useGravity = true;

        // Congela a rotação nos eixos X e Z para evitar que o cubo tombe para frente ou para os lados
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);

        // Encontrar todos os Renderers nos sub-objetos do personagem
        characterRenderers = GetComponentsInChildren<Renderer>();

        // Salvar a cor original
        if (characterRenderers.Length > 0)
        {
            originalColor = characterRenderers[0].material.color;
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
    }

    void MoveRight()
    {
        float currentSpeed = Input.GetKey(KeyCode.Z) ? dashSpeed : moveSpeed;
        rb.velocity = new Vector3(currentSpeed, rb.velocity.y, 0);
    }

    void Fly()
    {
        rb.velocity = new Vector3(rb.velocity.x, flyForce, 0);
        PlaySound(jumpSound);  // Toca o som de pulo
    }

    void StartDash()
    {
        rb.velocity = new Vector3(dashSpeed, rb.velocity.y, 0);
        PlaySound(dashSound);  // Toca o som de dash
        StartCoroutine(ChangeColorDuringDash());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Teto"))
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        Debug.Log("Game Over!");
    }

    void RestartGame()
    {
        score = 0;  // Zera a pontuação ao reiniciar
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Método para incrementar a pontuação
    public void AddScore()
    {
        score++;
        Debug.Log("Pontuação: " + score);  // Exibe a pontuação no console
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

    // Método para tocar sons
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Corrotina para mudar a cor durante o dash
    IEnumerator ChangeColorDuringDash()
    {
        // Mudar a cor de todos os renderers do personagem
        foreach (Renderer renderer in characterRenderers)
        {
            renderer.material.color = Color.red;  // Muda a cor para vermelho durante o dash
        }

        yield return new WaitForSeconds(0.5f);  // Duração do dash

        // Voltar à cor original
        foreach (Renderer renderer in characterRenderers)
        {
            renderer.material.color = originalColor;
        }
    }
}
