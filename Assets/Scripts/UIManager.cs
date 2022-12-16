using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject booksLeft;


    public void SetBooksRemaining(int newBooksLeft)
    {
        booksLeft.GetComponent<TMPro.TextMeshProUGUI>().text = newBooksLeft.ToString();
    }
}
