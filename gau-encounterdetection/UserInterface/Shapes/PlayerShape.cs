﻿using Data.Gameobjects;
using EDGUI.Utils;
using MathNet.Spatial.Units;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Shapes
{
    class PlayerShape : Shape
    {
        /// <summary>
        /// Line of sight length of a playershape
        /// </summary>
        private const int LOS_LENGTH = 40;

        public bool Active { get; set; }

        public double Radius { get; set; }

        public double Yaw
        {
            get { return (double)GetValue(yawProperty); }
            set { SetValue(yawProperty, value); }
        }

        public string Playerlevel
        {
            get { return (string)GetValue(plProperty); }
            set { SetValue(plProperty, value); }
        }

        public double X
        {
            get { return (double)GetValue(xProperty); }
            set { SetValue(xProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(yProperty); }
            set { SetValue(yProperty, value); }
        }

        #region Properties
        // DependencyProperty - Yaw
        private static FrameworkPropertyMetadata yawMetadata =
                new FrameworkPropertyMetadata(
                    90.0,     // Default value
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,    // Property changed callback
                    null);   // Coerce value callback

        public static readonly DependencyProperty yawProperty =
            DependencyProperty.Register("Yaw", typeof(double), typeof(PlayerShape), yawMetadata);



        // DependencyProperty - Yaw
        private static FrameworkPropertyMetadata plMetadata =
                new FrameworkPropertyMetadata(
                    "",     // Default value
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,    // Property changed callback
                    null);   // Coerce value callback

        public static readonly DependencyProperty plProperty =
            DependencyProperty.Register("Playerlevel", typeof(string), typeof(PlayerShape), plMetadata);


        // DependencyProperty - Position X
        private static FrameworkPropertyMetadata XMetadata =
                new FrameworkPropertyMetadata(
                    0.0,     // Default value
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,    // Property changed callback
                    null);   // Coerce value callback

        public static readonly DependencyProperty xProperty =
            DependencyProperty.Register("X", typeof(double), typeof(PlayerShape), XMetadata);


        // DependencyProperty - Position Y
        private static FrameworkPropertyMetadata YMetadata =
                new FrameworkPropertyMetadata(
                    0.0,     // Default value
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,    // Property changed callback
                    null);   // Coerce value callback

        public static readonly DependencyProperty yProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(PlayerShape), YMetadata);

#endregion  

        private Point aimPoint = new Point(0, 0);
        protected override Geometry DefiningGeometry
        {
            get
            {
                var aimX = (X + LOS_LENGTH * Math.Cos(Yaw)); // Aim vector from Yaw -> dont forget toRadian for this calc
                var aimY = (Y + LOS_LENGTH * Math.Sin(Yaw));

                aimPoint.X = aimX;
                aimPoint.Y = aimY;
                FormattedText text = new FormattedText(Playerlevel,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Tahoma"),
                        7,
                        Brushes.Black);
                Geometry textg = text.BuildGeometry(new Point(X-6, Y));
                Geometry line = new LineGeometry(new Point(X, Y), aimPoint);
                Geometry e = new EllipseGeometry(new Point(X, Y), Radius, Radius);
                GeometryGroup combined = new GeometryGroup();
                combined.Children.Add(e);
                combined.Children.Add(line);
                combined.Children.Add(textg);
                return combined;
            }
        }

        public void UpdatePlayer(Player p)
        {
            PlayerShape ps = this;
            if (p.HP <= 0)
                ps.Active = false;
            if (p.HP > 0)
                ps.Active = true;

            if (!ps.Active)
            {
                ps.Fill = new SolidColorBrush(UIColors.DEAD_PLAYER);
                ps.Stroke = new SolidColorBrush(UIColors.DEAD_PLAYER);
                return;
            }
            else
            {
                Color color;
                if (p.GetTeam() == Team.T)
                    color = UIColors.TEAM_1;
                else
                    color = UIColors.TEAM_2;
                ps.Fill = new SolidColorBrush(color);
                ps.Stroke = new SolidColorBrush(color);
            }

            if (p.IsSpotted)
            {
                if (p.GetTeam() == Team.T)
                    ps.Fill = new SolidColorBrush(Color.FromArgb(255, 225, 160, 160));
                else
                    ps.Fill = new SolidColorBrush(Color.FromArgb(255, 160, 160, 225));
            }
            else if (!p.IsSpotted)
            {

                if (p.GetTeam() == Team.T)
                    ps.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                else
                    ps.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            }
            //ps.Playerlevel = "" + EDAlgorithm.playerlevels[p.player_id].height;

            /*var vector = CoordinateConverter.GameToUIPosition(p.Position.SubstractZ());
            ps.X = vector.X;
            ps.Y = vector.Y;*/
            ps.Yaw = Angle.FromDegrees(-p.Facing.Yaw).Radians;

        }
    }
}