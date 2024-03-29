﻿using System;

namespace CommonAPI
{
    public abstract class BaseSubmodule
    {
        /// <summary>
        /// Return true if the submodule is loaded.
        /// </summary>
        public bool Loaded { get; internal set; }

        internal virtual Version Build => new Version();
        internal virtual Type[] Dependencies => Type.EmptyTypes;

        internal void ThrowIfNotLoaded()
        {
            if (!Loaded)
            {
                var submoduleName = GetType().Name;
                string message = $"{submoduleName} is not loaded. Please use [{nameof(CommonAPISubmoduleDependency)}(nameof({submoduleName})]";
                throw new InvalidOperationException(message);
            }
        }

        internal virtual void SetHooks() {}
        internal virtual void Load() {}
        internal virtual void PostLoad() {}
        internal virtual void Unload() {}
        internal virtual void UnsetHooks() {}

        internal virtual bool LoadCheck()
        {
            return true;
        }

        internal virtual Type[] GetOptionalDependencies()
        {
            return Array.Empty<Type>();
        }
    }
}