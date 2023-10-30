using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] SpriteRenderer spriterenderer;
    [SerializeField] Animator animator;
    [SerializeField] float Speed;

    [Header("Rolling")]
    private bool CanRoll = true;
    private bool IsRolling;
    private bool IsAttacking;
    [SerializeField] private float RollingPower = 1.5f;
    [SerializeField] private float RollingTime = 0.8f;
    [SerializeField] private float RollingCooldown = 1.25f;
    [SerializeField] private AudioClip RollSound;
    [SerializeField] private AudioClip Footsteps;

    Rigidbody2D RB;
    Vector2 Velocity = Vector2.zero;

    void Start() {
        RB = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (IsRolling || IsAttacking) return;

        Vector2 Direction = Vector2.zero;
        Direction.x = Input.GetAxis("Horizontal");
        Direction.y = Input.GetAxis("Vertical");
        Direction.Normalize();

        Velocity.x = Direction.x * Speed;
        Velocity.y = Direction.y * Speed;

        // Rolling
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanRoll) {
            animator.SetTrigger("Roll");
            //SoundManager.Instance.PlaySound(RollSound);
            StartCoroutine(Roll());
        }

        // Move player
        RB.velocity = Velocity;

        animator.SetFloat("Horizontal", Direction.x);
        animator.SetFloat("Vertical", Direction.y);
        animator.SetFloat("Speed", Direction.sqrMagnitude);

    }

    IEnumerator Roll() {
        IsRolling = true;
        CanRoll = false;
        Physics2D.IgnoreLayerCollision(3, 6, true);
        Velocity = RB.velocity * RollingPower;
        yield return new WaitForSeconds(RollingTime);
        IsRolling = false;
        Physics2D.IgnoreLayerCollision(3, 6, false);
        yield return new WaitForSeconds(RollingCooldown);
        CanRoll = true;
    }
}