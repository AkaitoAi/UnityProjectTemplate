using System;
using UnityEngine;

public class Collectibles : MonoBehaviour, ICollectible
{
    public int IncrementValue = 0;
    public CollectibleType ItemType;
    public static event Action<int, CollectibleType> OnCollectiblesCollected;
    
    public void OnCollect()
    {
        Debug.Log($"You Have Collected {ItemType}");
        OnCollectiblesCollected?.Invoke(IncrementValue, ItemType);
        Destroy(gameObject);
    }

    //TODO Use Where UI is referenced
    //private void CollectiblesCollected(int valueForIncrement, CollectibleType item)
    //{
    //    Debug.Log($"Increasing amount {valueForIncrement} of {item}");
    //    if (item == CollectibleType.Coin)
    //    {
    //        //CoinText.text = "Coins: " + valueForIncrement;
    //    }
    //    if (item == CollectibleType.Gem)
    //    {
    //        //GemText.text = "Gems: " + valueForIncrement;
    //    }
    //}
}
