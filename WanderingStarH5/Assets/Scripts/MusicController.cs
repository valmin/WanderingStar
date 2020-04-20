using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioClip menuWind;

    [SerializeField]
    private AudioClip[] samples;

    [SerializeField]
    private AudioClip gameMusic;

    private void Start()
    {
        EndGame();
    }

    public void StartGame()
    {
        StopCoroutine(Samples());
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().clip = gameMusic;
        this.GetComponent<AudioSource>().Play();
    }

    public void EndGame()
    {
        this.GetComponent<AudioSource>().clip = menuWind;
        this.GetComponent<AudioSource>().Play();

        StartCoroutine(Samples());
    }

    private IEnumerator Samples()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(4, 7));
            this.GetComponent<AudioSource>().PlayOneShot(samples[Random.Range(0, samples.Length)]);
        }
    }
}
