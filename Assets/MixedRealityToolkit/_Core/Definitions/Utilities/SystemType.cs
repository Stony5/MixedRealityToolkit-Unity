﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    /// <summary>
    /// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
    /// </summary>
    [Serializable]
    public sealed class SystemType : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string reference = string.Empty;

        private Type type;

        public static string GetReference(Type type)
        {
            return type != null ? $"{type.FullName}, {type.Assembly.GetName().Name}" : string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
        public SystemType(string assemblyQualifiedClassName)
        {
            Type = !string.IsNullOrEmpty(assemblyQualifiedClassName)
                ? Type.GetType(assemblyQualifiedClassName)
                : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="type">Class type.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="type"/> is not a class type.
        /// </exception>
        public SystemType(Type type)
        {
            Type = type;
        }

        #region ISerializationCallbackReceiver Members

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(reference))
            {
                type = Type.GetType(reference);

                if (type == null)
                {
                    Debug.LogWarning($"'{reference}' was referenced but class or struct type was not found.");
                }
            }
            else
            {
                type = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion ISerializationCallbackReceiver Members

        /// <summary>
        /// Gets or sets type of class reference.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not a class type.
        /// </exception>
        public Type Type
        {
            get { return type; }
            set
            {
                bool isValid = value.IsValueType && !value.IsEnum || value.IsClass;
                if (value != null && !isValid)
                {
                    throw new ArgumentException($"'{value.FullName}' is not a class or struct type.", nameof(value));
                }

                type = value;
                reference = GetReference(value);
            }
        }

        public static implicit operator string(SystemType type)
        {
            return type.reference;
        }

        public static implicit operator Type(SystemType type)
        {
            return type.Type;
        }

        public static implicit operator SystemType(Type type)
        {
            return new SystemType(type);
        }

        public override string ToString()
        {
            if (Type.FullName != null)
            {
                if (Type != null)
                {
                    return Type.FullName;
                }
            }

            return "(None)";
        }
    }
}