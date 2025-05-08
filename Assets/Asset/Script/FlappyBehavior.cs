using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class FlappyBehavior : MonoBehaviour
{
    [SerializeField] private float _velocity = 1.5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private AudioClip _flyClip;
    [SerializeField] private AudioClip _damageClip;
    [SerializeField] private GameObject _clickImage;

    private Rigidbody2D _rb;
    private AudioSource _AudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Space))
        {
            if (_clickImage != null && GameManager.instance.HasGameStarted == false) {
                GameManager.instance.HasGameStarted = true;
                _clickImage.SetActive(false);
                Time.timeScale = 1f;
            }

            _rb.linearVelocity = Vector2.up * _velocity;

            if (_AudioSource != null && GameManager.instance.getGameState() == GameState.GameActive)
            {
                _AudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                _AudioSource.PlayOneShot(_flyClip);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0,0, _rb.linearVelocity.y * _rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Collision") {
            return;
        }

        if (_AudioSource != null && GameManager.instance.getGameState() == GameState.GameActive)
        {
            _AudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            _AudioSource.PlayOneShot(_damageClip);
        }
        
        GameManager.instance.GameOver();    
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.AddScore(1);
    }
}
