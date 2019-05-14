﻿using Unicon2.Fragments.Configuration.Matrix.Interfaces.EditorViewModel;
using Unicon2.Fragments.Configuration.Matrix.Interfaces.EditorViewModel.Factories;
using Unicon2.Fragments.Configuration.Matrix.Interfaces.Model;
using Unicon2.Unity.Interfaces;

namespace Unicon2.Fragments.Configuration.Matrix.EditorViewModel.Factories
{
    public class VariableSignatureEditorViewModelFactory : IVariableSignatureEditorViewModelFactory
    {
        private readonly ITypesContainer _container;

        public VariableSignatureEditorViewModelFactory(ITypesContainer container)
        {
            this._container = container;
        }

        #region Implementation of IVariableSignatureEditorViewModelFactory

        public IVariableSignatureEditorViewModel CreateVariableSignatureEditorViewModel(IVariableSignature variableSignature)
        {
            IVariableSignatureEditorViewModel variableSignatureEditorViewModel =
                this._container.Resolve<IVariableSignatureEditorViewModel>();
            variableSignatureEditorViewModel.Model = variableSignature;
            return variableSignatureEditorViewModel;
        }

        public IVariableSignatureEditorViewModel CreateVariableSignatureEditorViewModel()
        {
            IVariableSignatureEditorViewModel variableSignatureEditorViewModel =
                this._container.Resolve<IVariableSignatureEditorViewModel>();
            variableSignatureEditorViewModel.Model = this._container.Resolve<IVariableSignature>();
            return variableSignatureEditorViewModel;
        }

        #endregion
    }
}
