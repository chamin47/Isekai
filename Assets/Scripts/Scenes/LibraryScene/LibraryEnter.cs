using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Playables;

public class LibraryEnter : MonoBehaviour
{
	public float zoomDuration = 2f;
	public float targetZoom = 4f;
	public float delayBeforeLoad = 2f;

	public PlayableDirector timeline;
	private PlayerController playerController;

	private Camera mainCamera;
	private float originalZoom;

	// Ʈ���� �浹 ������ �÷��̾� ��ġ�� ������ ����
	private Vector3 triggerPlayerPosition;

	[SerializeField] private GameObject _gauidLine;

	private bool canUI = true;

	private void Start()
	{
		mainCamera = Camera.main;
		playerController = FindAnyObjectByType<PlayerController>();
		timeline = FindAnyObjectByType<PlayableDirector>();
		if (mainCamera != null)
		{
			mainCamera.orthographic = true;
			originalZoom = mainCamera.orthographicSize;
		}
		else
		{
			Debug.LogError("���� ī�޶� ã�� �� �����ϴ�!");
		}
    }

    public void Update()
    {
        if(canUI && Input.GetKeyDown(KeyCode.M))
		{
            _gauidLine.SetActive(!_gauidLine.activeSelf);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			// Ʈ���ſ� �ε��� ������ �÷��̾� ��ġ ����
			triggerPlayerPosition = collision.transform.position;
			playerController.canMove = false;
			Debug.Log(triggerPlayerPosition);
			StartCoroutine(HandleSceneTransition());
		}
	}

	private IEnumerator HandleSceneTransition()
	{
		canUI = false;
        _gauidLine.SetActive(false);

        if (timeline != null)
		{
			timeline.Play();
			while (timeline.state == PlayState.Playing)
			{
				yield return null;
			}
		}
		else
		{
			Debug.LogWarning("Ÿ�Ӷ��� ������ �����ϴ�.");
		}

		PixelPerfectCamera pixelPerfectCamera = mainCamera.GetComponent<PixelPerfectCamera>();
		if (pixelPerfectCamera != null)
			pixelPerfectCamera.enabled = false;

		Vector3 startPos = mainCamera.transform.position;

		
		// ����� �浹 ������ �÷��̾� ��ġ�� �������� �ϵ��ڵ�
		Vector3 endPos = new Vector3(
			24.78f,
			-0.48f,
			-10f
			);

		float startZoom = originalZoom;
		float elapsed = 0f;

		while (elapsed < zoomDuration)
		{
			float t = elapsed / zoomDuration;
			mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
			mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);
			elapsed += Time.deltaTime;
			yield return null;
		}

		mainCamera.transform.position = endPos;
		mainCamera.orthographicSize = targetZoom;

		yield return new WaitForSeconds(delayBeforeLoad);

		Managers.Scene.LoadScene(Scene.LibraryScene);
	}
}
