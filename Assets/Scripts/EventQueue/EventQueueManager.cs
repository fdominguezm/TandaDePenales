using System.Collections.Generic;
using UnityEngine;

public class EventQueueManager : MonoBehaviour
{
    #region SINGLETON
    static public EventQueueManager Instance;

    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
    }
    #endregion

    private Queue<ICommand> _commands = new Queue<ICommand>();

    public void AddCommand(ICommand command) => _commands.Enqueue(command);

    void Update()
    {
        while (_commands.Count > 0)
            _commands.Dequeue()?.Execute();
    }
}