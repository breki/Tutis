using System;

namespace TreasureChest
{
    internal class PooledObject
    {
        public PooledObject(object instance)
        {
            this.instance = instance;
        }

        public object Instance
        {
            get { return instance; }
        }

        public bool IsNotInUse
        {
            get { return !isInUse; }
        }

        public void Reserve()
        {
            if (isInUse)
                throw new InvalidOperationException("The object is already in use.");

            isInUse = true;
        }

        public void Release()
        {
            if (!isInUse)
                throw new InvalidOperationException("The object was not in use.");

            isInUse = false;
        }

        private object instance;
        private bool isInUse;
    }
}