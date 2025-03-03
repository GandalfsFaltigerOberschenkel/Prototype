using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UFOAnimation : MonoBehaviour
{
    public float amplitude = 1f;
    public float frequency = 1f;
    public Light2D light2D;
    private Vector3 initialPosition;
    public float lightFrequency = 1f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }


    private void Update()
    {
        light2D.intensity = Mathf.Sin(Time.time * lightFrequency) * amplitude + 1;
        transform.position = initialPosition + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
        transform.rotation = initialRotation * Quaternion.Euler(0, 0, (Mathf.Sin(Time.time * frequency) + Mathf.Cos(Time.time)*frequency) * amplitude * 10);
    }

}
