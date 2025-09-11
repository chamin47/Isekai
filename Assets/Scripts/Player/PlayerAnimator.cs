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

    private void Start()
    {
        WorldType currentWorld = Managers.World.CurrentWorldType;
		Scene currentScene = Managers.Scene.CurrentScene.SceneType;

		_animator.SetBool("IdleWorld", currentScene != Scene.GameScene);
		_animator.SetBool("VinterWorld", currentWorld == WorldType.Vinter);
		_animator.SetBool("ChaumWorld", currentWorld == WorldType.Chaumm);
		_animator.SetBool("GangWorld", currentWorld == WorldType.Gang);
		_animator.SetBool("PelmanusWorld", currentWorld == WorldType.Pelmanus);
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
