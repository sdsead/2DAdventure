using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour,ISaveable
{
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    
    
    
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    private GameSceneSO sceneToLoad;
    private GameSceneSO currentLoadScene;
    private Vector3 positionToGo;
    private bool isLoading;
    private bool fadeScreen;
    public float fadeDuration;


    
    [Header("广播")]
    public FadeEventSO fadeEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public SceneLoadEventSO sceneUnloadedEvent;


    private void Start()
    {
        loadEventSO.RaiseLoadRequestEvent(menuScene,menuPosition,true);
        
       
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += newGeme;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
        
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= newGeme;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;
        
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    
    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }
    
    /// <summary>
    /// 场景加载事件请求
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if(isLoading)
            return;
        
        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        
        if(currentLoadScene!=null) 
            StartCoroutine(UnLoadPreviousScene());
        else
            LoadNewScene();
        
    }

    
    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //变黑
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);
        //广播事件调整血条显示
        sceneUnloadedEvent.RaiseLoadRequestEvent(sceneToLoad,positionToGo,true);
        
        yield return currentLoadScene.sceneReference.UnLoadScene();
        
        //关闭人物
        playerTrans.gameObject.SetActive(false);
        //加载新场景
        LoadNewScene();
    }

    private void newGeme()
    {
        sceneToLoad = firstLoadScene;
       
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad,firstPosition,true);
    }
    
    
    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    /// <summary>
    /// 场景加载完成
    /// </summary>
    /// <param name="obj"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadScene = sceneToLoad;

        playerTrans.position = positionToGo;
        playerTrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;
        if(currentLoadScene.sceneType == SceneType.Location)
            afterSceneLoadedEvent?.OnRaiseEvent();
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        
        if (data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();
            sceneToLoad = data.GetSavedScene();

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
    }
}
