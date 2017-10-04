using System;

namespace ComputePower.Computation.Models
{
    public class Body
    {
        public double Mass;
        public double PX, PY, PZ;
        private double VX, VY, VZ;
        private double FX, FY, FZ;

        public Body(double x, double y, double z)
        {
            PX = x;
            PY = y;
            PZ = z;
        }
        /// <summary>
        /// Generate a random Body
        /// </summary>
        public Body(double radius, double maxMass)
        {
            Random r = new Random();
            // Random mass
            Mass = r.NextDouble() * maxMass;

            // Random radius
            var radi = r.NextDouble() * radius;

            // Random position
            var x = r.NextDouble();
            var y = r.NextDouble();
            var z = r.NextDouble();
            var factor = 1 / Math.Sqrt(x * x + y * y + z * z) * radi;
            PX = x * factor;
            PY = y * factor;
            PZ = z * factor;

            VX = 0.0;
            VY = 0.0;
            VZ = 0.0;
        }

        public void AddForce(Body b)
        {
            double EPS = 3E4; // Softening parameter to avoid infinities

            // Calculate distance between bodies
            double dx = PX - b.PX;
            double dy = PY - b.PY;
            double dz = PZ - b.PZ;
            double dist = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            // Calculate the absolute force
            double F = (6.673e-11 * Mass * b.Mass) / (dist * dist + EPS);

            // Add the force to each direction
            FX += F * dx / dist;
            FY += F * dy / dist;
            FZ += F * dz / dist;
        }

        public void ResetForce()
        {
            FX = 0;
            FY = 0;
            FZ = 0;
        }

        public void Update(double deltaTime)
        {
            VX += deltaTime * FX / Mass;
            VY += deltaTime * FY / Mass;
            VZ += deltaTime * FZ / Mass;

            PX += deltaTime * VX;
            PY += deltaTime * VY;
            PZ += deltaTime * VZ;
        }

        public static Body operator - (Body b1, Body b2)
        {
            return new Body(b1.PX - b2.PX, b1.PY - b2.PY, b1.PZ - b2.PZ);
        }
    }


}
