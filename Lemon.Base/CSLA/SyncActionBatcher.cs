using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterspring.Lemon.Base
{
    public class SyncActionBatcher : IDisposable
    {
        private readonly IControlPropertyChangedActionMode _model;
        private readonly PropertyChangedActionMode _originalSyncMode;

        public SyncActionBatcher(IControlPropertyChangedActionMode model)
        {
            _model = model;
            _originalSyncMode = _model.SyncMode;

            //If it was already batched, we don't need to do anything.
            //If they were suppressed, and we're nested in side that, then that takes precedence over this, so again we don't need to do anything.
            if(_model.SyncMode == PropertyChangedActionMode.Normal)
                _model.SyncMode = PropertyChangedActionMode.Batch;
        }

        public void Dispose()
        {            
            _model.SyncMode = _originalSyncMode;
        }
    }
}
