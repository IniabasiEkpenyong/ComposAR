using UnityEngine;
using ABCUnity;

public class ABCNotationTester : MonoBehaviour
{
    public Layout abcLayout;

    void Start()
    {
        // Define ABC notation for notes C1 to C7
        string abcNotation = @"
X:1
T:Full Range Reference
M:4/4
L:1/4
V:1
V:2 clef=bass
K:C
[V:1] E F G A | B c d e |
f g a b | c' d' e' f' |
g' a' b' c'' | d'' e'' f'' g'' |
[V:2] C,,, D,, E,, F,, | G,, A,, B,, C, |
D, E, F, G, | A, B, C D |
E F G A | B/4 |
";

        abcLayout.LoadString(abcNotation);
    }
}
