using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Flight
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        static double myRound(double x, int precision)
        {
            return ((int)(x * Math.Pow(10.0, precision)) / Math.Pow(10.0, precision));
        }
        static int GetDecimalDigitsCount(double number)
        {
            string[] str = number.ToString(new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." }).Split('.');
            return str.Length == 2 ? str[1].Length : 0;
        }
        /*struct Coords
        {
            public Coords(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; set; }
            public double Y { get; set; }
        }
        List<Coords> Coordinates = new List<Coords>();*/
        double acceleration_of_gravity, steps, starting_speed, angle,
            drag_coefficient, medium_density, body_radius, resistance_force, cofficient_width, cofficient_height, x = 0,y = 0, 
            actual_width, actual_height, coordsx = 0, coordsy = 0, speedx, speedy, speed = 0;
        int how_many_decimal_places = 2, index = int.MinValue;
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }


        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(OnTimer);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 800);
        }

        private static readonly Regex _regex = new Regex("[^0-9,]");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void Index_RadioButton()
        {
            if (sphere_radiobutton.IsChecked == true)
                index = 1;
            if (cone_radiobutton.IsChecked == true)
                index = 2;
            if (cube_radiobutton.IsChecked == true)
                index = 3;
            if (cylinder_radiobutton.IsChecked == true)
                index = 4;
        }
        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            Index_RadioButton();
            if (speed_textbox.Text != "" && angle_textbox.Text != "" && steps_textbox.Text != ""
                && acceleration_of_gravity_textbox.Text != "" && medium_density_textbox.Text != ""
                && body_radius_textbox.Text != "" && index != int.MinValue)
            {
                Output();
            }
            else
                MessageBox.Show("Не до конца заполнили начальные данные", "Body flight", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            speed_textbox.Text = "";
            angle_textbox.Text = "";
            steps_textbox.Text = "0,01";
            acceleration_of_gravity_textbox.Text = "9,81";
            medium_density_textbox.Text = "1,2754";
            body_radius_textbox.Text = "";
            sphere_radiobutton.IsChecked = cone_radiobutton.IsChecked = cube_radiobutton.IsChecked = cylinder_radiobutton.IsChecked = false;
        }
        private void Input()
        {
            starting_speed = Convert.ToDouble(speed_textbox.Text);
            angle = Math.PI * Convert.ToDouble(angle_textbox.Text) / 180;
            steps = Convert.ToDouble(steps_textbox.Text);
            how_many_decimal_places = GetDecimalDigitsCount(steps);
            acceleration_of_gravity = Convert.ToDouble(acceleration_of_gravity_textbox.Text);
            medium_density = Convert.ToDouble(medium_density_textbox.Text);
            body_radius = Convert.ToDouble(body_radius_textbox.Text);
            do
            {
                switch (index)
                {
                    case 1:
                        drag_coefficient = 0.47;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    case 2:
                        drag_coefficient = 0.5;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    case 3:
                        drag_coefficient = 1.05;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    case 4:
                        drag_coefficient = 0.82;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    default:
                        index = int.MinValue;
                        break;
                }
            } while (index == int.MinValue);
        }
        void Calculation_of_the_resistance_force(double speed)
        {
            if (index == 1)
                resistance_force = drag_coefficient * medium_density * speed * speed / 2
                    * Math.PI * body_radius * body_radius;
            if (index == 2)
                resistance_force = drag_coefficient * medium_density * speed * speed / 2 * Math.PI * body_radius * body_radius * 2
                    * body_radius / 3;
            if (index == 3)
                resistance_force = drag_coefficient * medium_density * speed * speed / 2 * body_radius * body_radius;
            if (index == 4)
                resistance_force = drag_coefficient * medium_density * speed * speed * Math.PI * body_radius * body_radius * body_radius;
        }

        private void OnTimer(Object sender, EventArgs args)
        {
            if (coordsy >= 0)
            {
                coordsx = coordsx + steps * speedx;
                x = actual_width + 1000 * coordsx;
                speedx = speedx - steps * resistance_force * speedx;
                coordsy = coordsy + steps * speedy;
                y = actual_height + 1000 * coordsy * (-1);
                speedy = speedy - steps * (9.81 + resistance_force * speedy);
                if (coordsy <= 0 && speed != 0)
                {
                    y = actual_height;
                    polyline.Points.Add(new Point(x, y));
                    Canvas.SetLeft(elipse, polyline.Points.Last().X - elipse.Width / 2.0);
                    Canvas.SetTop(elipse, polyline.Points.Last().Y - elipse.Height / 2.0);
                    MessageBox.Show("Расстояние которое пролетело тело: " + myRound(coordsx, how_many_decimal_places), "Body flight", MessageBoxButton.OK, MessageBoxImage.Information);
                    timer.Stop();
                }
                else
                {
                    polyline.Points.Add(new Point(x, y));
                    Canvas.SetLeft(elipse, polyline.Points.Last().X - elipse.Width / 2.0);
                    Canvas.SetTop(elipse, polyline.Points.Last().Y - elipse.Height / 2.0);
                    if (polyline.Points.Count == 1)
                    {
                        elipse.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        elipse.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    }
                    if (speed == 0)
                    {
                        speedx = starting_speed * Math.Cos(angle);
                        speedy = starting_speed * Math.Sin(angle);
                    }
                    speed = Math.Sqrt(speedx * speedx + speedy * speedy);
                    Calculation_of_the_resistance_force(speed);
                }
            }
            else
            {
                y = actual_height;
                polyline.Points.Add(new Point(x, y));
                Canvas.SetLeft(elipse, polyline.Points.Last().X - elipse.Width / 2.0);
                Canvas.SetTop(elipse, polyline.Points.Last().Y - elipse.Height / 2.0);
                MessageBox.Show("Расстояние которое пролетело тело: " + myRound(coordsx, how_many_decimal_places), "Body flight", MessageBoxButton.OK, MessageBoxImage.Information);
                timer.Stop();
            }
        }
        /*private void CountingcCordinates()
        {
            double speed;
            double speedx = starting_speed * Math.Cos(angle);
            double speedy = starting_speed * Math.Sin(angle);
            Coords coords = new Coords(0, 0);
            Coordinates.Add(coords);
            cofficient_width = canvas.ActualWidth / 2 / 2 * Math.Sin(100) * (100 + Math.Sin(100 * 100 / 2));
            cofficient_height = canvas.ActualHeight / 2 / 2 * Math.Cos(100) * (100 + Math.Sin(100 * 100 / 2));
            while (coords.Y >= 0)
            {
                coords.X = coords.X + steps * speedx;
                speedx = speedx - steps * resistance_force * speedx;
                coords.Y = coords.Y + steps * speedy;
                speedy = speedy - steps * (9.81 + resistance_force * speedy);
                if (coords.Y <= 0)
                    break;
                Coordinates.Add(coords);
                speed = Math.Sqrt(speedx * speedx + speedy * speedy);
                Calculation_of_the_resistance_force(speed);
            }
            coords.Y = 0;
            Coordinates.Add(coords);
            MessageBox.Show("Расстояние которое пролетело тело: " + myRound(coords.X, how_many_decimal_places), "Body flight", MessageBoxButton.OK, MessageBoxImage.Information);
            MessageBox.Show("Время полёта: " + myRound(steps * Coordinates.Count,how_many_decimal_places), "Body flight", MessageBoxButton.OK, MessageBoxImage.Information);
        }*/

        private void clear()
        {
            speed = 0;
            speedx = 0;
            speedy = 0;
            coordsx = 0;
            coordsy = 0;
            elipse.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            elipse.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
        private void Output()
        {
            Input();
            //CountingcCordinates();
            polyline.Points.Clear();
            actual_width = canvas.ActualWidth / 2;
            actual_height = canvas.ActualHeight / 2;
            clear();
            timer.Start();
            /*string path = @"..\..\..\" + file_name + ".csv";
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                for (int i = 1; i < Coordinates.Count + 1; ++i)
                {
                    sw.Write("x(");
                    sw.Write(myRound(steps * Convert.ToDouble(i), how_many_decimal_places));
                    sw.Write(") = ");
                    sw.Write(myRound(Coordinates[i - 1].X, how_many_decimal_places));
                    sw.Write("\t" + "y(");
                    sw.Write(myRound(steps * Convert.ToDouble(i), how_many_decimal_places));
                    sw.Write(") = ");
                    sw.Write(myRound(Coordinates[i - 1].Y, how_many_decimal_places));
                    sw.Write("\n");
                }
            }*/
        }
    }
}
