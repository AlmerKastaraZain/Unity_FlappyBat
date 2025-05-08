// d:\Unity\FlappyBird\Assets\Asset\Script\MovePipe.cs
using UnityEngine;

public class MovePipe : MonoBehaviour
{
    // [SerializeField] private float _speed = 0.65f; // REMOVE THIS LINE

    // Update is called once per frame
    void Update()
    {
        // Get the current speed from the LevelManager
        float currentSpeed = LevelManager.instance != null ? LevelManager.instance.CurrentPipeSpeed : 0.65f; // Use base speed as fallback

        transform.position += Vector3.left * currentSpeed * Time.deltaTime;
    }
}
