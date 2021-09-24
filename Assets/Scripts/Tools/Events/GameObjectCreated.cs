using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectCreated : GameEvent
{
    public GameObject CreatedGameObject;
    public GameObjectCreated(GameObject CreatedGameObject)
    {
        this.CreatedGameObject = CreatedGameObject;
    }
}
