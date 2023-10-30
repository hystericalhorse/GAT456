using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    [Header("Health")]
    [SerializeField] private float StartingHealth;
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    private Animator animator;
    private Rigidbody2D RB;

    [Header("IFrames")]
    [SerializeField] private float IFrameDuration;
    [SerializeField] private int NumberOfFlashes;
    private SpriteRenderer spriterenderer;

    [Header("Components")]
    [SerializeField] private Behaviour[] Components;
    private bool Invulnerable = false;

    [Header("Audio")]
    [SerializeField] private AudioClip HurtSound;
    [SerializeField] private AudioClip DeathSound;

    private Material ShaderMaterial;

    public void Awake() {
        CurrentHealth = StartingHealth;
        animator = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody2D>();
        ShaderMaterial = GetComponent<SpriteRenderer>().material;
        MaxHealth = StartingHealth;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            TakeDamage(1);
            //Debug.Log(CurrentHealth);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            AddHealth(1);
            //Debug.Log(CurrentHealth);
        }
    }

    public void TakeDamage(float damage) {
        if (!Invulnerable) CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, StartingHealth);

        if (CurrentHealth > 0) {
            animator.SetTrigger("IsHit");
            StartCoroutine(Invulnerability());
            //SoundManager.Instance.PlaySound(HurtSound);
        } else {
            foreach (Behaviour component in Components) component.enabled = false;

            animator.SetTrigger("Death");
            StartCoroutine(FadeOutMaterial());
        }
    }

    public void TakeDamage(float damage, Vector2 knockback) {
        if (!Invulnerable) CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, StartingHealth);

        if (CurrentHealth > 0) {
            animator.SetTrigger("IsHit");
            RB.AddForce(knockback, ForceMode2D.Impulse);
            StartCoroutine(Invulnerability());
            //SoundManager.Instance.PlaySound(HurtSound);
        } else {
            foreach (Behaviour component in Components) component.enabled = false;

            animator.SetTrigger("Death");
        }
    }

    public void AddHealth(float heal) {
        CurrentHealth = Mathf.Clamp(CurrentHealth + heal, 0, StartingHealth);
    }

    public void Respawn() {
        AddHealth(StartingHealth);
        animator.ResetTrigger("Death");
        animator.Play("KnightIdle");
        StartCoroutine(Invulnerability());
        foreach (Behaviour component in Components) component.enabled = true;
    }

    private IEnumerator Invulnerability() {
        Invulnerable = true;
        Physics2D.IgnoreLayerCollision(3, 6, true);
        for (int i = 0; i < NumberOfFlashes; i++) {
            spriterenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(IFrameDuration / (NumberOfFlashes * 2));
            spriterenderer.color = Color.white;
            yield return new WaitForSeconds(IFrameDuration / (NumberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(3, 6, false);
        Invulnerable = false;
    }

    IEnumerator FadeOutMaterial() {
        for (float Fade = 1; Fade >= 0; Fade -= 0.075f) {
            ShaderMaterial.SetFloat("_Fade", Fade);
            yield return new WaitForSeconds(.05f);
        }
    }
}