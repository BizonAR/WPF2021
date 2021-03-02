using System;

namespace Trajectory
{
    class Trajectory
    {
        double speed, time, x, y, angle, steps;
        public Trajectory()
        {
            Console.WriteLine("Введите скорость");
            speed = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите угол в градусах");
            angle = Math.PI * Convert.ToDouble(Console.ReadLine()) / 180;
            Console.WriteLine("Введите время полёта");
            time = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите шаг изменения времени");
            steps = Convert.ToDouble(Console.ReadLine());
        }
        public void output(double time)
        {
            x = speed * Math.Cos(angle) * time;
            y = speed * Math.Sin(angle) * time - 4.9 * time * time;
            if (y <= 0)
                y = 0;
            Console.WriteLine("x(" + time + ") = " + x + " y(" + time + ") = " + y);
        }
        public void cycle(double time, double steps)
        {
            for (double i = 0; i < time; i += steps)
            {
                output(i);
            }
            output(time);
        }
        public static void Main(string[] args)
        {
            Trajectory trajectory = new Trajectory();
            trajectory.cycle(trajectory.time, trajectory.steps);
        }
    }
}
