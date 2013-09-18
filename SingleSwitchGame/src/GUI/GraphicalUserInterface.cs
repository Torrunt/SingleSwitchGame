namespace SingleSwitchGame
{
    class GraphicalUserInterface : DisplayObject
    {
        protected Game Game;

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
            Deinit();
        }
    }
}
