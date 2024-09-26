using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DVAH;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
         
        LoadingManager.Instant.SetMaxTimeLoading(7).Init(1, (checklistCondition)=>
        {  
            SceneManager.UnloadSceneAsync(0);
            
        });

        //
        LoadingManager.Instant.DoneConditionSelf(0,()=> AdBridge.Instant.IsAdLoaded(AD_TYPE.Aoa));

        AdBridge.Instant.ShowAd(AD_TYPE.MRecs);
    }
     
    
}
