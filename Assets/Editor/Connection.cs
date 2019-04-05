using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class Connection
{
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;
    public Action<Connection> OnClickRemoveConnection;

    public Connection(ConnectionPoint _inPoint, ConnectionPoint _outPoint, Action<Connection> _OnClickRemoveConnection)
    {
        this.inPoint = _inPoint;
        this.outPoint = _outPoint;
        this.OnClickRemoveConnection = _OnClickRemoveConnection;
    }
    
    public void Draw()
    {
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50.0f,
            inPoint.rect.center - Vector2.left * 50.0f,
            Color.white,
            null,
            2.0f
            );
        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }

        }
        
    }


}
