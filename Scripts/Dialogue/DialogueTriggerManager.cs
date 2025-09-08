using UnityEngine;
using cherrydev;
using System.Collections.Generic;

public class DialogueTriggerManager : MonoBehaviour
{
    public static DialogueTriggerManager Instance;
    [SerializeField] private DialogBehaviour dialogBehaviour;
    //[SerializeField] private DialogNodeGraph dialogGraph; // change this for a list or idk 
    [SerializeField] private List<DialogNodeGraph> dialogGraph = new(); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerDialogue(string text)
    {
        Debug.Log($"[DIALOGUE]: {text}");
        dialogGraph.ForEach(graph => {
            if (graph.name == text)
            {
                dialogBehaviour.StartDialog(graph);
                return;
            }
        });

    }
}