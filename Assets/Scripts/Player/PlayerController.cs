using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	private MovementRigidbody2D _movement;
	private PlayerAnimator _playerAnimator;
	public bool isMoving = false;

	private string currentScene;

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
		currentScene = SceneManager.GetActiveScene().name;
		Debug.Log(currentScene);
	}

	private void Update()
	{
		if (isMoving == true || currentScene != "IntroScene")
		{
			float x = 0f;

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				x = -1f;
			}
			else if (Input.GetKey(KeyCode.RightArrow))
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
					_footStepInterval = 0.3f;
				}
				else if (speed >= 5f)
				{
					_footStepInterval = 0.45f;
				}
				else if (speed >= 3f)
				{
					_footStepInterval = 0.56f;
				}
				else
				{
					_footStepInterval = 0.63f;
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
		Managers.Sound.Play("all_s_walk2", Sound.Effect);
	}

	private void UpdateMove(float x)
	{
		_movement.MoveTo(x);
	}

	public float GetCurrentXSpeed()
	{
		return _movement.Velocity.x;
	}
}
