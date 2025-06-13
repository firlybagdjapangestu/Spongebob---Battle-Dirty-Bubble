using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void OnEnable()
    {
        GameEventHub.OnCameraShake += Shake;
    }

    private void OnDisable()
    {
        GameEventHub.OnCameraShake -= Shake;
    }

    private void Shake(float intensity)
    {
        impulseSource.GenerateImpulse(intensity);
    }
}
