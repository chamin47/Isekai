using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	private MovementRigidbody2D _movement;
	private PlayerAnimator _playerAnimator;
    public bool canMove = false;

	private string _currentScene;

	[SerializeField] private float _footStepInterval = 1f;
	[SerializeField] private float _maxSpeed = 7f;
	private float _footStepTimer = 0f;

	// dir, moveSpeed
	public event Action<Vector2, float> OnPlayerMove;

	private void Awake()
	{
		_movement = GetComponent<MovementRigidbody2D>();
		_playerAnimator = GetComponentInChildren<PlayerAnimator>();
	}

	private void Start()
	{
		_currentScene = SceneManager.GetActiveScene().name;
		Debug.Log(_currentScene);
	}

	private void Update()
	{
		if (_movement.IsKnockback)
		{
			return;
		}

		if (canMove == true)
		{
			float x = 0f;

			if (Input.GetKey(KeyCode.A))
			{
				x = -1f;
			}
			else if (Input.GetKey(KeyCode.D))
			{
				x = 1f;
			}

			OnPlayerMove?.Invoke(new Vector2(x, 0), _movement.GetMoveSpeed());

			UpdateMove(x);
			_playerAnimator.UpdateAnimation(x);

			if (x != 0)
			{
				float speed = _movement.GetMoveSpeed();

				if (speed >= 7f)
				{
					_footStepInterval = 0.33f;
				}
				else if (speed >= 5f)
				{
					_footStepInterval = 0.48f;
				}
				else if (speed >= 3f)
				{
                    _footStepInterval = 0.59f;
                }
				else if(speed >= 1f)
				{
                    _footStepInterval = 0.66f;
                }
				else
				{
					_footStepInterval = 0.66f + (1 - speed);
				}
                _footStepTimer += Time.deltaTime;

				if (_footStepTimer >= _footStepInterval)
				{
					PlayFootSound();
					_footStepTimer = 0f;
				}
			}
		}
		else
		{
			UpdateMove(0);
			_playerAnimator.UpdateAnimation(0);
		}
	}

	public void PlayFootSound()
	{
		Managers.Sound.Play("all_s_walk2", Sound.Effect, 0.8f);
	}

	private void UpdateMove(float x)
	{
		_movement.MoveTo(x);
	}

	public void SetLook(float x)
	{
        _playerAnimator.SpriteFlipX(x);
    }

	public float GetCurrentXSpeed()
	{
		return _movement.Velocity.x;
	}

    public IEnumerator CoMoveToTarget(Transform target)
    {
		
		_playerAnimator.SetCutSceneTalk();
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, transform.position.z);
        Vector3 startPos = transform.position;

		float duration = Vector3.Distance(startPos, targetPos);

        float direction = Mathf.Sign(targetPos.x - startPos.x);
        if (direction != 0)
        {
            _playerAnimator.SpriteFlipX(direction);
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        _playerAnimator.EndCurSceneTalk();
        // 목표 지점에 정확히 도착
        transform.position = targetPos;
        _footStepTimer = 0f;
		yield return null;
    }

    private void OnDisable()
    {
		UpdateMove(0);
    }
}
