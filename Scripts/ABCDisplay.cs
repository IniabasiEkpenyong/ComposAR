using UnityEngine;
using BuildForReal.Utilities;
using ABCUnity;

public class ABCDisplay : MonoBehaviour
{
    public Layout abcLayout;

    public void UpdateScoreDisplay()
    {
        string abcNotation = ABCConverter.ConvertToABC(CompositionManager.Instance.GetNoteEvents());
        abcLayout.LoadString(abcNotation);
    }
}

