using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 1f; // Èçµé¸®´Â ½Ã°£
    public float magnitude = 0.2f; // Èçµé¸®´Â ¼¼±â
    public int count = 6; // Èçµé¸®´Â È½¼ö
    private Vector3 _initialPosition;

    private Coroutine _shakeCoroutine;

    private void Start()
    {
        _initialPosition = transform.localPosition;
    }

    public void Shake(float duration = 1f, float magnitude = 0.2f)
    {
        if(_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(CoShake());
    }

    private IEnumerator CoShake()
    {
        _initialPosition = transform.localPosition;

        for (int i = 0; i < count; i++)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude * (1 - i / count);
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude * (1 - i / count);

            transform.localPosition = _initialPosition + new Vector3(x, y, 0);

            yield return WaitForSecondsCache.Get(duration / count); 
        }

        transform.localPosition = _initialPosition; 
    }
}
