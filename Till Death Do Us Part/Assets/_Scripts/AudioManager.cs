using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public List<AudioGroup> sfx;
    public List<AudioGroup> dialogue;
    public List<AudioGroup> bgm;

    [Header("References")]
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioSource dialogueAudioSource;
    [SerializeField] AudioSource bgmAudioSource;

    [Header("Prefabs")]
    [SerializeField] GameObject sfxAudioSourcePrefab;

    public void PlaySFXOneShot(string sfxAudioGroupName)
    {
        //Find correct Audio Clip
        AudioGroup audioGroup = sfx.Find(x => x.groupName == sfxAudioGroupName);
        if (audioGroup == null)
        {
            Debug.LogError($"Audio Group {sfxAudioGroupName} not found.");
            return;
        }
        AudioClip selectedAudioClip = audioGroup.GetRandomAudioClip();
        if (!sfxAudioSource.isPlaying)
        {
            sfxAudioSource.PlayOneShot(selectedAudioClip);
            Debug.Log($"Playing SFX: {sfxAudioGroupName} - {selectedAudioClip.name}");
        }
        else
        {
            //Start new coroutine to temporarily create new Audio Source
            StartCoroutine(CreateTemporarySfxAudioSource(selectedAudioClip));
        }
    }

    public void PlayDialogue(string dialogueId)
    {
        //Find correct Audio Clip
        AudioGroup audioGroup = dialogue.Find(x => x.groupName == dialogueId);
        if (audioGroup == null)
        {
            Debug.LogError($"Dialogue {dialogueId} not found.");
            return;
        }
        AudioClip selectedAudioClip = audioGroup.GetAudioClip();

        dialogueAudioSource.Stop();
        dialogueAudioSource.PlayOneShot(selectedAudioClip);
        Debug.Log($"Playing Dialogue: {dialogueId} - {selectedAudioClip.name}");
    }

    public void PlayBGM(string BgmGroupId)
    {
        //Find correct Audio Clip
        AudioGroup audioGroup = bgm.Find(x => x.groupName == BgmGroupId);
        if (audioGroup == null)
        {
            Debug.LogError($"BGM {BgmGroupId} not found.");
            return;
        }
        AudioClip selectedAudioClip = audioGroup.GetRandomAudioClip();

        bgmAudioSource.Stop();
        bgmAudioSource.clip = selectedAudioClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
        Debug.Log($"Playing BGM: {BgmGroupId} - {selectedAudioClip.name}");
    }

    IEnumerator CreateTemporarySfxAudioSource(AudioClip audioClip)
    {
        GameObject temporarySfxAudioSourceObject = Instantiate(sfxAudioSourcePrefab, sfxAudioSource.transform);
        AudioSource temporarySfxAudioSource = temporarySfxAudioSourceObject.GetComponent<AudioSource>();

        temporarySfxAudioSource.PlayOneShot(audioClip);
        while (temporarySfxAudioSource.isPlaying)
        {
            yield return null;
        }
        Destroy(temporarySfxAudioSourceObject);
    }
}

[Serializable]
public class AudioGroup
{
    public string groupName;
    public int Count => audioClips.Count;
    [SerializeField] List<AudioClip> audioClips;

    public AudioClip GetAudioClip()
    {
        if (Count < 1)
        {
            Debug.LogError($"AudioGroup {groupName} does not contain any AudioClips.");
            return null;
        }
        else
        {
            return audioClips.FirstOrDefault();
        }
    }

    public AudioClip GetRandomAudioClip()
    {
        if (Count < 1)
        {
            Debug.LogError($"AudioGroup {groupName} does not contain any AudioClips.");
            return null;
        }
        else if (Count == 1)
        {
            return audioClips.FirstOrDefault();
        }
        else
        {
            var random = UnityEngine.Random.Range(0, Count);
            return audioClips[random];
        }
    }
}