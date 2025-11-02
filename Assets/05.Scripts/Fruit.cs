using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("과일 등급")]
    [Tooltip("이 과일의 등급 (1 = Fruit1, 2 = Fruit2...)")]
    public int fruitLevel;

    [Header("다음 단계 프리펩")]
    [Tooltip("합체되었을 때 생성될 다음 단계 과일 프리펩")]
    public GameObject nextFruitPrefab; // 마지막 6단계는 여기를 'None' (null)으로 둡니다.

    [Header("효과 (선택 사항)")]
    [Tooltip("합체 시 재생될 파티클/효과음 프리펩")]
    public GameObject mergeEffectPrefab; 

    private bool hasMerged = false;

    // 물리적 충돌이 시작될 때 호출됩니다.
    void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

        if (otherFruit == null || otherFruit.hasMerged) return;

        // 4. 두 과일의 레벨이 같은지 확인합니다.
        if (otherFruit.fruitLevel == this.fruitLevel)
        {
            // 5. 중복 실행 방지 ID 체크
            if (this.gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                // --- [수정된 부분] ---
                // 6. 합체할 다음 과일이 있는지 (1~5단계인지) 확인
                if (this.nextFruitPrefab != null)
                {
                    // Case 1: 다음 과일이 있다 (1~5단계) -> 합체!
                    Merge(otherFruit);
                }
                else
                {
                    // Case 2: 다음 과일이 없다 (마지막 6단계) -> 파괴!
                    DestroyFinalFruits(otherFruit);
                }
                // --- [수정 완료] ---
            }
        }
    }

    // 합체 로직 (1~5 단계)
    void Merge(Fruit otherFruit)
    {
        this.hasMerged = true;
        otherFruit.hasMerged = true;

        Vector3 midpoint = (this.transform.position + otherFruit.transform.position) / 2.0f;

        if (mergeEffectPrefab != null)
        {
            Instantiate(mergeEffectPrefab, midpoint, Quaternion.identity);
        }

        // 다음 단계 과일을 생성합니다.
        Instantiate(nextFruitPrefab, midpoint, Quaternion.identity);
        
        Destroy(this.gameObject);
        Destroy(otherFruit.gameObject);
    }

    // --- [새로 추가된 함수] ---
    // 마지막 과일 파괴 로직 (6단계)
    void DestroyFinalFruits(Fruit otherFruit)
    {
        // 1. 두 과일 모두 "처리 중" 상태로 만듭니다.
        this.hasMerged = true;
        otherFruit.hasMerged = true;

        // 2. 중간 지점 계산 (이펙트용)
        Vector3 midpoint = (this.transform.position + otherFruit.transform.position) / 2.0f;

        // 3. (선택 사항) 파괴 시 이펙트가 설정되어 있다면, 중간 지점에 생성합니다.
        if (mergeEffectPrefab != null)
        {
            Instantiate(mergeEffectPrefab, midpoint, Quaternion.identity);
        }
        
        // 4. 두 과일(나와 상대방)을 파괴합니다.
        Destroy(this.gameObject);
        Destroy(otherFruit.gameObject);
    }
}
