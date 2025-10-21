using UnityEngine;
using Universal;

public class InputHolder : Singleton<InputHolder>
{
    public InputActions actions { get; private set; }
    
    internal override void Awake()
    {
        base.Awake();
        if(inst != this) return;
        
            actions = new InputActions();
            actions.Enable();
    }
    
}