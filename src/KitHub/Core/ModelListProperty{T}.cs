// -----------------------------------------------------------------------
// <copyright file="ModelListProperty{T}.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace KitHub.Core
{
    internal class ModelListProperty<T> : ListBase<T>
    {
        internal ModelListProperty(KitHubSession session)
            : base(session)
        {
        }

        internal void UpdateList(SerializableModelBase sender, JArray array, IModelInitializer initializer)
        {
            int i = 0;
            foreach (JToken token in array)
            {
                T item;
                if (initializer != null)
                {
                    item = (T)initializer.InitializeModel(sender, token);
                }
                else
                {
                    item = token.ToObject<T>(KitHubSession.Serializer);
                }

                bool found = false;
                for (int j = i; j < Count && !found; j++)
                {
                    if (Equals(this[j], item))
                    {
                        if (j > i)
                        {
                            RemoveRangeInternal(i, j - i);
                        }

                        found = true;
                    }
                }

                // clear tail as it no longer exists and add item to the end of the list
                if (!found)
                {
                    if (Count > i)
                    {
                        if (i == 0)
                        {
                            ClearInternal();
                        }
                        else
                        {
                            while (i < Count + 1)
                            {
                                RemoveAtInternal(Count - 1);
                            }
                        }
                    }

                    AddInternal(item);
                }

                i++;
            }
        }
    }
}
