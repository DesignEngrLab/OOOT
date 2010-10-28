/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    class VariableDescriptorEnum : IEnumerator<VariableDescriptor>
    {
        private readonly VariableDescriptor[] _variableDescriptors;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public VariableDescriptorEnum(VariableDescriptor[] list)
        {
            _variableDescriptors = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _variableDescriptors.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

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
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        #endregion
    }

}