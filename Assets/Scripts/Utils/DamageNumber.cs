using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float lifetime = 1f;
    private TextMeshProUGUI text;
    private float timer;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Setup(float damage)
    {
        text.text = ((int)damage).ToString();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Zanikanie
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
