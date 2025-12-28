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
            return;

        PendingPortalId = portalId;

        StartCoroutine(CoLoadScene(sceneName));
    }

    private IEnumerator CoLoadScene(string sceneName)
    {
        IsLoading = true;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;

        IsLoading = false;
    }
}
