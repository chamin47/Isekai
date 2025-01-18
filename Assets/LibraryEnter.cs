using System.Collections;
using UnityEngine;

public class LibraryEnter : MonoBehaviour
{
	// ���� ���� (OrthographicSize ���)
	public float zoomDuration = 1f;      // ���� �ִϸ��̼� ���� �ð�
	public float targetZoom = 3f;        // ��ǥ Orthographic Size (�������� ���� ����)

	// �� ��ȯ ���� �ð�
	public float delayBeforeLoad = 2f;   // �� ��ȯ �� ��� �ð�

	private Camera mainCamera;
	private float originalZoom;

	// �÷��̾� ��ġ�� �޾ƿ��� ���� ����
	private Transform playerTransform;

	// ī�޶� ������
	public float offsetX = 2f;
	public float offsetY = 1000000f;  // ���� �߰��� y�� ������

	private void Start()
	{
		mainCamera = Camera.main;
		if (mainCamera != null)
		{
			// Ȥ�� ī�޶� Perspective ����� Orthographic ���� ��ȯ
			mainCamera.orthographic = true;

			// ���� orthographicSize �� ����
			originalZoom = mainCamera.orthographicSize;
		}
		else
		{
			Debug.LogError("���� ī�޶� ã�� �� �����ϴ�!");
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			// Ʈ���ſ� ���� Collider�� ���� �÷��̾� transform Ȯ��
			playerTransform = collision.transform;
			StartCoroutine(HandleSceneTransition());
		}
	}

	private IEnumerator HandleSceneTransition()
	{
		if (mainCamera == null || playerTransform == null) yield break;

		float elapsed = 0f;

		// ī�޶� ���� ��ġ
		Vector3 startPos = mainCamera.transform.position;
		// ��ǥ ��ġ: x��� y������ ���� ������ ����
		Vector3 endPos = new Vector3(
			playerTransform.position.x + offsetX,
			playerTransform.position.y + offsetY,
			startPos.z
		);

		float startZoom = originalZoom;

		// ī�޶� �̵� �� ����
		while (elapsed < zoomDuration)
		{
			float t = elapsed / zoomDuration;

			// ī�޶� ��ġ �̵�
			mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
			// Orthographic Size ���� (�۾������� ����)
			mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);

			elapsed += Time.deltaTime;
			yield return null;
		}

		// ������ ����
		mainCamera.transform.position = endPos;
		mainCamera.orthographicSize = targetZoom;

		// �߰� ���� �ð�
		yield return new WaitForSeconds(delayBeforeLoad);

		// �� ��ȯ
		Managers.Scene.LoadScene(Scene.LibraryScene);

		// (���� ����) �� ��ȯ �� ī�޶� ������� ����
		// elapsed = 0f;
		// while (elapsed < zoomDuration)
		// {
		//     float t = elapsed / zoomDuration;
		//     mainCamera.transform.position = Vector3.Lerp(endPos, startPos, t);
		//     mainCamera.orthographicSize = Mathf.Lerp(targetZoom, startZoom, t);
		//     elapsed += Time.deltaTime;
		//     yield return null;
		// }
		//
		// mainCamera.transform.position = startPos;
		// mainCamera.orthographicSize = startZoom;
	}
}
