// d:\Unity\FlappyBird\Assets\Asset\Script\LoopGround.cs
using UnityEngine;

public class LoopGround : MonoBehaviour
{
    // [SerializeField] private float _speed = 1f; // REMOVE THIS LINE
    [SerializeField] private float _width = 6f;

    private SpriteRenderer _spriteRenderer;
    private Vector2 _startSize;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) // Add null check
        {
             _startSize = new Vector2(_spriteRenderer.size.x, _spriteRenderer.size.y);
        } else {
            Debug.LogError("LoopGround script needs a SpriteRenderer component!", this.gameObject);
        }

    }

    private void Update()
    {
        if (_spriteRenderer == null) return; // Don't run if no sprite renderer

        // Get the current speed from the LevelManager
        float currentSpeed = LevelManager.instance != null ? LevelManager.instance.CurrentGroundSpeed : 1.0f; // Use base speed as fallback

        _spriteRenderer.size = new Vector2(_spriteRenderer.size.x + currentSpeed * Time.deltaTime, _spriteRenderer.size.y);

        // Check if the sprite has scrolled past its original width relative to the start position
        // This logic might need adjustment depending on how exactly you want the loop to work.
        // A common way is using Material Tiling Offset or having two ground sprites leapfrog.
        // The current size-based approach might look strange if the speed changes rapidly.
        // Let's stick to the original logic for now, but be aware it might need refinement.
        if (_spriteRenderer.size.x > _width)
        {
            _spriteRenderer.size = _startSize;
            // Consider resetting position or using a different looping mechanism if this doesn't work well.
        }
    }
}
