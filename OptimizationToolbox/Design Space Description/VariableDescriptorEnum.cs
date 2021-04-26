// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="VariableDescriptorEnum.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class VariableDescriptorEnum.
    /// Implements the <see cref="System.Collections.Generic.IEnumerator{OptimizationToolbox.VariableDescriptor}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerator{OptimizationToolbox.VariableDescriptor}" />
    class VariableDescriptorEnum : IEnumerator<VariableDescriptor>
    {
        /// <summary>
        /// The variable descriptors
        /// </summary>
        private readonly VariableDescriptor[] _variableDescriptors;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        /// <summary>
        /// The position
        /// </summary>
        int position = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDescriptorEnum"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public VariableDescriptorEnum(VariableDescriptor[] list)
        {
            _variableDescriptors = list;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns><see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            position++;
            return (position < _variableDescriptors.Length);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            position = -1;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        /// <exception cref="InvalidOperationException"></exception>
        public VariableDescriptor Current
        {
            get
            {
                try
                {
                    return _variableDescriptors[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        #endregion
    }

}