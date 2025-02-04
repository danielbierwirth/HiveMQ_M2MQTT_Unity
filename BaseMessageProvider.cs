using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BaseMessageProvider : MonoBehaviour
{
    public event Action<BaseMessageProvider, BaseMessage> OnMessageReceived;

    public bool IsInitialized { get; protected set; } = false;

    [SerializeField] private bool _isPaused;

    public bool IsPaused
    {
        get { return _isPaused; }
        protected set { _isPaused = value; }
    }

    public virtual Task InitializeAsync()
    {
        IsInitialized = true;
        return Task.CompletedTask;
    }

    public virtual void Pause()
    {
        IsPaused = true;
    }

    public virtual void Resume()
    {
        IsPaused = false;
    }

    protected void MessageReceived(BaseMessage msg)
    {
        OnMessageReceived?.Invoke(this, msg);
    }
}

