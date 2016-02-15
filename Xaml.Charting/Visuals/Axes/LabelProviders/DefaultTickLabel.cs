// *************************************************************************************
// ULTRACHART� � Copyright ulc software Services Ltd. 2011-2014. All rights reserved.
//  
// Web: http://www.ultrachart.com
// Support: support@ultrachart.com
// 
// DefaultTickLabel.cs is part of Ultrachart, a High Performance WPF & Silverlight Chart. 
// For full terms and conditions of the license, see http://www.ultrachart.com/ultrachart-eula/
// 
// This source code is protected by international copyright law. Unauthorized
// reproduction, reverse-engineering, or distribution of all or any portion of
// this source code is strictly prohibited.
// 
// This source code contains confidential and proprietary trade secrets of
// ulc software Services Ltd., and should at no time be copied, transferred, sold,
// distributed or made available without express written permission.
// *************************************************************************************
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Ecng.Xaml.Charting.Themes;
using Ecng.Xaml.Charting.Visuals.Annotations;
using Ecng.Xaml.Charting.Visuals.RenderableSeries;

namespace Ecng.Xaml.Charting.Visuals.Axes
{
    /// <summary>
    /// Provides a class for tick axis labels rendering.
    /// </summary>
    public class DefaultTickLabel : TemplatableControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Defines the HorizontalAnchorPoint DependencyProperty
        /// </summary>
        public static readonly DependencyProperty HorizontalAnchorPointProperty = DependencyProperty.Register("HorizontalAnchorPoint", typeof(HorizontalAnchorPoint), typeof(DefaultTickLabel), new PropertyMetadata(HorizontalAnchorPoint.Center, OnHorizontalAnchorPointChanged));

        /// <summary>
        /// Defines the VerticalAnchorPoint DependencyProperty
        /// </summary>
        public static readonly DependencyProperty VerticalAnchorPointProperty = DependencyProperty.Register("VerticalAnchorPoint", typeof(VerticalAnchorPoint), typeof(DefaultTickLabel), new PropertyMetadata(VerticalAnchorPoint.Top, OnVerticalAnchorPointChanged));

        /// <summary>
        /// Defines the Position DependencyProperty
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(DefaultTickLabel), new PropertyMetadata(new Point(double.NaN, double.NaN), OnPositionChanged));

        /// <summary>
        /// Defines the DefaultForeground DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DefaultForegroundProperty = DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(DefaultTickLabel), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Defines the DefaultVerticalAnchorPoint DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DefaultVerticalAnchorPointProperty = DependencyProperty.Register("DefaultVerticalAnchorPoint", typeof(VerticalAnchorPoint), typeof(DefaultTickLabel), new PropertyMetadata(default(VerticalAnchorPoint)));

        /// <summary>
        /// Defines the DefaultHorizontalAnchorPoint DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DefaultHorizontalAnchorPointProperty = DependencyProperty.Register("DefaultHorizontalAnchorPoint", typeof(HorizontalAnchorPoint), typeof(DefaultTickLabel), new PropertyMetadata(default(HorizontalAnchorPoint)));

        /// <summary>
        /// Defines the LayoutTransform DependencyProperty
        /// </summary>
        public
#if !SILVERLIGHT
            new
#endif
            static readonly DependencyProperty LayoutTransformProperty = DependencyProperty.Register("LayoutTransform",
                typeof (Transform), typeof (DefaultTickLabel), new PropertyMetadata(null));

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTickLabel"/> class.
        /// </summary>
        /// <remarks></remarks>
        public DefaultTickLabel()
        {
            DefaultStyleKey = typeof(DefaultTickLabel);
        }

        /// <summary>
        /// Gets or sets the <see cref="HorizontalAnchorPoint" />.
        /// The value of Left means the X1,Y1 coordinate of the annotation is on the Left horizontally.
        /// The value of Center means the X1,Y1 coordinate of the annotation is at the center horizontally.
        /// The value of Right means the X1,Y1 coordinate of the annotation is at the right horizontally.
        /// </summary>
        public HorizontalAnchorPoint HorizontalAnchorPoint
        {
            get { return (HorizontalAnchorPoint)GetValue(HorizontalAnchorPointProperty); }
            set { SetValue(HorizontalAnchorPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="VerticalAnchorPoint" />.
        /// The value of Top means the X1,Y1 coordinate of the annotation is on the Top vertically.
        /// The value of Center means the X1,Y1 coordinate of the annotation is at the center vertically.
        /// The value of Bottom means the X1,Y1 coordinate of the annotation is at the Bottom vertically.
        /// </summary>
        public VerticalAnchorPoint VerticalAnchorPoint
        {
            get { return (VerticalAnchorPoint)GetValue(VerticalAnchorPointProperty); }
            set { SetValue(VerticalAnchorPointProperty, value); }
        }

        public Point Position
        {
            get { return (Point) GetValue(PositionProperty); }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        public Brush DefaultForeground
        {
            get { return (Brush) GetValue(DefaultForegroundProperty); }
        }

        public VerticalAnchorPoint DefaultVerticalAnchorPoint
        {
            get { return (VerticalAnchorPoint) GetValue(DefaultVerticalAnchorPointProperty); }
        }

        public HorizontalAnchorPoint DefaultHorizontalAnchorPoint
        {
            get { return (HorizontalAnchorPoint) GetValue(DefaultHorizontalAnchorPointProperty); }
        }

        internal int CullingPriority { get; set; }

        internal Rect ArrangedRect { get; set; }

        /// <summary>
        /// Implementation of <see cref="INotifyPropertyChanged" />
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnHorizontalAnchorPointChanged(d, new DependencyPropertyChangedEventArgs());
            OnVerticalAnchorPointChanged(d, new DependencyPropertyChangedEventArgs());
        }

        private static void OnHorizontalAnchorPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = d as DefaultTickLabel;
            if (label != null)
            {
                label.SetValue(AxisCanvas.LeftProperty, double.NaN);
                label.SetValue(AxisCanvas.RightProperty, double.NaN);
                label.SetValue(AxisCanvas.CenterLeftProperty, double.NaN);
                label.SetValue(AxisCanvas.CenterRightProperty, double.NaN);

                switch (label.HorizontalAnchorPoint)
                {
                    case HorizontalAnchorPoint.Left:
                        label.SetValue(AxisCanvas.LeftProperty, label.Position.X);
                        break;
                    case HorizontalAnchorPoint.Right:
                        label.SetValue(AxisCanvas.RightProperty, label.Position.X);
                        break;
                    case HorizontalAnchorPoint.Center:
                        label.SetValue(AxisCanvas.CenterLeftProperty, label.Position.X);
                        break;
                }
            }
        }

        private static void OnVerticalAnchorPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = d as DefaultTickLabel;
            if (label != null)
            {
                label.SetValue(AxisCanvas.TopProperty, double.NaN);
                label.SetValue(AxisCanvas.BottomProperty, double.NaN);
                label.SetValue(AxisCanvas.CenterTopProperty, double.NaN);
                label.SetValue(AxisCanvas.CenterBottomProperty, double.NaN);

                switch (label.VerticalAnchorPoint)
                {
                    case VerticalAnchorPoint.Top:
                        label.SetValue(AxisCanvas.TopProperty, label.Position.Y);
                        break;
                    case VerticalAnchorPoint.Bottom:
                        label.SetValue(AxisCanvas.BottomProperty, label.Position.Y);
                        break;
                    case VerticalAnchorPoint.Center:
                        label.SetValue(AxisCanvas.CenterTopProperty, label.Position.Y);
                        break;
                }
            }
        }
    }
}