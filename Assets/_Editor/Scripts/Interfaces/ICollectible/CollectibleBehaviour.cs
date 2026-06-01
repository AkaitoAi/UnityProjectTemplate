using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public enum CollectibleType
{
    Coin,
    Gem
}

[RequireComponent(typeof(AudioSource))]
public class CollectibleBehaviour : MonoBehaviour
{
    [SerializeField] private CollectibleData coin;
    [SerializeField] private CollectibleData gem;

    private AudioSource _audioSource;

    private Dictionary<CollectibleType, CollectibleData> collectibleTexts;

    [Serializable]
    public struct CollectibleData
    {
        public Text text;
        public AudioClip clip;
    }

    private void Start()
    {
        collectibleTexts = new Dictionary<CollectibleType, CollectibleData>()
        {
            { CollectibleType.Coin, coin },
            { CollectibleType.Gem, gem }
        };

        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
    }

    private void CollectiblesCollected(int valueForIncrement, CollectibleType item)
    {
        Debug.Log($"Increasing amount {valueForIncrement} of {item}");

        if (collectibleTexts.TryGetValue(item, out CollectibleData data))
        {
            data.text.text = $"{item.ToString()}: {valueForIncrement}";

            _audioSource.clip = data.clip;
            _audioSource.pitch += UnityEngine.Random.Range(-.05f, .05f);
            _audioSource.Play();
        }
    }

    private void OnEnable()
    {
        Collectibles.OnCollectiblesCollected += CollectiblesCollected;
    }

    private void OnDisable()
    {
        Collectibles.OnCollectiblesCollected -= CollectiblesCollected;
    }
}
