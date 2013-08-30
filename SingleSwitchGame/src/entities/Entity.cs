using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class Entity : DisplayObject
    {
        protected Game game;
        public Sprite Model;

        public Entity(Game game, Sprite model)
        {
            this.game = game;
            
            if (model != null)
                this.Model = model;
            else
                this.Model = new Sprite();
            AddChild(this.Model);

            Init();
        }
        public virtual void Init()
        {
            game.AddToUpdateList(this);
        }
        public virtual void Deinit()
        {
            game.RemoveFromUpdateList(this);
        }

        public virtual void Update(float dt) { }
        
    }
}
