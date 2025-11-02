using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("카메라가 바라볼 대상(바구니의 중심)")]
    public Transform target;

    [Tooltip("목표물로부터의 거리")]
    public float distance = 10.0f;

    [Tooltip("목표물로부터의 높이")]
    public float height = 5.0f;

    [Tooltip("카메라 회전 속도")]
    public float rotationSpeed = 100.0f;

    // 현재 카메라의 수평 각도를 저장할 변수
    private float currentAngle = 0.0f;

   void LateUpdate()
    {
        if (!target)
        {
            Debug.LogWarning("CameraController에 Target이 설정되지 않았습니다!");
            return;
        }

        // --- 입력 처리 ---
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float horizontalInput = 0f;

        // A 키 또는 왼쪽 방향키가 눌렸는지 확인
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            horizontalInput = 1f;  // <--- 여기를 -1f 에서 1f 로 수정
        }
        // D 키 또는 오른쪽 방향키가 눌렸는지 확인
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            horizontalInput = -1f; // <--- 여기를 1f 에서 -1f 로 수정
        }
        
        // --- 위치 계산 (핵심 로직) ---
        // 1. 입력에 따라 현재 각도를 업데이트합니다.
        currentAngle += horizontalInput * rotationSpeed * Time.deltaTime;

        // 2. 각도를 라디안으로 변환합니다. (삼각함수 계산용)
        float radianAngle = currentAngle * Mathf.Deg2Rad;

        // 3. target을 중심으로 원을 그리듯 X와 Z 오프셋을 계산합니다.
        float offsetX = distance * Mathf.Sin(radianAngle);
        float offsetZ = distance * Mathf.Cos(radianAngle);

        // 4. 카메라가 있어야 할 최종 위치를 계산합니다.
        Vector3 desiredPosition = target.position + new Vector3(offsetX, height, offsetZ);

        // 5. 계산된 위치로 카메라를 이동시키고 target을 바라보게 합니다.
        transform.position = desiredPosition;
        transform.LookAt(target);
    }
}