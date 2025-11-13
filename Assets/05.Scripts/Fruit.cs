using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("과일 등급")]
    [Tooltip("이 과일의 등급 (1 = Fruit1, 2 = Fruit2...)")]
    public int fruitLevel;

    [Header("다음 단계 프리펩")]
    [Tooltip("합체되었을 때 생성될 다음 단계 과일 프리펩")]
    public GameObject nextFruitPrefab; 

    [Header("효과 (선택 사항)")]
    [Tooltip("합체 시 재생될 파티클/효과음 프리펩")]
    public GameObject mergeEffectPrefab; 

    private bool hasMerged = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;
        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        if (otherFruit == null || otherFruit.hasMerged) return;

        if (otherFruit.fruitLevel == this.fruitLevel)
        {
            if (this.gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                if (this.nextFruitPrefab != null)
                {
                    Merge(otherFruit);
                }
                else
                {
                    DestroyFinalFruits(otherFruit);
                }
            }
        }
    }

    // 합체 로직 (1~5 단계)
    public void Merge(Fruit otherFruit)
    {
        this.hasMerged = true;
        otherFruit.hasMerged = true;

        
        // GameManager의 '대표(instance)'에게 점수를 추가하라고 요청
        // 점수는 이 과일의 등급(fruitLevel)만큼 추가됩니다. (1단계면 1점, 2단계면 2점...)
        GameManager.instance.AddScore(this.fruitLevel);
        

        Vector3 midpoint = (this.transform.position + otherFruit.transform.position) / 2.0f;

        if (mergeEffectPrefab != null)
        {
            Instantiate(mergeEffectPrefab, midpoint, Quaternion.identity);
        }

        Instantiate(nextFruitPrefab, midpoint, Quaternion.identity);
        
        Destroy(this.gameObject);
        Destroy(otherFruit.gameObject);
    }

    // 마지막 과일 파괴 로직 (6단계)
    public void DestroyFinalFruits(Fruit otherFruit)
    {
        this.hasMerged = true;
        otherFruit.hasMerged = true;

        
        // 마지막 6단계 과일이 합쳐질 때도 6점을 추가합니다.
        GameManager.instance.AddScore(this.fruitLevel);
        
        Vector3 midpoint = (this.transform.position + otherFruit.transform.position) / 2.0f;

        if (mergeEffectPrefab != null)
        {
            Instantiate(mergeEffectPrefab, midpoint, Quaternion.identity);
        }
        
        Destroy(this.gameObject);
        Destroy(otherFruit.gameObject);
    }
}