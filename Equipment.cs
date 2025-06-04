using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the equipment abstract class that inherits from Item, it has 3 extra attributes.
    /// </summary>
    public abstract class Equipment : Item
    {
        private int _durability;
        protected static Raylib_cs.Texture2D _texture;
        protected static bool _textureLoaded = false;
        /// <summary>
        /// Parameterized constructor for the Equipment class to set the itemID, quantity, name, description, tier, price, and durability.
        /// </summary>
        public Equipment(int itemID, int quantity, string name, string description, int tier, int price, int durability) : base(itemID, quantity, name, description, tier, price)
        {
            _durability = durability;
        }
        /// <summary>
        /// Property methotd to get and set the durability of the equipment.
        /// </summary>
        public int Durability
        {
            get { return _durability; }
            set { _durability = value; }
        }
        /// <summary>
        /// Abstract method is to get the bonus description of the equipment when it is equipped.
        /// </summary>
        public abstract string Equipped();
        /// <summary>
        /// Abstract method to load the texture of the equipment.
        /// </summary>
        public abstract void LoadTexture();
        /// <summary>
        /// Virtual method to get the texture of the equipment, it checks if the texture is loaded, if not it loads it.
        /// </summary>
        public virtual Raylib_cs.Texture2D GetTexture()
        {
            if (!_textureLoaded)
            {
                LoadTexture();
            }
            return _texture;
        }
    }
}