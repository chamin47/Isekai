using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRigidbody2D : MonoBehaviour
{
	[Header("Move")]
	[SerializeField] private float _moveSpeed = 2.5f;

	[SerializeField] private float _gravityScale = 3.5f;

	private Vector2 _collisionSize;
	private Vector2 _footPosition;

	private Rigidbody2D _rigid2D;
	private Collider2D _collider2D;

	public Vector2 Velocity => _rigid2D.velocity;

	public bool isTestMode = false;

	private void Awake()
	{
		_rigid2D = GetComponent<Rigidbody2D>();
		_collider2D = GetComponent<Collider2D>();
		_rigid2D.gravityScale = _gravityScale;
    }

    public void MoveTo(float x)
	{
		if(!isTestMode)
		{
			if (Managers.Happy.Happiness >= 0 && Managers.Happy.Happiness <= 20)
			{
				_moveSpeed = Managers.DB.GetPlayerData().moveSpeed[0];
			}
			else if (Managers.Happy.Happiness > 20 && Managers.Happy.Happiness <= 50)
			{
				_moveSpeed = Managers.DB.GetPlayerData().moveSpeed[1];
			}
			else
			{
				_moveSpeed = Managers.DB.GetPlayerData().moveSpeed[2];
			}
		}

		if (x != 0) x = Mathf.Sign(x);

		_rigid2D.velocity = new Vector2(x * _moveSpeed, _rigid2D.velocity.y);
	}

	public float GetMoveSpeed()
	{
		return _moveSpeed;
	}

    private bool _isKnockback = false;
    public bool IsKnockback => _isKnockback;

    public void Knockback(Vector2 direction, float force, float duration = 0.3f)
    {
        if (_isKnockback) return;
        StartCoroutine(KnockbackCoroutine(direction, force, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float force, float duration)
    {
        _isKnockback = true;
        _rigid2D.velocity = Vector2.zero;
        _rigid2D.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);

        _isKnockback = false;
    }
}
