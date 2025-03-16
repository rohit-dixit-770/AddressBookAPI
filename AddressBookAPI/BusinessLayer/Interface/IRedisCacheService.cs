﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IRedisCacheService
    {
        void SetCache(string key, string value, int expirationInMinutes);
        string GetCache(string key);
        void RemoveCache(string key);
    }
}
