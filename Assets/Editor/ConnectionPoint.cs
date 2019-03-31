using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ConnectionPointType { In, Out}

public class ConnectionPoint  {

    public Rect rect;

    public ConnectionPointType type;

    public DialogueNode node;

    public GUIStyle style;

    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(DialogueNode _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _OnClickConnectionPoint)
    {
        this.node = _node;
        this.type = _type;
        this.style = _style;
        this.OnClickConnectionPoint = _OnClickConnectionPoint;
        rect = new Rect(0.0f, 0.0f, 10.0f, 20.0f);
    }

    public void Draw()
    {
        rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

        switch (type)
        {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
        }

        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}
