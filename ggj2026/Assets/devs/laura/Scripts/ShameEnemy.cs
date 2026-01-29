using UnityEngine;

public class ShameEnemy : EnemyBase
{
    public enum State
    {
        Normal,
        Hidden,
    }

    public State currentState;
    
    void Start()
    {
        base.Start();
        currentState = State.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        HandleSwitch();
        DoState();

    }

    private void HandleSwitch()
    {
        switch (currentState)
        {
            case State.Normal:
                //switchtohidden
                break;
            case State.Hidden:
                //switchtonormal
                break;
        }
    }

    private void DoState()
    {
        switch (currentState)
        {
            case State.Normal:
                Debug.Log("Normal");
                break;
            case State.Hidden:
                Debug.Log("Hidden");
                break;
        }
    }

    public void Hide()
    {
        currentState = State.Hidden;
    }

    public void Show()
    {
        currentState = State.Normal;
    }
}
