using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Test : MonoBehaviour {

    public LoopScrollRect test1;

    private void Awake()
    {
        test1.m_pCallback = (obj, index) =>
        {
            Text tt = obj.transform.GetComponentInChildren<Text>();
            tt.text = index + "";
        };
    }
    void Start () {
       
        test1.ITEM_MAX_COUNT = 55;
        test1.RefillCells();
    }

}
