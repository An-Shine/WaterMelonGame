using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

public class SmartLauncher : MonoBehaviour
{
    [Header("오브젝트 연결")]
    
    [Tooltip("생성할 과일 프리펩 목록 (Fruit1 ~ Fruit4)")]
    public List<GameObject> spawnableFruits; // 단일 프리펩에서 리스트로 변경
    

    [Tooltip("공이 생성/대기할 위치 (SpawnBall 오브젝트)")]
    public Transform spawnPoint;

    [Tooltip("발사 목표 지점 (BowlCenter 오브젝트)")]
    public Transform targetCenter;

    [Header("발사 설정")]
    [Tooltip("공을 '수평'으로 발사할 힘 (앞으로 나아가는 힘)")]
    public float launchForce = 500f;

    [Tooltip("공을 '위로' 쏘아 올릴 힘 (포물선의 높이)")]
    public float upwardArc = 300f;

    private GameObject currentBall;
    private Rigidbody currentBallRigidbody;

    void Start()
    {
        // --- [수정됨] ---
        // 리스트가 비어있는지 확인하는 로직으로 변경
        if (spawnableFruits == null || spawnableFruits.Count == 0 || spawnPoint == null || targetCenter == null)
        {
            Debug.LogError("SmartLauncher 스크립트에 프리펩 목록이나 필수 오브젝트가 연결되지 않았습니다!");
            return;
        }
        // --- [수정 완료] ---
        SpawnNextBall();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentBall != null)
        {
            FireBall();
        }
    }

    void SpawnNextBall()
    {        
        // 1. 리스트에서 랜덤 과일 프리펩 선택
        // Random.Range(min, max)에서 정수형 max는 '미만'이므로 0부터 (리스트 크기 - 1)까지 랜덤 선택
        int randomIndex = Random.Range(0, spawnableFruits.Count);
        GameObject randomFruitPrefab = spawnableFruits[randomIndex];

        if (randomFruitPrefab == null)
        {
             Debug.LogError($"spawnableFruits 리스트의 {randomIndex}번째 프리펩이 비어있습니다.");
             return;
        }
        
        // 2. 선택된 랜덤 프리펩으로 공 생성
        currentBall = Instantiate(randomFruitPrefab, spawnPoint.position, spawnPoint.rotation);      
        
        currentBall.transform.SetParent(spawnPoint);
        currentBallRigidbody = currentBall.GetComponent<Rigidbody>();

        if (currentBallRigidbody == null)
        {
            Debug.LogError($"프리펩 '{randomFruitPrefab.name}'에 Rigidbody 컴포넌트가 없습니다!");
            return;
        }
        currentBallRigidbody.isKinematic = true;
    }

    void FireBall()
    {
        // currentBall(현재 공)의 부모-자식 관계를 해제합니다. (null = 최상위 월드)
        // 이렇게 하면 공이 더 이상 'SpawnPoint'를 따라다니지 않고 독립적으로 움직일 수 있게 됩니다.
        currentBall.transform.SetParent(null);
        
        // 공의 Rigidbody에서 isKinematic 속성을 false로 변경합니다.
        // 이 순간부터 공은 중력의 영향을 받고 물리적인 힘(AddForce)에 반응하기 시작합니다.
        currentBallRigidbody.isKinematic = false;

        // --- 발사 방향 및 힘 계산 ---

        // 1. 발사 지점(spawnPoint)에서 목표 지점(targetCenter)을 향하는 '방향 벡터'를 계산합니다.
        //    (벡터의 뺄셈: 목표 위치 - 시작 위치 = 시작에서 목표로 향하는 방향)
        Vector3 direction = targetCenter.position - spawnPoint.position;
        
        // 2. 계산된 방향 벡터의 Y(높이) 값을 0으로 강제로 설정합니다.
        //    (목표 지점이 스폰 지점보다 높든 낮든 상관없이, 순수하게 '수평(X, Z)' 방향만 남기기 위함입니다.)
        direction.y = 0; 

        // 3. '수평 방향'으로 가할 힘을 계산합니다.
        //    direction.normalized: 벡터의 크기를 1로 만듭니다. (순수한 방향만 남김)
        //    여기에 launchForce(수평 힘 크기)를 곱합니다.
        Vector3 horizontalForce = direction.normalized * launchForce; 
        
        // 4. '수직(위쪽)' 방향으로 가할 힘을 계산합니다. (포물선의 높이를 만듭니다)
        //    Vector3.up은 (0, 1, 0) 방향을 의미합니다.
        //    여기에 upwardArc(수직 힘 크기)를 곱합니다.
        Vector3 verticalForce = Vector3.up * upwardArc; 

        // 5. 최종 발사 힘 = 계산된 수평 힘 + 계산된 수직 힘
        //    (두 힘 벡터를 더하여 대각선 위로 향하는 힘을 만듭니다.)
        Vector3 finalForce = horizontalForce + verticalForce;

        // 6. 계산된 최종 힘(finalForce)을 공의 Rigidbody에 '순간적으로' 가합니다(ForceMode.Impulse).
        //    이 힘과 중력이 합쳐져 포물선 궤적이 만들어집니다.
        currentBallRigidbody.AddForce(finalForce, ForceMode.Impulse);

        // --- 발사 후 처리 ---

        // 7. 발사된 공에 대한 참조(연결)를 끊습니다(null로 만듭니다).
        //    이렇게 해야 Update() 함수에서 스페이스바를 연타해도 이미 발사된 공을
        //    다시 발사하려는 시도를 하지 않습니다. (currentBall != null 조건 때문)
        currentBall = null;
        currentBallRigidbody = null;

        // 8. "SpawnNextBall" 함수를 1.0초 뒤에 실행하도록 '예약'합니다.
        //    (다음 공이 즉시 생성되어 방금 발사한 공과 부딪히는 것을 방지합니다.)
        Invoke("SpawnNextBall", 1.0f);
    }
}