using UnityEngine;

public class SmartLauncher : MonoBehaviour
{
    [Header("오브젝트 연결")]
    [Tooltip("생성할 과일 (Fruit1) 프리펩")]
    public GameObject fruitPrefab;

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
        if (fruitPrefab == null || spawnPoint == null || targetCenter == null)
        {
            Debug.LogError("SmartLauncher 스크립트에 필수 오브젝트가 연결되지 않았습니다!");
            return;
        }
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
        currentBall = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
        currentBall.transform.SetParent(spawnPoint);
        currentBallRigidbody = currentBall.GetComponent<Rigidbody>();

        if (currentBallRigidbody == null)
        {
            Debug.LogError("프리펩에 Rigidbody 컴포넌트가 없습니다!");
            return;
        }
        currentBallRigidbody.isKinematic = true;
    }

    void FireBall()
    {
        currentBall.transform.SetParent(null);
        currentBallRigidbody.isKinematic = false;

        // 1. 수평 방향 계산 (Y축 높이는 무시)
        Vector3 direction = targetCenter.position - spawnPoint.position;
        direction.y = 0; 

        // 2. 수평 힘과 수직 힘(포물선)을 조합
        Vector3 horizontalForce = direction.normalized * launchForce; 
        Vector3 verticalForce = Vector3.up * upwardArc; 

        // 3. 최종 힘 계산 (두 힘을 더함)
        Vector3 finalForce = horizontalForce + verticalForce;

        // 4. 발사!
        currentBallRigidbody.AddForce(finalForce, ForceMode.Impulse);

        currentBall = null;
        currentBallRigidbody = null;

        // --- [수정된 부분] ---
        // 다음 공을 바로 생성하지 않고, 1.0초 뒤에 생성하도록 예약합니다.
        Invoke("SpawnNextBall", 1.0f);
    }
}