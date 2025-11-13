using UnityEngine;


public class Stage : MonoBehaviour
{
    // 게임이 이미 멈췄는지 확인하기 위한 변수
    bool isGameOver = false;

    // 충돌이 감지되는 순간 호출되는 함수
    public void OnCollisionEnter(Collision collision)
    {
        // 1. 이미 게임 오버(일시 정지) 상태라면, 더 이상 아무것도 하지 않습니다.
        if (isGameOver) return;

        // 2. 충돌한 오브젝트의 태그가 "Object"인지 확인합니다.
        if (collision.gameObject.CompareTag("Object"))
        {
            // 3. "Object" 태그가 맞다면 게임 오버 로직을 실행합니다.
            GameOver();
        }
    }

    public void GameOver()
    {
        // 4. 게임 오버 상태로 변경 (이 함수가 두 번 이상 실행되는 것을 방지)
        isGameOver = true;

        // 5. 콘솔에 로그를 남깁니다.
        Debug.Log("GAME OVER!");

        // 6. 게임의 시간 흐름을 0배속(정지)으로 설정합니다.
        //    (모든 물리 효과, 애니메이션, Update 함수 등이 멈추게 됩니다.)
        Time.timeScale = 0f;
    }
}