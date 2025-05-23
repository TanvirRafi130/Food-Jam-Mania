using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // The main camera
    public Camera mainCamera;

    // The shake power (can be adjusted in the editor)
    public float shakePower = 1.0f;

    // The duration of the shake effect
    public float shakeDuration = 0.5f;



    private static CameraShake _instance;

    public static CameraShake Instance => _instance;


    private void Awake()
    {
        _instance = this;

    }


    private void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }

    // Public function to shake the camera
    public void ShakeCamera()
    {
        // Create a coroutine to handle the shake effect
      //  Debug.LogError("Reached Camera shake");
        StartCoroutine(ShakeCameraCoroutine());
    }

    // Coroutine to handle the shake effect
    private IEnumerator ShakeCameraCoroutine()
    {
       // Debug.LogError("reached corouting");
        // Store the initial camera position
        Vector3 initialPosition = mainCamera.transform.position;

        // Calculate the shake offset
        float shakeOffset = shakePower * 0.01f;

        // Shake the camera for the specified duration
        float timer = 0;
        while (timer < shakeDuration)
        {
            // Calculate the new camera position
            Vector3 newPosition = initialPosition + new Vector3(
                Mathf.PerlinNoise1D(timer * 10) * shakeOffset,
                Mathf.PerlinNoise1D(timer * 10 + 1) * shakeOffset,
                0
            );

            // Set the new camera position
            mainCamera.transform.position = newPosition;

            // Increment the timer
            timer += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Reset the camera position
        mainCamera.transform.position = initialPosition;
    }
}