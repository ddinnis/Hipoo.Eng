﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public interface ISubscriberService<T>
    {
        public Task HandleSubscriber(T EventData);
    }
}