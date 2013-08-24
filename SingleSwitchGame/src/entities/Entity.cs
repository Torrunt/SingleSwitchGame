using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class Entity : DisplayObject
    {
        protected Game game;
        public Sprite model;

        public Entity(Game game, Sprite model)
        {
            this.game = game;
            
            if (model != null)
                this.model = model;
            else
                this.model = new Sprite();
            addChild(this.model);

            init();
        }
        public virtual void init()
        {
            game.addToUpdateList(this);
        }
        public virtual void deinit()
        {
            game.removeFromUpdateList(this);
        }

        public virtual void update(float dt) { }
        
    }
}
