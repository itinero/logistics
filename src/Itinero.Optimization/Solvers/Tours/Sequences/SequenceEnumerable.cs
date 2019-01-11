﻿/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;

namespace Itinero.Optimization.Solvers.Tours.Sequences
{
    /// <summary>
    /// Enumerates all sequences of given length.
    /// </summary>
    internal sealed class SequenceEnumerable : IEnumerable<int[]>
    {
        private readonly IEnumerable<int> _tour;
        private readonly bool _isClosed;
        private readonly bool _loopAround;
        private readonly int _size;
        
        /// <summary>
        /// Creates a new sequence enumerable starting from the given customer.
        /// </summary>
        public SequenceEnumerable(IEnumerable<int> tour, bool isClosed, int size, bool loopAround = true)
        {
            _size = size;
            _tour = tour;
            _isClosed = isClosed;
            _loopAround = loopAround;
        }
        
        /// <summary>
        /// Creates a new sequence enumerable starting from the given customer.
        /// </summary>
        public SequenceEnumerable(Tour tour, int size, bool loopAround = true)
        {
            _size = size;
            _tour = tour;
            _isClosed = tour.IsClosed();
            _loopAround = loopAround;
        }

        /// <summary>
        /// Enumerator for sequences, allow access to the sequences without copying them.
        /// </summary>
        public sealed class SequenceEnumerator : IEnumerator<int[]>
        {
            private readonly bool _isClosed;
            private readonly bool _loopAroundFirst;
            private readonly IEnumerator<int> _enumerator;
            private readonly int _size;

            /// <summary>
            /// Creates a new sequence enumerator.
            /// </summary>
            /// <param name="enumerator">The enumerator.</param>
            /// <param name="isClosed">True if the tour is closed.</param>
            /// <param name="loopAroundFirst">If the tour is closed, loop around first or not.</param>
            /// <param name="size">The size of the sequences to return.</param>
            public SequenceEnumerator(IEnumerator<int> enumerator, bool isClosed, int size, bool loopAroundFirst)
            {
                _enumerator = enumerator;
                _isClosed = isClosed;
                _loopAroundFirst = loopAroundFirst;
                _size = size;

                _current = new int[_size];
                for (var i = 0; i < _size; i++)
                {
                    _current[i] = Tour.NOT_SET;
                }
            }

            private int[] _current;

            /// <summary>
            /// Gets a copy of the current sequence.
            /// </summary>
            /// <returns></returns>
            public int[] Current => _current.Clone() as int[];

            /// <summary>
            /// Gets an element in the current sequence.
            /// </summary>
            /// <returns></returns>
            public int this[int i] => _current[i];

            /// <summary>
            /// Disposes this enumerator.
            /// </summary>
            public void Dispose()
            {
                _enumerator.Dispose();
            }

            object System.Collections.IEnumerator.Current => this.Current;

            private void MoveCurrent()
            {
                // move current.
                for (var i = 0; i < _size - 1; i++)
                {
                    _current[i] = _current[i + 1];
                }
            }

            private int _afterFirst = -1;

            /// <summary>
            /// Moves to the next sequence.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_afterFirst == -1)
                { // enumeration is before first (thing a closed tour).
                    if (_enumerator.MoveNext())
                    {
                        this.MoveCurrent();
                        _current[_size - 1] = _enumerator.Current;
                        while (_current[0] == Tour.NOT_SET)
                        {
                            if (!_enumerator.MoveNext())
                            {
                                return false;
                            }
                            this.MoveCurrent();
                            _current[_size - 1] = _enumerator.Current;
                        }
                        return true;
                    }
                    else if(_isClosed)
                    {
                        _afterFirst = 0;
                        _enumerator.Reset();
                        return this.MoveNext();
                    }
                    return false;
                }
                else if(_afterFirst < _size - 1)
                {
                    if (!_loopAroundFirst && _afterFirst > 0)
                    { // don't loop around, only use first once as the last.
                        return false;
                    }
                    if (_enumerator.MoveNext())
                    {
                        _afterFirst++;
                        this.MoveCurrent();
                        _current[_size - 1] = _enumerator.Current;
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _enumerator.Reset();

                _current = new int[_size];
                for (var i = 0; i < _size; i++)
                {
                    _current[i] = Tour.NOT_SET;
                }
                _afterFirst = -1;
            }
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>The sequence enumerator.</returns>
        public SequenceEnumerator GetEnumerator()
        {
            return new SequenceEnumerator(_tour.GetEnumerator(), _isClosed, _size, _loopAround);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator<int[]> IEnumerable<int[]>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}