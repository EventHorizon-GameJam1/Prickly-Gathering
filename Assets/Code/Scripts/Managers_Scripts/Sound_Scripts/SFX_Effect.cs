using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_Effect : MonoBehaviour
{
    [SerializeField] private AudioSource Source;

    private void OnEnable()
    {
        Source = GetComponent<AudioSource>();
    }

    public void PlaySound(Vector3 pos, AudioClip clip)
    {
        transform.position = pos;
        Source.clip = clip;
        Source.Play();
        StartCoroutine(EffectIEnum(clip.length));
    }

    private IEnumerator EffectIEnum(float clipLenght)
    {
        yield return new WaitForSeconds(clipLenght);
        this.gameObject.SetActive(false);
    }
}
