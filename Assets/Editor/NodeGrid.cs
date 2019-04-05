using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeGrid  {
    [SerializeField]
    public List<DialogueNode> dialogueNodes;
    [SerializeField]
    public List<Connection> connections;
    [SerializeField]
    public string saveddata;

}
