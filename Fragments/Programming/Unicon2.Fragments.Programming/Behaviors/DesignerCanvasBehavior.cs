﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using Unicon2.Fragments.Programming.Adorners;
using Unicon2.Fragments.Programming.Infrastructure.ViewModels.Scheme;
using Unicon2.Fragments.Programming.Infrastructure.ViewModels.Scheme.ElementViewModels;
using Unicon2.Fragments.Programming.ViewModels;

namespace Unicon2.Fragments.Programming.Behaviors
{
    public class DesignerCanvasBehavior : Behavior<Canvas>
    {
        private const double SCALE_STEP = 0.25;
        private const double MAX_SCALE = 2.5;

        private ScaleTransform _scaleTransform;

        public Point? RubberbandSelectionStartPoint { get; private set; }
        public Canvas DesignerCanvas { get; private set; }
        public SchemeTabViewModel TabViewModel { get; private set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.DesignerCanvas = AssociatedObject;
            Grid outerGrid = this.GetOuterGrid(this.DesignerCanvas);
            if (outerGrid != null)
            {
                this._scaleTransform = new ScaleTransform(1, 1);
                outerGrid.LayoutTransform = this._scaleTransform;
            }
            this.DesignerCanvas.MouseDown += this.OnMouseDown;
            this.DesignerCanvas.MouseMove += this.OnMouseMove;
            this.DesignerCanvas.Drop += this.OnDrop;
            this.TabViewModel = this.DesignerCanvas.DataContext as SchemeTabViewModel;
            if (this.TabViewModel != null)
            {
                this.TabViewModel.SelfBehavior = this;
                this.TabViewModel.Scale = 1.0;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.DesignerCanvas);
                if (adornerLayer != null)
                {
                    var designerCanvasAdorner = new DesignerCanvasAdorner(this.DesignerCanvas, this.TabViewModel) {IsHitTestVisible = false};
                    adornerLayer.Add(designerCanvasAdorner);
                }
            }
        }

        //На случай, если вместо Barder использовать Canvas
        private Grid GetOuterGrid(DependencyObject obj)
        {
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(obj);
                if (parent == null) return null;
                if (parent is Grid grid && grid.Name == "OuterBorder") return grid;
                obj = parent;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.DesignerCanvas.MouseDown -= this.OnMouseDown;
            this.DesignerCanvas.MouseMove -= this.OnMouseMove;
            this.DesignerCanvas.Drop -= this.OnDrop;                 
        }

        public void IncrementZoom()
        {
            if (this.TabViewModel.Scale >= MAX_SCALE)
            {
                this.TabViewModel.Scale = MAX_SCALE;
                return;
            }
            this.TabViewModel.Scale += SCALE_STEP;
            this._scaleTransform.ScaleX = this.TabViewModel.Scale;
            this._scaleTransform.ScaleY = this.TabViewModel.Scale;
        }

        public void DecrementZoom()
        {
            if (this.TabViewModel.Scale <= SCALE_STEP)
            {
                this.TabViewModel.Scale = SCALE_STEP;
                return;
            }
            this.TabViewModel.Scale -= SCALE_STEP;
            this._scaleTransform.ScaleX = this.TabViewModel.Scale;
            this._scaleTransform.ScaleY = this.TabViewModel.Scale;
        }



        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Equals(e.Source, this.DesignerCanvas)) return;

            if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Left)
            {
                // если нажата была клавиша мыши, то сбрасываем выделение
                foreach (ISchemeElement item in this.TabViewModel.ElementCollection)
                    item.IsSelected = false;
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                //когда нажали на Canvas левой клавишей мышью, фиксируем позицию мыши для выделения рамкой элементов
                this.RubberbandSelectionStartPoint = e.GetPosition(this.DesignerCanvas);
            }

            e.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // если клавиша мыши не нажата, то нет выделения
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.RubberbandSelectionStartPoint = null;
                return;
            }
            // иначе делаем рамку
            if (this.RubberbandSelectionStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.DesignerCanvas);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this);
                    adornerLayer.Add(adorner);
                }
            }
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.AllowedEffects != DragDropEffects.Copy) return;

            DragElement dragObject = e.Data.GetData(typeof(DragElement)) as DragElement;
            if (dragObject == null) return;
            ILogicElementViewModel dragItem = dragObject.Item;
            this.TabViewModel.ElementCollection.Add(dragItem);
            // округление до чисел кратных 5
            double param = e.GetPosition(this.DesignerCanvas).X;
            if (param % 5 > 0)
            {
                int i = (int)param / 5;
                double delta = param - i * 5;
                param = delta >= 2.5 ? param + 5 - delta : param - delta;
            }
            dragItem.X = param;
            param = e.GetPosition(this.DesignerCanvas).Y;
            if (param % 5 > 0)
            {
                int i = (int)param / 5;
                double delta = param - i * 5;
                param = delta >= 2.5 ? param + 5 - delta : param - delta;
            }
            dragItem.Y = param;

            // Получение ContentPresenter и Thumb соответствующего итема для выравнивания координат
            ContentPresenter contentPresenter = this.DesignerCanvas.Children.OfType<ContentPresenter>()
                .First(cp => cp.Content.Equals(dragItem));
            contentPresenter.Loaded += this.ContentPresenterOnLoaded;

            foreach (var element in this.TabViewModel.ElementCollection)
            {
                element.IsSelected = false;
            }
            dragItem.IsSelected = true;
            CommandManager.InvalidateRequerySuggested();
        }

        // Выравнивание координат элемента,чтобы он был ровно по сетке
        private void ContentPresenterOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ContentPresenter presenter = sender as ContentPresenter;

            ILogicElementViewModel item = presenter?.Content as ILogicElementViewModel;
            if (item == null)
                return;

            Thumb thumb = CommonHelper.GetThumbOfPresenter(presenter);
            Rect boundsRect = VisualTreeHelper.GetDescendantBounds(thumb);
            Rect thumbRect = thumb.TransformToAncestor(this.DesignerCanvas).TransformBounds(boundsRect);
            if (thumbRect.X % 5 > 0)
            {
                item.X += 5 - thumbRect.X % 5;
            }
            if (thumbRect.Y % 5 > 0)
            {
                item.Y += 5 - thumbRect.Y % 5;
            }
            presenter.Loaded -= this.ContentPresenterOnLoaded;
        }
    }
}