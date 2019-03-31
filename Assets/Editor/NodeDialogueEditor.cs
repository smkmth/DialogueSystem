using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeDialogueEditor : EditorWindow
{
    public bool groupEnabled;
    public List<DialogueNode> dialogueNodes;
    public List<Connection> connections;

    public GUIStyle nodeStyle;
    public GUIStyle selectedNodeStyle;

    public GUIStyle inPointStyle;
    public GUIStyle outPointStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;






    [MenuItem("Window/Dialogue Window")]
    private static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(NodeDialogueEditor));

    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);


        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

    }

    void OnGUI()
    {
        DrawGrid(20.0f, 0.2f, Color.gray);
        DrawGrid(100.0f, 0.4f, Color.gray);
        DrawNodes();
        DrawConnections();
        DrawConnectionLine(Event.current);
        if (dialogueNodes != null)
        {
            CheckNodes();
        }


        ProcessNodeEvents(Event.current);

        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();

    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDiv = Mathf.CeilToInt(position.width/gridSpacing);
        int heightDiv = Mathf.CeilToInt(position.height/gridSpacing);

        Handles.BeginGUI();
        Handles.color= new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
        for (int i = 0; i < widthDiv; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDiv; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }


    private void DrawNodes()
    {
        if (dialogueNodes != null)
        {
            for (int i = 0; i <dialogueNodes.Count; i++)
            {
                dialogueNodes[i].Draw();
            }
        }
    }
    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i <connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left *50.0f,
                Color.white,
                null,
                2f
                );

            GUI.changed = true;
        }
        else if(selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
               selectedOutPoint.rect.center,
               e.mousePosition,
               selectedOutPoint.rect.center - Vector2.left * 50f,
               e.mousePosition + Vector2.left * 50f,
               Color.white,
               null,
               2f
           );

            GUI.changed = true;
        }


    }

    private void CheckNodes()
    {
        List<DialogueNode> uncheckedNodes = new List<DialogueNode>();
        foreach (DialogueNode node in dialogueNodes)
        {
            if (node.DialogueChecked == false)
            {
                if (node.dialogue != null)
                {
                    if (node.dialogue.NextLine.Count > 0)
                    {
                        uncheckedNodes.Add(node);

                    }
                }
            }
        }
        if (uncheckedNodes.Count > 0)
        {
            foreach (DialogueNode node in uncheckedNodes)
            {

                foreach (Dialogue dialogue in node.dialogue.NextLine)
                {
                    if (!NodeExists(dialogue))
                    {
                        DialogueNode connectingnode = AddNode(node.rect.center + Vector2.right * 60.0f);
                        connectingnode.dialogue = dialogue;
                        CreateConnection(node, connectingnode);
                        connectingnode.DialogueChecked = true;

                    }


                }
            }
        }
    
    }

    public bool NodeExists(DialogueNode newnode)
    {
        if (newnode.dialogue == null)
        {
            return false;
        }

        foreach(DialogueNode node in dialogueNodes)
        {
            if (node.dialogue != null && node.dialogue.Equals(newnode.dialogue))
            {
                return true;

            }
        }
        return false;
    }

    public bool NodeExists(Dialogue newnode)
    {
        if (newnode == null)
        {
            return false;
        }

        foreach (DialogueNode node in dialogueNodes)
        {
            if (node.dialogue != null && node.dialogue.Equals(newnode))
            {
                return true;

            }
        }
        return false;
    }


    private void ProcessNodeEvents(Event e)
    {
        if (dialogueNodes != null)
        {
            for(int i = dialogueNodes.Count -1;  i>=0; i--)
            {
                bool guiChanged = dialogueNodes[i].ProcessEvents(e);
                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);

                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);

                }
                break;

                
        }
    }


    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (dialogueNodes != null)
        {
            for (int i = 0; i < dialogueNodes.Count; i++)
            {
                dialogueNodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (dialogueNodes == null)
        {
            dialogueNodes = new List<DialogueNode>();
        }

        dialogueNodes.Add(new DialogueNode(mousePosition, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));

    }

    private DialogueNode AddNode(Vector2 pos)
    {
        if (dialogueNodes == null)
        {
            dialogueNodes = new List<DialogueNode>();
        }

        DialogueNode node = new DialogueNode(pos, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        dialogueNodes.Add(node);
        return node;
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;
        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();

            }
            else
            {
                ClearConnectionSelection();
            }
        }

    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        if (connection.outPoint.node.dialogue != null && connection.inPoint.node.dialogue != null)
        {
            connection.outPoint.node.dialogue.NextLine.Remove(connection.inPoint.node.dialogue);
        }
        connections.Remove(connection);

    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        if (selectedInPoint.node.dialogue != null && selectedInPoint.node.dialogue != null)
        {
            selectedOutPoint.node.dialogue.NextLine.Add(selectedInPoint.node.dialogue);

        }
    }

    private void CreateConnection(DialogueNode innode, DialogueNode outnode)
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(innode.outPoint, outnode.inPoint, OnClickRemoveConnection));
      

    }

    private void DestroyConnection(Connection connection)
    {
        if (connection.outPoint.node.dialogue != null && connection.inPoint.node.dialogue != null)
        {
            connection.outPoint.node.dialogue.NextLine.Remove(connection.inPoint.node.dialogue);
        }
        connections.Remove(connection);


    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

   
    private void OnClickRemoveNode(DialogueNode node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                DestroyConnection(connectionsToRemove[i]);
                //connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        dialogueNodes.Remove(node);
    }



}
