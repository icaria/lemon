using System;

namespace Lemon.Base
{
    public class DoActionDeactivator : IDisposable
    {
        private readonly IControlPropertyChangedActionMode _model;
        private readonly PropertyChangedActionMode _originalDoMode;
        private readonly PropertyChangedActionMode _originalSyncMode;

        public DoActionDeactivator(IControlPropertyChangedActionMode model)
        {
            _model = model;
            _originalDoMode = _model.DoMode;
            _originalSyncMode = _model.SyncMode;

            //Suppress all actions
            _model.DoMode = PropertyChangedActionMode.Suppress;
            if (_model.SyncMode == PropertyChangedActionMode.Normal)
                _model.SyncMode = PropertyChangedActionMode.Batch;
        }

        public void Dispose()
        {
            _model.DoMode = _originalDoMode;
            _model.SyncMode = _originalSyncMode;
        }
    }
}