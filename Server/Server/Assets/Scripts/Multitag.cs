using UnityEngine;

public class Multitag : MonoBehaviour
{
    public string[] Tags;

    public bool Contains(string value)
    {
        foreach (string tag in Tags)
        {
            if (tag == value)
            {
                return true;
            }
        }

        return false;
    }
}