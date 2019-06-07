using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using basketball_game.Models;
using System.Windows.Input;

namespace basketball_game.Utilities
{
    public class GameUtils : ObjectMesh
    {
        public static float Constrain(float val, float min, float max)
        {
            if (val <= min) return min;
            else if (val >= max) return max;
            else return val;
        }
        public static float GetDistanceBetween(ObjectMesh firstObject, ObjectMesh secondObject)
        {
            var x = secondObject.Position.x - firstObject.Position.x;
            var y = secondObject.Position.y - firstObject.Position.y;
            var distance = (float)Math.Sqrt((x * x) + (y * y));
            return distance;
        }

        public static double GetAngleBetween(ObjectMesh firstObject, ObjectMesh secondObject)
        {
            var x = secondObject.Position.x - firstObject.Position.x;
            var y = secondObject.Position.y - firstObject.Position.y;
            var angle = Math.Tan(y / x);
            return angle;
        }

    }

}
