using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorGamesLink : MonoBehaviour
{
    public void LinkToArmorGames()
    {
        //Application.OpenURL("http://armor.ag/MoreGames");
        Application.ExternalEval("window.open('http://armor.ag/MoreGames','_blank')");
    }
}
