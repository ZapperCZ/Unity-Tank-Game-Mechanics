using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wheel : MonoBehaviour
{
    [SerializeField] public WheelType WheelType;
    [SerializeField] public List<System.Type> test;
    [SerializeField] SO_WheelComponentPrefabs WheelComponentSO_Local;       //The SO that the user can interact with; used to change the global SO
    [SerializeField] bool keepExistingComponents = true;
    static SO_WheelComponentPrefabs WheelComponentSO = null;    //The global SO that is the same across all script instances
    static bool SOIsSet = false;                                //Global check to prevent setting of the global SO multiple times

    Component[] ComponentsToCopy;   
    Component[] ComponentsToDelete;     //The components present on the Game Object
    private void Awake()
    {
        test = new List<System.Type>();
        if (Application.isPlaying)
            return;
        if(SOIsSet == false)    //Get Component SO if it hasn't been set by another script instance
        {
            WheelComponentSO = WheelComponentSO_Local;
            SOIsSet = true;
        }
        WheelComponentSO_Local = WheelComponentSO;

        ComponentsToCopy = WheelComponentSO.WheelComponentPrefabArray[(int)WheelType].GetComponents<Component>();
        ComponentsToDelete = this.GetComponents<Component>();

        test.Add(new Component().GetType());

        if (!keepExistingComponents)
        {
            DeleteComponents();
        }
        CopyComponents();
    }
    void CopyComponents()
    {
        Component tempComp;
        foreach(Component comp in ComponentsToCopy)
        {
            if(comp.GetType() == transform.GetType())
            gameObject.AddComponent(comp.GetType());
            tempComp = gameObject.GetComponent(comp.GetType());
            tempComp = comp;
        }
    }
    void DeleteComponents()
    {
        for (int i = 0; i < ComponentsToDelete.Length; i++)
        {
            if (ComponentsToDelete[i].GetType() == transform.GetType())
                i++;
            DestroyImmediate(ComponentsToDelete[i]);
        }
    }
}
