using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//just facilitates updating the remaining books UI
public class UIManager : MonoBehaviour
{
    public GameObject booksLeft;



    public void SetBooksRemaining(int newBooksLeft)
    {
        booksLeft.GetComponent<TMPro.TextMeshProUGUI>().text = newBooksLeft.ToString();
    }
}
