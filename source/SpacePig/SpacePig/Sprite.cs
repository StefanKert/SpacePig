using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePig
{
    public class Sprite
    {
        public Rectangle Size;
        public float Scale = 1.0f;
        public Vector2 Position = new Vector2(0,0);
      
        private Texture2D _spriteTexture;

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            this._spriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            Size = new Rectangle(0, 0, (int)(this._spriteTexture.Width * Scale), (int)(this._spriteTexture.Height * Scale));
        }

        public void Draw(SpriteBatch scriptBatch)
        {
            scriptBatch.Draw(this._spriteTexture, this.Position, new Rectangle(0, 0, this._spriteTexture.Width, this._spriteTexture.Height), Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
