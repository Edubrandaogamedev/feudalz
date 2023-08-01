#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//https://www.icode.com/c-function-for-a-bezier-curve/#:~:text=The%20Bezier%20Curve%20formula%20below,and%20line%20P2%20to%20P3).&text=At%20t%3D0%20you%20will,you%20will%20be%20at%20p3.
public class BezierCoin : MonoBehaviour
{
    public delegate void InsertCoins(float qnt);
    public static event InsertCoins OnInsertCoins;

    [Header("Setup")]
    [SerializeField]
    private Image flyCoinImage;
    [SerializeField]
    private ParticleSystem trail;
    [SerializeField]
    private float timeToCoinFly = 1;

    [Header("Points")]
    [SerializeField]
    private Vector3 ref1 = Vector3.one;
    [SerializeField]
    private Vector3 ref2 = Vector3.zero + Vector3.right;

    public GameObject target;
    private float coinsValue;

    private void Start()
    {
        flyCoinImage.enabled = false;
        trail.gameObject.SetActive(false);
        StartCoroutine(FlyCoin());
    }

    private Vector3 StartScreenCoinPosition
    {
        get { return flyCoinImage.transform.position; }
        //get { return Camera.main.ScreenToWorldPoint(flyCoinImage.transform.position); }
    }

    public Vector3 Ref1 { get => ref1; set => ref1 = value; }
    public Vector3 Ref2 { get => ref2; set => ref2 = value; }
    public float CoinsValue { get => coinsValue; set => coinsValue = value; }

    public IEnumerator FlyCoin()
    {
        flyCoinImage.enabled = true;
        trail.gameObject.SetActive(true);
        AudioManager.instance.Play("coin_sfx");

        float time = Time.timeSinceLevelLoad + timeToCoinFly;
        float normalizeTime = 0;
        Vector3 standard = StartScreenCoinPosition;
        Vector3 screenStart = standard;

        Vector3 barPosition = target.transform.position;

        Vector3 aux;

#if UNITY_EDITOR
        bool pause = Input.GetMouseButton(1);
        EditorApplication.isPaused = pause;
#endif

        do
        {
            normalizeTime = 1 - ((time - Time.timeSinceLevelLoad) / timeToCoinFly);
            aux = GetPoint(screenStart, barPosition, normalizeTime);

            aux.z = standard.z;
            flyCoinImage.transform.position = aux;

            yield return new WaitForFixedUpdate();
        }
        while (normalizeTime < 1);

        //todo subir numero do quanto foi comprado
        flyCoinImage.transform.position = standard;

        flyCoinImage.enabled = false;
        trail.gameObject.SetActive(false);
        OnInsertCoins?.Invoke(coinsValue);
    }

    private Vector2 GetPoint(Vector3 start, Vector3 end, float t)
    {
        Vector3 p1 = ref1 + start;
        Vector3 p2 = ref2 + start;

        float cx = 3 * (p1.x - start.x);
        float cy = 3 * (p1.y - start.y);
        float bx = 3 * (p2.x - p1.x) - cx;
        float by = 3 * (p2.y - p1.y) - cy;
        float ax = end.x - start.x - cx - bx;
        float ay = end.y - start.y - cy - by;
        float Cube = t * t * t;
        float Square = t * t;

        float resx = (ax * Cube) + (bx * Square) + (cx * t) + start.x;
        float resy = (ay * Cube) + (by * Square) + (cy * t) + start.y;

        return new Vector3(resx, resy, start.z);
    }


#if UNITY_EDITOR
    Vector3 auxPointStart;
    Vector3 auxPointEnd;

    [Header("UNITY_EDITOR")]
    [SerializeField]
    Vector3 start;
    [SerializeField]
    Vector3 end;
    [SerializeField]
    private int max = 150;

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;

        start = StartScreenCoinPosition;

        end = target.transform.position;
        Vector3 screenStart = StartScreenCoinPosition;

        auxPointStart = GetPoint(start, end, 0);
        for (int i = 1; i < max; i++)
        {
            auxPointEnd = (GetPoint(screenStart, end, i / (float)max));
            Handles.DrawLine(auxPointStart, auxPointEnd);
            auxPointStart = auxPointEnd;
        }

        Handles.color = Color.blue;
        start = GetPoint(StartScreenCoinPosition, target.transform.position, 1);
        end = GetPoint(StartScreenCoinPosition, target.transform.position, 0);

        start = StartScreenCoinPosition;
        end = target.transform.position;

        Handles.color = Color.green;
        Handles.DrawWireDisc(start, Vector3.forward, 10f);
        Handles.color = Color.red;
        Handles.DrawWireDisc(end, Vector3.forward, 10);
        Handles.color = Color.blue;
        Handles.DrawLine(start, end);


        Handles.color = Color.green;
        Handles.DrawWireDisc(ref1, Vector3.forward, 5);
        Handles.DrawWireDisc(ref2, Vector3.forward, 5);

        //Handles.color = Color.red;
        //Handles.DrawWireDisc(screenStart, Vector3.forward, 0.1f);
        //Handles.color = Color.green;
        //Handles.DrawWireDisc(ref1, Vector3.forward, 0.1f);
        //Handles.color = Color.blue;
        //Handles.DrawWireDisc(ref2, Vector3.forward, 0.1f);
        //Handles.color = Color.red;
        //Handles.DrawWireDisc(end, Vector3.forward, 0.1f);
    }
#endif
}
