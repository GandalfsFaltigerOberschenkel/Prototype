using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
[System.Serializable]
public class CutSceneImageLayer
{
    public GameObject image;
    public float scrollSpeed;
}
[System.Serializable]
public class CutSceneStage
{
    [SerializeField]
    public CutSceneImageLayer[] cutSceneImageLayers;
    public float duration;
    public float delay;
    public float transitionDuration;
   

}
public class CutSceneManager : MonoBehaviour
{
    [SerializeField] CutSceneStage[] cutSceneStages;
    private
   Stack<CutSceneStage> cutSceneStageStack;
    bool isPlaying = true;
    public string nextScene;
    void LoadCutScenesIntoStack()
    {
        cutSceneStageStack = new Stack<CutSceneStage>();
        foreach (CutSceneStage cutSceneStage in cutSceneStages.Reverse())
        {
            cutSceneStageStack.Push(cutSceneStage);
        }
    }
    public void Start()
    {
        LoadCutScenesIntoStack();
        StartCutScene();
    }
    public void StartCutScene()
    {
        StartCoroutine(PlayCutScene());
    }
    public void Update()
    {
        if (isPlaying == false)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
    public IEnumerator PlayCutScene()
    {

        isPlaying = true;
        CutSceneStage currentCutscene = PullCutsceneFromStack();
        if (currentCutscene == null)
        {
            isPlaying = false;
            yield break;
        }
        foreach (CutSceneImageLayer cutSceneImageLayer in currentCutscene.cutSceneImageLayers)
        {
            cutSceneImageLayer.image.SetActive(true);
        }
        float counter = 0;
        while (counter < currentCutscene.duration)
        {
            //scroll images based on scroll speed and duration
            foreach(CutSceneImageLayer cutSceneImageLayer in currentCutscene.cutSceneImageLayers)
            {
                cutSceneImageLayer.image.transform.position += new Vector3(cutSceneImageLayer.scrollSpeed * Time.deltaTime, 0, 0);
            }

            counter += Time.deltaTime;
            yield return null;
        }
        //Fade out images
        float opacity = 1;
        while (opacity > 0)
        {
            foreach (CutSceneImageLayer cutSceneImageLayer in currentCutscene.cutSceneImageLayers)
            {
                Color color = cutSceneImageLayer.image.GetComponent<SpriteRenderer>().color;
                color.a = opacity;
                cutSceneImageLayer.image.GetComponent<SpriteRenderer>().color = color;
            }
            opacity -= Time.deltaTime / currentCutscene.transitionDuration;
            yield return null;
        }
        yield return new WaitForSeconds(currentCutscene.delay);
        StartCoroutine(PlayCutScene());
    }

    public CutSceneStage PullCutsceneFromStack()
    {
        if (cutSceneStageStack.Count > 0)
        {
            return cutSceneStageStack.Pop();
        }
        return null;
    }
}
