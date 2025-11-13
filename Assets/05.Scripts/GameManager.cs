using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 꼭 필요합니다!

public class GameManager : MonoBehaviour
{
       
   
    public static GameManager instance;

    [Header("UI 연결")]
    [Tooltip("점수를 표시할 TextMeshPro UI 요소")]
    public TextMeshProUGUI scoreText; // 점수 텍스트 UI

    private int currentScore = 0; // 현재 점수
 
    //싱글톤
    void Awake()
    {
        // --- 싱글톤 설정 ---
        
        if (instance == null)
        {
            instance = this;
        }
        
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 게임 시작 시 1회 호출됩니다.
    void Start()
    {
        // 게임 시작 시 점수를 0점으로 초기화하고 UI를 업데이트합니다.
        currentScore = 0;
        UpdateScoreText();
    }

    // 점수를 추가하는 공개 함수 (다른 스크립트에서 호출할 함수)
    public void AddScore(int pointsToAdd)
    {
        currentScore += pointsToAdd; // 점수 누적
        UpdateScoreText();       
       
    }

    // UI 텍스트를 업데이트하는 함수
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            // scoreText의 텍스트를 "SCORE: [현재점수]"로 변경
            scoreText.text = "SCORE: " + currentScore.ToString();
        }        
    }
}
