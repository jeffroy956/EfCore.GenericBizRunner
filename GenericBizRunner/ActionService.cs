﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner
{
    public class ActionService<TBizInstance> : ActionService<DbContext, TBizInstance>
        where TBizInstance : class, IBizActionStatus
    {
        public ActionService(DbContext context, TBizInstance bizInstance, IGenericBizRunnerConfig config = null)
            : base(context, bizInstance, config)
        {
        }
    }

    public class ActionService<TContext, TBizInstance> : BizActionStatus, IActionService<TBizInstance>
        where TContext : DbContext
        where TBizInstance : class, IBizActionStatus
    {
        private readonly TBizInstance _bizInstance;
        private readonly IGenericBizRunnerConfig _config;
        private readonly TContext _context;

        public ActionService(TContext context, TBizInstance bizInstance, IGenericBizRunnerConfig config = null)
        {

            _context = context ?? throw new ArgumentNullException(nameof(context));
            _bizInstance = bizInstance ?? throw new ArgumentNullException(nameof(bizInstance));
            _config = config ?? new GenericBizRunnerConfig();
        }

        public IBizActionStatus Status => _bizInstance;

        public TOut RunBizAction<TOut>(object inputData)
        {
            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.InOut, _config.TurnOffCaching);
            return decoder.BizInfo.GetServiceInstance(_config).RunBizActionDbAndInstance<TOut>(_context, _bizInstance, inputData);
        }

        public TOut RunBizAction<TOut>()
        {
            throw new NotImplementedException();
        }

        public void RunBizAction(object inputData)
        {
            throw new NotImplementedException();
        }

        public TDto GetDto<TDto>() where TDto : class, new()
        {
            throw new NotImplementedException();
        }

        public TDto ResetDto<TDto>(TDto dto) where TDto : class
        {
            throw new NotImplementedException();
        }
    }
}