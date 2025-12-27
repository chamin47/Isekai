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

    private bool _canUI = true;


    private void Start()
	{
		_mainCamera = Camera.main;
		_playerController = (Managers.Scene.CurrentScene as IntroScene).Player;
        timeline = FindAnyObjectByType<PlayableDirector>();
		_originalZoom = _mainCamera.orthographicSize;
    }

    public void Update()
    {
        if (_canUI && Input.GetKeyDown(KeyCode.M))
        {
            _gauidLine.SetActive(!_gauidLine.activeSelf);
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
			Debug.LogWarning("타임라인 참조가 없습니다.");
		}

		Vector3 startPos = _mainCamera.transform.position;

		// 저장된 충돌 시점의 플레이어 위치를 기준으로 하드코딩
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
