using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuageImageAlpha : MonoBehaviour
{
    public static GuageImageAlpha Instance { get; private set; }

    public static event System.Action OnStaminaEmpty;
    public static event System.Action OnStaminaRecovered;

    [Header("�̹��� ����")]
    public Image FilledGuageImage;
    public Image BackgroundGuageImage;

    [Header("������Ÿ�� ����")]
    public float lifeTime = 5f;  // �պ� ���� �ð�
    public float resetDelay = 3f;  // ���׹̳� ���� �� ���±��� ������
    public float baseSpeed = 1f;  // �ʱ� �պ� ���ǵ�
    public float acceleration = 1f;  // �ʴ� ���ǵ� ������

    private Coroutine lifeRoutine;

    private void Awake()
    {
        Instance = this;
    }

    // �ӽ� ������ Ÿ�� 5�� / ������ ����� 0.8�� �Ǵ� ���� ���� 255~0 �� �պ� �ð��� �������� �� ������
    // �ð��� �ٵǸ� PlayerStaminaZero �Լ� ȣ��
    // ���� �ð�(�ӽ� 3��) �� PlayerStaminaReset �Լ� ȣ��
    public void StartLifeRoutine()
    {
        if (lifeRoutine == null)
            lifeRoutine = StartCoroutine(LifeCoroutine());
    }

    public void CancelLifeRoutine()
    {
        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
            // ��� ���� ȸ��
            PlayerStaminaReset();
        }
    }

    private IEnumerator LifeCoroutine()
    {
        float t = 0f;
        // ���� ���İ��� �������鼭 �պ��� �ϴµ� 5�ʸ� ��Ƽ�� �ٽ� �ݺ���
        while (t < lifeTime)
        {
            t += Time.deltaTime;
            float speed = baseSpeed + acceleration * t;
            float alpha = Mathf.PingPong(t * speed, 1f);
            SetAlpha(alpha);
            yield return null;
        }

        // 5�� ����ٸ� ���� ���� ����
        lifeRoutine = null;

        // 5�� ��Ƽ�� ���� ����
        PlayerStaminaZero();

        // �̺�Ʈ ȣ��
        OnStaminaEmpty?.Invoke();

        // 3�� ��ٷȴٰ� ȸ��
        yield return new WaitForSeconds(resetDelay);

        PlayerStaminaReset();

        // ���⿡�� ���� �̺�Ʈ ȣ��
        OnStaminaRecovered?.Invoke();
    }

    private void SetAlpha(float a)
    {
        // Image.color.a�� 0~1 ����
        var c1 = FilledGuageImage.color;
        c1.a = a;
        FilledGuageImage.color = c1;

        var c2 = BackgroundGuageImage.color;
        c2.a = a;
        BackgroundGuageImage.color = c2;
    }

    public void PlayerStaminaZero()
    {
        // FilledGuageImage, BackgroundGuageImage �� alpha �� -> 0���� ����
        SetAlpha(0f);
        // ��ܰ� �÷��̾� �ִϸ��̼� ����
        // �÷��̾� �Ÿ� ������ �������� ������ �ڵ� ����
    }

    public void PlayerStaminaReset()
    {
        // FilledGuageImage, BackgroundGuageImage �� alpha �� -> 255���� ����
        SetAlpha(1f);
        // ��ܰ� �÷��̾� �ִϸ��̼� ����
        // �÷��̾� �Ÿ� ������ �������� ������ �ڵ� ����
    }
}
