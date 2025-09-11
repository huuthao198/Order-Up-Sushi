using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHomeManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DelayLoad());
    }

    IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(.3f);
        CutScene.Instance.Hide();
    }

    // Update is called once per frame
    public void OnBtnStartClick()
    {
        CutScene.Instance.Show(() =>
        {
            SceneManager.LoadScene("GamePlay");
        });
    }
}
