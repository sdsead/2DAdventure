using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class InitialLoad : MonoBehaviour
{
    public AssetReference persistentScene;

    private void Awake()
    {
        Addressables.LoadSceneAsync(persistentScene);
    }
}