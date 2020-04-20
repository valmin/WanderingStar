using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStarController : StarController
{
    public Action onEat;

    [SerializeField]
    private float sensivity = 0.025f;

    protected override void Update()
    {
        base.Update();

        if (IsDying)
            return;

        ApplyInput();
    }

    private void ApplyInput()
    {
        this.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * sensivity;
    }

    protected override IEnumerator Eat(StarController star)
    {
        yield return base.Eat(star);

        onEat?.Invoke();
    }
}
