using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue", order = 1)]  
public class Dialogue : ScriptableObject {


    public string CharacterName;

    [TextArea(20,0)]
    public string Content;

    public List<Dialogue> NextLine;

    public bool StartPoint;


}
