using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    private const float DEATH_TIME = 0.5f;
    private const float BIRTH_TIME = 0.25f;
    private const float EAT_DELAY = 0.2f;
    private const float DEATH_SCALE = 0.5f;
    private const int BIRTH_ROTATION_CIRCLES = 1;

    [SerializeField]
    private SpriteRenderer eatGlow;

    [SerializeField]
    private SpriteRenderer spiralRays;

    public Action<StarController> onDead;
    public Action<StarController> onBorn;

    public float chance;
    public float density;

    public bool IsDying
    {
        get; private set;
    }

    public float radius;
    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            radius = value;
            this.transform.localScale = new Vector3(radius, radius, 1);
        }
    }

    public float Mass
    {
        get
        {
            return density * Mathf.PI * Mathf.Pow(radius, 3) * 4 / 3;
        }
    }

    public void Death(StarController star = null)
    {
        IsDying = true;
        StartCoroutine(DeathAnimation(star));
    }

    private IEnumerator DeathAnimation(StarController star)
    {
        float animTime = DEATH_TIME;
        float startScale = this.transform.localScale.x;

        while(animTime >= 0)
        {
            float newScale = Mathf.Lerp(0, startScale, animTime / DEATH_TIME);
            this.transform.localScale = new Vector3(newScale, newScale, 1);
            animTime -= Time.deltaTime;
            spiralRays.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, animTime / DEATH_TIME));
            this.transform.Rotate(Vector3.forward, animTime * 500 * Time.deltaTime / DEATH_TIME);

            if(star != null)
                this.transform.position = Vector3.Lerp(star.transform.position, this.transform.position, animTime / DEATH_TIME);

            yield return new WaitForEndOfFrame();
        }

        onDead?.Invoke(this);
        Destroy(this.gameObject);
    }

    public void Init(Vector3 position, float minRadius, float maxRadius)
    {
        this.transform.position = position;
        Radius = UnityEngine.Random.Range(minRadius, maxRadius);
        eatGlow.color = new Color(eatGlow.color.r, eatGlow.color.g, eatGlow.color.b, 0);

        StartCoroutine(InitAnimation());
    }

    public void Init(Vector3 position, float mass)
    {
        this.transform.position = position;
        Radius = Mathf.Pow(mass * 4 / (3 * density), 1f / 3f);
        eatGlow.color = new Color(eatGlow.color.r, eatGlow.color.g, eatGlow.color.b, 0);

        StartCoroutine(InitAnimation());
    }

    private IEnumerator InitAnimation()
    {
        float animTime = BIRTH_TIME;
        float targetScale = radius;
        float rotationSpeed = (float)BIRTH_ROTATION_CIRCLES * 360 / BIRTH_TIME;

        while (animTime >= 0)
        {
            float newScale = Mathf.Lerp(targetScale, 0, animTime / BIRTH_TIME);
            this.transform.localScale = new Vector3(newScale, newScale, 1);
            animTime -= Time.deltaTime;
            spiralRays.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, animTime / BIRTH_TIME));
            this.transform.Rotate(Vector3.forward, animTime * rotationSpeed * Time.deltaTime / BIRTH_TIME);

            yield return new WaitForEndOfFrame();
        }

        this.transform.rotation = Quaternion.identity;

        //this.transform.Rotate(Vector3.forward, 45);

        onBorn?.Invoke(this);
    }

    protected virtual void Update()
    {
        if (IsDying)
            return;

        UpdateM(-Time.deltaTime * GameManager.MASS_LOSE * Mass);
        if (eatGlow.color.a > 0)
            eatGlow.color = new Color(eatGlow.color.r, eatGlow.color.g, eatGlow.color.b, eatGlow.color.a - Time.deltaTime);
    }

    private void UpdateM(float deltaM)
    {
        float newM = Mass + deltaM;

        float newRadius = Mathf.Pow(newM * 3 / (4 * density * Mathf.PI), 1f / 3);

        if (newRadius <= DEATH_SCALE || float.IsNaN(newRadius))
            Death();

        Radius = newRadius;
    }

    public void ApplyForce(StarController star, float distance, Vector3 direction)
    {
        if (star.IsDying || IsDying)
            return;

        float force = star.Mass / (Mathf.Pow(distance, 2));
        Vector3 newPosition = this.transform.position + direction * force * GameManager.GRAVITY;

        if ((star.transform.position - newPosition).magnitude > (star.transform.position - this.transform.position).magnitude)
            this.transform.position = star.transform.position;
        else
            this.transform.position = newPosition;

        if (distance < radius / 4 && this.radius > star.radius)
        {
            star.Death(this);
            StartCoroutine(Eat(star));
            return;
        }
    }

    protected virtual IEnumerator Eat(StarController star)
    {
        yield return new WaitForSeconds(DEATH_TIME - EAT_DELAY);

        eatGlow.color = new Color(eatGlow.color.r, eatGlow.color.g, eatGlow.color.b, 1);
        UpdateM(star.Mass);
    }
}
