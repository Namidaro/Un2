﻿using System.Collections.Generic;
using System.Linq;
using Unicon2.Fragments.Configuration.Matrix.Keys;
using Unicon2.Fragments.Configuration.Matrix.Model.OptionTemplates;
using Unicon2.Infrastructure.Values.Matrix;
using Unicon2.Infrastructure.Values.Matrix.Helpers;
using Unicon2.Infrastructure.Values.Matrix.OptionTemplates;
using Unicon2.Unity.Interfaces;

namespace Unicon2.Fragments.Configuration.Matrix.Model.Helpers
{
    public class DefaultBitOptionUpdatingStrategy : IBitOptionUpdatingStrategy
    {
        private readonly ITypesContainer _container;

        public DefaultBitOptionUpdatingStrategy(ITypesContainer container)
        {
            _container = container;
        }

        public void UpdateBitOptions(IMatrixTemplate matrixTemplate)
        {
            List<IBitOption> resultBitOptions = new List<IBitOption>();
            foreach (IVariableColumnSignature variableOptionSignature in matrixTemplate.VariableColumnSignatures)
            {
                if (matrixTemplate.MatrixVariableOptionTemplate is ListMatrixVariableOptionTemplate)
                {
                    foreach (IOptionPossibleValue optionPossibleValue in
                        (matrixTemplate.MatrixVariableOptionTemplate as ListMatrixVariableOptionTemplate)
                        .OptionPossibleValues)
                    {
                        IBitOption bitOption = _container.Resolve<IBitOption>(MatrixKeys.LIST_MATRIX_BIT_OPTION);
                        ((ListMatrixBitOption)bitOption).OptionPossibleValue = optionPossibleValue;
                        bitOption.VariableColumnSignature = variableOptionSignature;

                        IBitOption existing =
                            matrixTemplate.ResultBitOptions.FirstOrDefault((option => option.IsBitOptionEqual(bitOption)));
                        resultBitOptions.Add(existing ?? bitOption);
                    }
                }
                else if (matrixTemplate.MatrixVariableOptionTemplate is BoolMatrixVariableOptionTemplate)
                {
                    IBitOption bitOption = _container.Resolve<IBitOption>(MatrixKeys.BOOL_MATRIX_BIT_OPTION);
                    bitOption.VariableColumnSignature = variableOptionSignature;

                    IBitOption existing =
                        matrixTemplate.ResultBitOptions.FirstOrDefault((option => option.IsBitOptionEqual(bitOption)));
                    resultBitOptions.Add(existing ?? bitOption);
                }
            }


            matrixTemplate.ResultBitOptions = resultBitOptions;

        }
    }

}

