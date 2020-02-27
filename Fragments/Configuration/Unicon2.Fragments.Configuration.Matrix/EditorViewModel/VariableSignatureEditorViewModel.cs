﻿using Unicon2.Fragments.Configuration.Matrix.Interfaces.EditorViewModel;
using Unicon2.Infrastructure.Values.Matrix;
using Unicon2.Unity.ViewModels;

namespace Unicon2.Fragments.Configuration.Matrix.EditorViewModel
{
    public class VariableSignatureEditorViewModel : ViewModelBase, IVariableSignatureEditorViewModel
    {
        private IVariableColumnSignature _model;
        private string _signature;
        private bool _isMultipleAssgnmentAllowed;

        public string StrongName => nameof(VariableSignatureEditorViewModel);

        public object Model
        {
            get
            {
                this._model.Signature = this.Signature;
                this._model.IsMultipleAssignmentAllowed = this.IsMultipleAssgnmentAllowed;
                return this._model;

            }
            set
            {
                this._model = value as IVariableColumnSignature;
                this.Signature = this._model.Signature;
                this.IsMultipleAssgnmentAllowed = this._model.IsMultipleAssignmentAllowed;
            }
        }

        public string Signature
        {
            get { return this._signature; }
            set
            {
                this._signature = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsMultipleAssgnmentAllowed
        {
            get { return this._isMultipleAssgnmentAllowed; }
            set
            {
                this._isMultipleAssgnmentAllowed = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
