using System.Collections.Generic;
using UnityEngine;

namespace HwigiTower.Core
{
    public sealed class CompositeMovementInputSource : IMovementInputSource
    {
        private readonly IReadOnlyList<IMovementInputSource> _sources;

        public CompositeMovementInputSource(IReadOnlyList<IMovementInputSource> sources)
        {
            _sources = sources;
        }

        public MovementInputState Read()
        {
            var move = Vector2.zero;
            var interactPressed = false;

            for (var i = 0; i < _sources.Count; i++)
            {
                var state = _sources[i].Read();
                if (state.Move.sqrMagnitude > move.sqrMagnitude)
                {
                    move = state.Move;
                }

                interactPressed |= state.InteractPressed;
            }

            return new MovementInputState(move, interactPressed);
        }
    }
}
