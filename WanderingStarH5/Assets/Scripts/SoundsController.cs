using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour
{
    public enum Type
    {
        Click = 0,
        Death,
        Eat
    }

    [SerializeField]
    private AudioClip[] sounds;

    public void PlaySound(Type soundType)
    {
        this.GetComponent<AudioSource>().PlayOneShot(sounds[(int)soundType]);
    }
}
