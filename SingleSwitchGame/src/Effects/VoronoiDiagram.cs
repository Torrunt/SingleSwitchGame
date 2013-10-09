using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class VoronoiDiagram : Entity
    {
        private List<List<Vector2f>> Points = new List<List<Vector2f>>();

        public VoronoiDiagram(Game game, Vector2f Size) : base(game)
        {
            // Generate
            
        }

    }
}
