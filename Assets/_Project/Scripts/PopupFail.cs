using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupFail : PopupBase
{
    protected override void Start()
    {
       
    }

    public void SkipWithADS()
    {
        
        LevelData.SaveLevel(LevelManager.Instance.CurrentLevel + 1);
        PopupManager.Instance.ShowPopupNextGame();
        Destroy(gameObject);
    }
    public void Replay()
    {
      

        PopupManager.Instance.ShowPopupNextGame();
        Destroy(gameObject);
    }
}
