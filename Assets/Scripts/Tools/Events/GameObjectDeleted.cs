using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDeleted : GameEvent
{
    public GameObject DeletedGameObject;
    public GameObjectDeleted(GameObject DeletedGameObject)
    {
        this.DeletedGameObject = DeletedGameObject;
    }
}
