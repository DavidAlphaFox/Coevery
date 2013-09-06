﻿using Orchard.Events;

namespace Coevery.Entities.Events {
    public interface IEntityEvents : IEventHandler {
        void OnCreated(string entityName);
        void OnDeleting(string entityName);
    }
}
