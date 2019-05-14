﻿using System;
using System.Windows;
using System.Windows.Controls;
using Unicon2.Presentation.Infrastructure.Enums;
using Unicon2.Presentation.Infrastructure.ViewModels.DockingManagerWindows;
using Unicon2.Presentation.Infrastructure.ViewModels.FragmentInterfaces;

namespace Unicon2.SharedResources.LayoutHelpers
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style ProjectPaneStyle { get; set; }

        public Style FragmentsPaneStyle { get; set; }

        public Style BottomWindowStyle { get; set; }

        public Style LeftWindowStyle { get; set; }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is IFragmentPaneViewModel)
                return this.FragmentsPaneStyle;
            if (item is IAnchorableWindow)
            {

                 switch ((item as IAnchorableWindow).AnchorableDefaultPlacementEnum)
                {
                    case PlacementEnum.Top:
                        break;
                    case PlacementEnum.Left:
                        return this.LeftWindowStyle;
                        break;
                    case PlacementEnum.Right:
                        break;
                    case PlacementEnum.Bottom:
                        return this.BottomWindowStyle;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }

            return base.SelectStyle(item, container);
        }
    }
}