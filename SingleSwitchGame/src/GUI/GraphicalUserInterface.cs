using System;

namespace SingleSwitchGame
{
    class GraphicalUserInterface : DisplayObject
    {
        protected Game Game;

        public event EventHandler Removed;

        public GraphicalUserInterface(Game Game)
        {
            this.Game = Game;
            Init();
        }
        public virtual void Init()
        {
        }
        public virtual void Deinit()
        {
        }
        public override void OnRemoved()
        {
            if (Removed != null)
                Removed(this, EventArgs.Empty);
            Deinit();
        }
    }
}
