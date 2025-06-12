using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	private Animator _animator;
	private MovementRigidbody2D _movement;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
	{
		_animator = GetComponent<Animator>();
		_movement = GetComponentInParent<MovementRigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();		
    }

	public void UpdateAnimation(float x)
	{
		if (x != 0)
		{
			SpriteFlipX(x);
		}

		_animator.SetFloat("Speed", Mathf.Abs(x) * Mathf.Abs(_movement.Velocity.x));
	}

	public void SpriteFlipX(float x)
	{
		if(Time.timeScale == 0) return;
        _spriteRenderer.flipX = x < 0 ? true : false;

    }

    
}
