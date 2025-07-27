using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Playables;
using System;

public class LibraryEnter : MonoBehaviour
{
	public float zoomDuration = 2f;
	public float targetZoom = 4f;
	public float delayBeforeLoad = 2f;

	public PlayableDirector timeline;
	private PlayerController _playerController;

	private Camera _mainCamera;
	private float _originalZoom;

	[SerializeField] private GameObject _gauidLine;
	[SerializeField] private GameObject _mouse;
	[SerializeField] private GameObject _diary;

    private bool _canUI = true;
	private Coroutine _mouseCoroutine;
    private bool _isMouseVisible = true;

    private void Start()
	{
		_mainCamera = Camera.main;
		_playerController = (Managers.Scene.CurrentScene as IntroScene).Player;
        timeline = FindAnyObjectByType<PlayableDirector>();
		_originalZoom = _mainCamera.orthographicSize;
		
        _mouseCoroutine = StartCoroutine(BlinkMouse());
    }

    public void Update()
    {
        if (_canUI && Input.GetKeyDown(KeyCode.M))
        {
            _gauidLine.SetActive(!_gauidLine.activeSelf);
        }

        if (_isMouseVisible && _diary.activeInHierarchy == false)
        {
            _isMouseVisible = false;
            StopCoroutine(_mouseCoroutine);
            _mouse.SetActive(false);
        }
    }

    private IEnumerator BlinkMouse()
    {
		float interval = 0.5f;
		while(true)
		{
            _mouse.SetActive(false);
            yield return WaitForSecondsCache.Get(interval);
            _mouse.SetActive(true);
            yield return WaitForSecondsCache.Get(interval);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			_playerController.canMove = false;
			
			StartCoroutine(HandleSceneTransition());
		}
	}

	private IEnumerator HandleSceneTransition()
	{
		_canUI = false;
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

		Vector3 startPos = _mainCamera.transform.position;

		// ����� �浹 ������ �÷��̾� ��ġ�� �������� �ϵ��ڵ�
		Vector3 endPos = new Vector3(
			24.78f,
			-0.48f,
			-10f
			);

		float startZoom = _originalZoom;
		float elapsed = 0f;

		while (elapsed < zoomDuration)
		{
			float t = elapsed / zoomDuration;
			_mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
			_mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);
			elapsed += Time.deltaTime;
			yield return null;
		}

		_mainCamera.transform.position = endPos;
		_mainCamera.orthographicSize = targetZoom;

		yield return WaitForSecondsCache.Get(delayBeforeLoad);

		Managers.Scene.LoadScene(Scene.LibraryScene);
	}
}
