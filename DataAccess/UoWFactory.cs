﻿using EF7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interaces
{
    public class UoWFactory
    {
        public static IUnitOfWork Create()
        {
            return new UnitOfWork();
        }
    }
}
