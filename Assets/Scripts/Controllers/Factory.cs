using UnityEngine;
using Infra;
using Views;

namespace Controllers
{
    public class Factory : MonoBehaviour
    {
        public static Factory Instance { get; private set; }

        [Header("Board Squares Section")]
        [SerializeField] private Transform boardParent;
        [SerializeField] private SymbolView symbolViewPrefab;
        private void Awake()
        {
            Instance = this;
        }

        public ISymbolView InstantiateSquare()
        {
            return Instantiate(symbolViewPrefab, boardParent);
        }
    }
}

