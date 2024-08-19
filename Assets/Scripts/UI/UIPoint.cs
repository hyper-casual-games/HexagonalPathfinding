using UnityEngine;
using TMPro;

public class UIPoint : MonoBehaviour
{
    [SerializeField] private Animation _animation;
    [SerializeField] private TMP_Text _text;

    public void SetText( string text )
    {
        _text.text = text;
    }


    public void PlayAnimation() {
        _animation?.Play();
    }

    public void StopAnimation() {
        _animation?.Stop();
    }
}
