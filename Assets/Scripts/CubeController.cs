using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CubeController : MonoBehaviour
{
    public float flyForce = 8f;
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    private Rigidbody rb;

    // Referências para os elementos de Game Over na UI
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    private bool isGameOver = false;

    // Sistema de pontuação
    private int score = 0;  // Pontuação começa em 0

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 5f;
        rb.useGravity = true;

        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        if (isGameOver) return;

        MoveRight();
        rb.AddForce(Vector3.down * 70f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fly();
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
}
