using UnityEngine;

public class FixedPosition : MonoBehaviour
{
    private Transform parentTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = parentTransform.transform.position + new Vector3(0, 1, 0);
        this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (parentTransform.localScale.x == -1)
            this.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        else
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
}
