using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Views
{
    public class DebugMessageUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private UnityEngine.UI.Image _background;
        
        private float _timerDeath = 10f;
        private bool _isDestroying;

        public void Set(string message)
        {
            _text.text = message;
        }
        
        private void Update()
        {
            if (_isDestroying)
            {
                return;
            }
            
            _timerDeath -= Time.deltaTime;
            if (_timerDeath <= 0)
            {
                _text.DOFade(0, 2f).OnComplete(() => Destroy(gameObject));
                _background.DOFade(0, 2f).OnComplete(() => Destroy(gameObject));
                _isDestroying = true;
            }
        }
    }
}