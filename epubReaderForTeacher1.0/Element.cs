using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace epubReader4._0_Dino
{
    class Element
    {
        // メンバー変数の定義
        public string id;
        public int x;
        public int y;
        public int width;
        public int height;

        //Getメソッドの定義
        public string GetId()
        {
            return id;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        //Setメソッドの定義
        public void SetId(string id)
        {
            this.id = id;
        }

        public void SetX(int x)
        {
            this.x = x;
        }

        public void SetY(int y)
        {
            this.y = y;
        }

        public void SetWidth(int width)
        {
            this.width = width;
        }

        public void SetHeight(int height)
        {
            this.height = height;
        }
    }
}
