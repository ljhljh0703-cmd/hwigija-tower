using System;
using UnityEngine;

namespace HwigiTower.Core
{
    [Serializable]
    public sealed class NumericParam
    {
        [SerializeField] private string key = string.Empty;
        [SerializeField] private float value;

        public string Key => key;
        public float Value => value;
    }
}
