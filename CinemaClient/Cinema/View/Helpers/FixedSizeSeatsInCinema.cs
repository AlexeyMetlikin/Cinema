using System;
using System.Windows;
using System.Windows.Controls;

namespace Cinema.View.Helpers
{
    public class FixedSizeSeatsInCinema : Panel
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FixedSizeSeatsInCinema), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var fixedLengthStackPanel = (FixedSizeSeatsInCinema)sender;
            fixedLengthStackPanel.OnOrientationChanged((Orientation)e.NewValue, (Orientation)e.OldValue);
        }

        private void OnOrientationChanged(Orientation newValue, Orientation oldValue)
        {
            InvalidateVisual();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size itemDesiredSize = new Size();
            Size itemSize = GetItemSize(availableSize);
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(itemSize);
                itemDesiredSize = child.DesiredSize;
            }
            return itemDesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size itemSize = GetItemSize(finalSize);
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i];
                Point itemPosition = GetItemPosition(itemSize, i);
                var itemRect = new Rect(itemPosition, itemSize);
                child.Arrange(itemRect);
            }
            return finalSize;
        }

        private Size GetItemSize(Size panelSize)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    return new Size(panelSize.Width / InternalChildren.Count, panelSize.Height);
                case Orientation.Vertical:
                    return new Size(panelSize.Width, panelSize.Height / InternalChildren.Count);
                default:
                    throw new Exception();
            }
        }

        private Point GetItemPosition(Size itemSize, int index)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    return new Point(index * itemSize.Width, 0.0);
                case Orientation.Vertical:
                    return new Point(0.0, index * itemSize.Height);
                default:
                    throw new Exception();
            }
        }
    }
}
