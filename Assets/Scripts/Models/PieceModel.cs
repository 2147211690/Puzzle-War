using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public struct PieceModel
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isEmpty;
        [SerializeField] private PieceTypeEnum _type;

        public int Id
        {
            readonly get => _id;
            set => _id = value;
        }

        public bool IsEmpty
        {
            readonly get => _isEmpty;
            set => _isEmpty = value;
        }

        public PieceTypeEnum Type
        {
            readonly get => _type;
            set => _type = value;
        }
    }
}