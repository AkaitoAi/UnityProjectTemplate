public interface IEvent { }


// Demo
//public struct TestEvent : IEvent { }

//public struct PlayerEvent : IEvent {
//    public int health;
//    public int mana;
//}

//public struct PlayerEvent : IEvent
//{
//    public int health;
//    public int mana;
//}

//EventBinding<PlayerEvent> playerEventBinding;

//void OnEnable()
//{
//    playerEventBinding = new EventBinding<PlayerEvent>(HandlePlayerEvent);
//    EventBus<PlayerEvent>.Register(playerEventBinding);

//    // Can Add or Remove Actions to/from the EventBinding
//}

//void OnDisable()
//{
//    EventBus<PlayerEvent>.Deregister(playerEventBinding);
//}

//void Start()
//{
//    EventBus<PlayerEvent>.Raise(new PlayerEvent
//    {
//        health = healthComponent.GetHealth(),
//        mana = manaComponent.GetMana()
//    });
//}

//void HandlePlayerEvent(PlayerEvent playerEvent)
//{
//    Debug.Log($"Player event received! Health: {playerEvent.health}, Mana: {playerEvent.mana}");
//}