﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Unicon2.Fragments.Configuration.Exporter.Interfaces;
using Unicon2.Fragments.Configuration.Infrastructure.Export;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Runtime;
using Unicon2.Fragments.Configuration.Model.Properties;
using Unicon2.Infrastructure.Common;
using Unicon2.Infrastructure.Extensions;
using Unicon2.Presentation.Infrastructure.TreeGrid;

namespace Unicon2.Fragments.Configuration.Exporter.ItemRenderers
{
   public class ComplexPropertyRenderer:IConfigurationItemRenderer
    {
        private readonly IItemRendererFactory _rendererFactory;

        public ComplexPropertyRenderer(IItemRendererFactory rendererFactory)
        {
            _rendererFactory = rendererFactory;
        }

        public Maybe<List<TagBuilder>> RenderHtmlFromItem(IConfigurationItemViewModel complexProperty, SelectorForItemsGroup selectorForItemsGroup = null,
            int depthLevel = 0)
        {

            List<TagBuilder> tagBuilders = new List<TagBuilder>();
            tagBuilders.Add(ConfigTableRowRenderer
                .Create()
                .SetName(new RenderData(complexProperty.Header))
                .SetDepth(depthLevel)
                .SetSelectors(selectorForItemsGroup.IsPrintDeviceValuesAllowed,selectorForItemsGroup.IsPrintLocalValuesAllowed)
                .Render());

            (complexProperty as IRuntimeComplexPropertyViewModel).ChildStructItemViewModels.ForEach((item =>
            {
                _rendererFactory
                    .GetConfigurationItemRenderer(item)
                    .RenderHtmlFromItem(item,selectorForItemsGroup, depthLevel + 1)
                    .OnNotEmpty(result => tagBuilders.AddRange(result));
            }));
            return Maybe<List<TagBuilder>>.FromValue(tagBuilders);
        }
    }
}
