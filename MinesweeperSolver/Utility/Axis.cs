using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackMine.Utility
{
    public class UAxis
    {
        Keys? last = null;
        List<Keys> queue = new List<Keys>();
        public Keys? Read()
        {
            var result = last;
            lock (queue)
            {
                if (queue.Count == 0)
                    last = null;
                else
                    last = queue.Last();
            }
            return result;
        }
        public void KeyDown(Keys key)
        {
            last = key;
            lock (queue)
            {
                queue.Add(key);
            }
        }
        public void KeyUp(Keys key)
        {
            lock (queue)
            {
                queue.RemoveAll(q => q == key);
            }
        }
        public void Reset()
        {
            last = null;
            lock (queue)
            {
                queue.Clear();
            }
        }
    }
    public class UButton
    {
        private bool _pressed = false;
        private double ignore = 0;
        public bool Read(double deltaTime)
        {
            ignore -= deltaTime;
            if (ignore < 0)
                ignore = 0;
            if (ignore > 0)
                return false;

            if (_pressed)
                ignore = 0.2;
            var result = _pressed;
            _pressed = false;
            return result;
        }
        public void KeyDown()
        {
            _pressed = true;
        }
        public void KeyUp()
        {
            if (ignore <= 0)
                _pressed = false;
        }
        public void Reset()
        {
            _pressed = false;
        }
    }
    public class USwitch
    {
        private bool _pressed = false;
        private bool _wasRead = false;
        public bool Read (double deltaTime)
        {
            var result = _pressed && !_wasRead;
            if (result)
            {
                _pressed = false;
                _wasRead = true;
            }
            return result;
        }
        public void KeyDown ()
        {
            _pressed = true;
        }
        public void KeyUp()
        {
            _wasRead = false;
            _pressed = false;
        }
        public void Reset ()
        {
            _wasRead = false;
            _pressed = false;
        }
    }
}
