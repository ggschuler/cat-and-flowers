using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loveIconBehavior : MonoBehaviour
{
    [SerializeField] GameObject parchment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (Input.GetMouseButtonDown(0)  & hit.collider == this.gameObject.GetComponent<BoxCollider2D>())
        {
            parchment.SetActive(!parchment.activeInHierarchy);
        }
    }
}
