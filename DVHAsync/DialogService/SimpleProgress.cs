﻿using System;

namespace DVHAsync
{
    public class SimpleProgress : ISimpleProgress
    {
        private readonly Action _onIncrement;

        public SimpleProgress(Action onIncrement)
        {
            _onIncrement = onIncrement;
        }

        public void Increment()
        {
            _onIncrement();
        }
    }
}