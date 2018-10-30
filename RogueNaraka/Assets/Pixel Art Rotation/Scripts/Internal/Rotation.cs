using UnityEngine;
using System.Collections;

namespace PixelArtRotation.Internal
{
    public class Rotation
    {
        /// <summary>
        /// Rotate the texture inside the sprite.
        /// </summary>
        public Sprite RotateTexture(Texture2D original, Vector2 pivot, FilterMode filterMode, int pixelsPerUnit, int angle, bool allowResize)
        {
            _oldArray = original.GetPixels32();
            //_newArray = new Color32[original.GetPixels32().Length];

            _oldWidth = original.width;
            _oldHeight = original.height;

            if (allowResize)
            {
                _width = _oldHeight > _oldWidth ? _oldHeight * 2 : _oldWidth * 2;
                _height = _width;
            }
            else
            {
                _width = _oldWidth;
                _height = _oldHeight;
            }

            _newArray = new Color32[_width * _height];

            _newTexture = new Texture2D(_width, _height);
            _newTexture.filterMode = _filterMode;
            _newTexture.name = original.name;

            _pivot = pivot;
            _filterMode = filterMode;
            _pixelsPerUnit = pixelsPerUnit;

            RotateSquare(Mathf.Deg2Rad * angle);

            //Then return the rotation.
            return ApplyRotation(_newArray);
        }

        /// <summary>
        /// Apply the new texture to the sprite.
        /// </summary>
        private Sprite ApplyRotation(Color32[] pix)
        {
            _newTexture.SetPixels32(pix);
            _newTexture.Apply(false);

            return Sprite.Create(_newTexture, new Rect(0f, 0f, _width, _height), new Vector2((_pivot.x / _oldWidth), (_pivot.y / _oldHeight)), _pixelsPerUnit);
            //(_pivot.x / _oldWidth), (_pivot.y / _oldHeight)
        }

        /// <summary>
        /// Rotate the texture by a rotation and traslation matrix.
        /// </summary>
        /// <param name="textureArray"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        private void RotateSquare(float phi)
        {
            int x, y, xc, yc, oxc, oyc;
            float sin, cos;

            x = 0;
            y = 0;
            sin = Mathf.Sin(phi);
            cos = Mathf.Cos(phi);

            //custom pivot
            xc = (int)_width / 2;
            yc = (int)_height / 2;
            oxc = (int)_pivot.x;
            oyc = (int)_pivot.y;

            for (int j = 0; j < _height; j++)
            {
                for (int i = 0; i < _width; i++)
                {
                    //_newArray[j * _width + i] = new Color32(0, 0, 0, 0);

                    x = Mathf.FloorToInt((cos * (i - xc) + sin * (j - yc) + oxc));
                    y = Mathf.CeilToInt((-sin * (i - xc) + cos * (j - yc) + oyc));

                    if ((x > -1) && (x < _oldWidth) && (y > -1) && (y < _oldHeight))
                    {
                        _newArray[j * _width + i] = _oldArray[y * _oldWidth + x];
                    }
                }
            }
        }

        private Texture2D _newTexture;

        private Color32[] _newArray;
        private Color32[] _oldArray;

        private int _width;
        private int _height;
        private int _oldWidth;
        private int _oldHeight;

        private Vector2 _pivot;
        private int _pixelsPerUnit;
        private FilterMode _filterMode;
    }
}
