using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public bool IsLoading { get; private set; }
    public string PendingPortalId { get; private set; }

    public void LoadScene(string sceneName, string portalId = null)
    {
        if (IsLoading)
        {
            Debug.Log("[SCeneLoadManager] ::: already Loading ...");
            return;
        }
        PendingPortalId = portalId;

        StartCoroutine(CoLoadSceneWithFade(sceneName));
    }

    private IEnumerator CoLoadSceneWithFade(string sceneName)
    {
        IsLoading = true;

        bool isFadeOutComplete = false;
        Managers.Instance.UI.FadeOut(() => isFadeOutComplete = true);

        while (isFadeOutComplete == false)
            yield return null;
        
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;

        Managers.Instance.UI.FadeIn();

        IsLoading = false;
    }
}
