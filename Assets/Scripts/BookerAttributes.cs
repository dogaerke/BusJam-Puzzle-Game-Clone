using System;
using UnityEngine;

namespace BookerNamespace
{
    [Serializable]
    public class BookerAttributes
    {
        [SerializeField] public GameColors bookerColor;

        private BookerAttributes(GameColors bookerColor)
        {
            this.bookerColor = bookerColor;
        }
    }

}