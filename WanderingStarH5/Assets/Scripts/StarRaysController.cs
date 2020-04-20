using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRaysController : StarController
{
    [SerializeField]
    private GameObject border;

    [SerializeField]
    private GameObject innerBorder;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float innerScale = 3;

    protected override void Update()
    {
        base.Update();

        innerBorder.SetActive(this.transform.localScale.x > innerScale);
        border.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
