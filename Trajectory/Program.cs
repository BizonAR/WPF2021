using System;
using System.IO;
using System.Collections.Generic;

namespace Trajectory
{
    /*class Trajectory1
    {
        public double speed, time, x, y, angle, steps;
        string path;
        public Trajectory1()
        {
            Console.WriteLine("Введите название файла, у которого установлено расширение csv");
            path = @"C:\Users\mikaa\source\repos\Trajectory\Trajectory\" + Console.ReadLine() + ".csv";
            Console.WriteLine("Введите скорость");
            speed = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите угол в градусах");
            angle = Math.PI * Convert.ToDouble(Console.ReadLine()) / 180;
            Console.WriteLine("Введите время полёта");
            time = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите шаг изменения времени");
            steps = Convert.ToDouble(Console.ReadLine());
            speed = reading_from_file(path, 0);
            angle = Math.PI * reading_from_file(path, 1) / 180;
            time = reading_from_file(path, 2);
            steps = reading_from_file(path, 3);
        }
        public double reading_from_file(string path, int i)
        {
            string line = null;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                for (int j = 0; j <= i; ++j)
                {
                    if ((line = sr.ReadLine()) == null)
                    {
                        break;
                    }
                }
            }
            if (line != null)
                return Convert.ToDouble(line);
            else return 0;
        }
        public void out_put(double time)
        {
            x = speed * Math.Cos(angle) * time;
            y = speed * Math.Sin(angle) * time - 4.9 * time * time;
            if (y <= 0)
                y = 0;
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                sw.Write("\n" + "x(");
                sw.Write("{0:0.##}", time);
                sw.Write(") = ");
                sw.Write("{0:0.##}", x);
                sw.Write("\t" + "y(");
                sw.Write("{0:0.##}", time);
                sw.Write(") = ");
                sw.Write("{0:0.##}", y); 
            }
        }
        public void cycle(double time, double steps)
        {
            for (double i = 0; i < time; i += steps)
            {
                out_put(i);
            }
            out_put(time);
        }
    }*/


    class Bodyflight
    {
        static int GetDecimalDigitsCount(double number)
        {
            string[] str = number.ToString(new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." }).Split('.');
            return str.Length == 2 ? str[1].Length : 0;
        }
        struct Coords
        {
            public Coords(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; set; }
            public double Y { get; set; }
        }
        List<Coords> Coordinates = new List<Coords>();
        double acceleration_of_gravity = 9.81, steps = 0.01, starting_speed, angle,
            drag_coefficient, medium_density = 1.2754, body_radius, resistance_force, high;
        int caseSwitch, how_many_decimal_places = 2;
        private void Input()
        {
            Console.WriteLine("Введите начальную скорость");
            starting_speed = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите угол в градусах");
            angle = Math.PI * Convert.ToDouble(Console.ReadLine()) / 180;
            Console.WriteLine("Введите шаг изменения времени или оставьте стандартным - 0.01");
            string test = "";
            if ((test = Console.ReadLine()) != "")
            {
                steps = Convert.ToDouble(test);
                test = "";
                how_many_decimal_places = GetDecimalDigitsCount(steps);
            }
            Console.WriteLine("Введите ускорение свободного падения или оставьте стандартным - 9.81");
            if ((test = Console.ReadLine()) != "")
            {
                acceleration_of_gravity = Convert.ToDouble(test);
                test = "";
            }
            Console.WriteLine("Введите коэффциент плотности среды или оставьте стандартным - 1.2754");
            if ((test = Console.ReadLine()) != "")
            {
                medium_density = Convert.ToDouble(test);
                test = "";
            }
            Console.WriteLine("Введите радиус тела");
            body_radius = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Выберите один из типов тела:");
            Console.WriteLine("1 - Сфера");
            Console.WriteLine("2 - Конус 2:1 (остриём к потоку)");
            Console.WriteLine("3 - Куб (поверхностью к потоку)");
            Console.WriteLine("4 - Цилиндр (длина равна двум диаметрам, торцом к потоку)");
            caseSwitch = Convert.ToInt16(Console.ReadLine());
            do
            {
                switch (caseSwitch)
                {
                    case 1:
                        Console.WriteLine("Вы выбрали сферу, её коэффициент сопротивления формы равен 0.47");
                        drag_coefficient = 0.47;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    case 2:
                        Console.WriteLine("Вы выбрали конус 2:1 (остриём к потоку), его коэффициент сопротивления формы равен 0.50");
                        drag_coefficient = 0.5;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    case 3:
                        Console.WriteLine("Вы выбрали куб (поверхностью к потоку), его коэффициент сопротивления формы равен 1.05");
                        drag_coefficient = 1.05;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    case 4:
                        Console.WriteLine("Вы выбрали цилиндр (длина равна двум диаметрам, торцом к потоку), " +
                            "его коэффициент сопротивления формы равен 0.82");
                        drag_coefficient = 0.82;
                        Calculation_of_the_resistance_force(starting_speed);
                        break;
                    default:
                        Console.WriteLine("Введено не правильное число");
                        caseSwitch = int.MinValue;
                        break;
                }
            } while (caseSwitch == int.MinValue);
        }

        void Calculation_of_the_resistance_force(double speed)
        {
            if (caseSwitch == 1)
                resistance_force = drag_coefficient * medium_density * speed * speed / 2
                    * Math.PI * body_radius * body_radius;
            if (caseSwitch == 2)
                resistance_force = drag_coefficient * medium_density * speed * speed / 2 * Math.PI * body_radius * body_radius * 2
                    * body_radius / 3;
            if (caseSwitch == 3)
                resistance_force = drag_coefficient * medium_density * speed * speed / 2 * body_radius * body_radius;
            if (caseSwitch == 4)
                resistance_force = drag_coefficient * medium_density * speed * speed * Math.PI * body_radius * body_radius * body_radius;
        }
        private void CountingcCordinates()
        {
            double speed;
            double speedx = starting_speed * Math.Cos(angle);
            double speedy = starting_speed * Math.Sin(angle);
            Coords coords = new Coords(0, 0);
            Coordinates.Add(coords);
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
        }
        public void Output()
        {
            string path = @"..\..\..\Trajectory.csv";
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                Input();
                CountingcCordinates();
                for (int i = 1; i < Coordinates.Count + 1; ++i)
                {
                    sw.Write("x(");
                    sw.Write(Math.Round(steps * Convert.ToDouble(i),how_many_decimal_places));
                    sw.Write(") = ");
                    sw.Write(Math.Round(Coordinates[i - 1].X, how_many_decimal_places));
                    sw.Write("\t" + "y(");
                    sw.Write(Math.Round(steps * Convert.ToDouble(i), how_many_decimal_places));
                    sw.Write(") = ");
                    sw.Write(Math.Round(Coordinates[i - 1].Y, how_many_decimal_places));
                    sw.Write("\n");
                }
            }
        }
        public Bodyflight() { }
    }

    class Trajectory
    {
        public static void Main(string[] args)
        {
            Bodyflight trajectory = new Bodyflight();
            trajectory.Output();
        }
    }
}
