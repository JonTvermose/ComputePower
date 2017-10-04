using System;

namespace ComputePower.Computation.Models
{
    public class Body
    {
        public float Mass;
        public double PX, PY, PZ;
        private double VX, VY, VZ;
        private double FX, FY, FZ;

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
    }


}
