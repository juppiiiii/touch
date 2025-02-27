using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MissionScript : MonoBehaviour
{
    public VideoPlayer video;      // 재생할 비디오
    public Button playButton;      // 영상 시작 버튼
    public Button endButton;       // 미션 종료 버튼
    public RawImage videoDisplay;  // 비디오를 표시할 RawImage
    public Text[] texts;           // 페이드 인할 텍스트 배열
    public RenderTexture renderTexture; // 비디오 출력용 RenderTexture
    public float fadeDuration = 1.5f;  // 페이드 인 지속 시간
    public float moveDuration = 1.5f;  // 이동 지속 시간
    public float endMoveSpeed = 2.0f;  // 위로 이동 속도
    public float endFadeDuration = 1.5f; // 사라지는 시간
    public float autoEndDelay = 3.0f; // 영상 종료 후 자동 종료 대기시간

    private CanvasGroup videoCanvasGroup; // RawImage의 투명도 조절을 위한 CanvasGroup
    private RectTransform videoTransform; // RawImage의 위치 조절을 위한 RectTransform

    private void Start()
    {
        playButton.onClick.AddListener(StartMission);
        endButton.onClick.AddListener(EndMission);
        video.loopPointReached += OnVideoFinished; // 영상 끝나면 이벤트 실행

        videoCanvasGroup = videoDisplay.GetComponent<CanvasGroup>();
        if (videoCanvasGroup == null)
        {
            videoCanvasGroup = videoDisplay.gameObject.AddComponent<CanvasGroup>();
        }

        videoTransform = videoDisplay.GetComponent<RectTransform>();

        // **비디오 시작 전: x 좌표를 1080으로 이동 & 투명하게 설정**
        videoTransform.anchoredPosition = new Vector2(1080, videoTransform.anchoredPosition.y);
        videoCanvasGroup.alpha = 0;
        videoDisplay.gameObject.SetActive(true); // **비활성화하면 Prepare()가 안 되므로 활성화 유지**

        ResetRenderTexture();

        // **비디오 준비 (첫 프레임 로드)**
        video.Prepare();
        video.prepareCompleted += OnVideoPrepared;

        foreach (Text text in texts)
        {
            Color c = text.color;
            c.a = 0;
            text.color = c;
        }
    }

    // **비디오가 준비되면 첫 프레임 로드**
    private void OnVideoPrepared(VideoPlayer vp)
    {
        video.frame = 0; // **첫 프레임으로 설정**
        video.Pause();   // **비디오를 첫 프레임에서 멈춤**
    }

    public void StartMission()
    {
        StartCoroutine(MoveInAndPlayVideo());
    }

    private IEnumerator MoveInAndPlayVideo()
    {
        float elapsedTime = 0;
        videoTransform.anchoredPosition = new Vector2(0, videoTransform.anchoredPosition.y);
        Vector2 targetPos = new Vector2(0, videoTransform.anchoredPosition.y);

        while (elapsedTime < moveDuration)
        {
            videoCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        videoTransform.anchoredPosition = targetPos;
        videoCanvasGroup.alpha = 1;

        video.Play(); // **비디오 재생**
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        video.frame = 0; // **첫 프레임으로 되돌리기**
        video.Play(); // 첫 프레임을 유지하기 위해 다시 재생
        video.Pause(); // 첫 프레임에서 멈추기

        StartCoroutine(StartFadeInTexts());
    }

    private IEnumerator StartFadeInTexts()
    {
        foreach (Text text in texts)
        {
            yield return StartCoroutine(FadeIn(text));
        }
    }

    private IEnumerator FadeIn(Text text)
    {
        float elapsedTime = 0;
        Color color = text.color;
        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            text.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        text.color = color;
    }

    public void EndMission()
    {
        StartCoroutine(MoveOutAndHide());
    }

    private IEnumerator MoveOutAndHide()
    {
        float elapsedTime = 0;
        RectTransform rectTransform = GetComponent<RectTransform>(); // **현재 오브젝트의 위치 변경**
        
        Vector2 startPos = rectTransform.anchoredPosition; // 현재 위치
        Vector2 targetPos = new Vector2(startPos.x, startPos.y + 2000); // 위로 500 이동

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>(); // 알파값 조절용
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>(); // 없으면 추가
        }

        while (elapsedTime < moveDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        canvasGroup.alpha = 0;
        gameObject.SetActive(false); // **스크립트가 붙어있는 오브젝트 비활성화**
    }

    private void ResetRenderTexture()
    {
        if (renderTexture != null)
        {
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }
    }
}
