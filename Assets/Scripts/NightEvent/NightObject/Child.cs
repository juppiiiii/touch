using UnityEngine;

public class Child : MonoBehaviour
{
    [SerializeField] private GameObject toddlerModel;
    [SerializeField] private GameObject teenModel;
    [SerializeField] private RuntimeAnimatorController toddlerController;
    [SerializeField] private RuntimeAnimatorController teenController;
    
    private Animator currentAnimator;
    private int currentWave;
    private bool isActive;

    private void Awake()
    {
        // 시작할 때 모든 모델 비활성화
        if (toddlerModel != null) toddlerModel.SetActive(false);
        if (teenModel != null) teenModel.SetActive(false);
    }

    public void Initialize(int wave)
    {
        currentWave = wave;
        UpdateChildState();
    }

    private void UpdateChildState()
    {
        if (toddlerModel == null || teenModel == null)
        {
            Debug.LogError("Child: 모델 참조가 설정되지 않았습니다!");
            return;
        }

        // 웨이브에 따라 모델 활성화
        bool isTeenager = currentWave > 2;
        toddlerModel.SetActive(!isTeenager);
        teenModel.SetActive(isTeenager);

        // 현재 활성화된 모델의 Animator 가져오기
        currentAnimator = isTeenager ? teenModel.GetComponent<Animator>() : toddlerModel.GetComponent<Animator>();
        
        // 애니메이터 컨트롤러 설정
        if (currentAnimator != null)
        {
            currentAnimator.runtimeAnimatorController = isTeenager ? teenController : toddlerController;
        }
    }

    public void SetActiveState(bool active)
    {
        isActive = active;
        if (toddlerModel != null) toddlerModel.SetActive(active && currentWave <= 2);
        if (teenModel != null) teenModel.SetActive(active && currentWave > 2);
    }

    // 애니메이션 재생을 위한 메서드들
    public void PlayGetupAnimation()
    {
        currentAnimator.SetTrigger("Getup");
    }

    public void PlayTossingAnimation()
    {
        currentAnimator.SetTrigger("Tossing");
    }

    public void PlaySleepingAnimation()
    {
        currentAnimator.SetTrigger("Sleeping");
    }

    // Toddler만 존재
    public void PlayRubbingEyesAnimation()
    {
        currentAnimator.SetTrigger("RubbingEyes");
    }

    public void PlayLyingDownAnimation()
    {
        currentAnimator.SetTrigger("LyingDown");
    }
}
