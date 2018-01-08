﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOut<TBizInterface, TBizIn, TBizOut> : ActionServiceBase
    {
        public ActionServiceInOut(WriteToDbStates writeStates, IGenericBizRunnerConfig config) 
            : base(writeStates, config)
        {
        }

        public TOut RunBizActionDbAndInstance<TOut>(DbContext db, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, false, Config);
            var fromBizCopier = DtoAccessGenerator.BuildCopier(typeof(TBizOut), typeof(TOut), false, false, Config);
            var bizStatus = (IBizActionStatus) bizInstance;

            var inData = toBizCopier.DoCopyToBiz<TBizIn>(db, inputData);

            var result = ((IGenericAction<TBizIn, TBizOut>) bizInstance).BizAction(inData);

            //This handles optional call of save changes
            SaveChangedIfRequiredAndNoErrors(db, bizStatus);
            if (bizStatus.HasErrors) return ReturnDefaultAndResetInDto<TOut>(db, toBizCopier, inputData);

            var data = fromBizCopier.DoCopyFromBiz<TOut>(db, result);
            return data;
        }
    }
}