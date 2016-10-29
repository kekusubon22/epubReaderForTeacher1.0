using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace epubReader4._0_Dino
{
    public class StrokeLine
    {
        // メンバー変数の定義
        public int id;
        public List<Point> points;
        public Color color;
        public int width;
        public bool downNow;
        public bool inSpace;
        public bool erased;
        public int erasedTime;

        //Getメソッドの定義
        public int GetId()
        {
            return id;
        }

        public List<Point> GetPoints()
        {
            return points;
        }

        public Color GetColor()
        {
            return color;
        }

        public int GetWidth()
        {
            return width;
        }

        public bool GetDownNow()
        {
            return downNow;
        }

        public bool GetInSpace()
        {
            return inSpace;
        }

        public bool GetEreased()
        {
            return erased;
        }

        public int GetEreasedTime()
        {
            return erasedTime;
        }

        //Setメソッドの定義
        public void SetId(int id)
        {
            this.id = id;
        }

        public void SetPoints(List<Point> points)
        {
            this.points = points;
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }

        public void SetWidth(int width)
        {
            this.width = width;
        }

        public void SetDownNow(bool downNow)
        {
            this.downNow = downNow;
        }

        public void SetInSpace(bool inSpace)
        {
            this.inSpace = inSpace;
        }

        public void SetEreased(bool erased)
        {
            this.erased = erased;
        }

        public void SetEreasedTime(int erasedTime)
        {
            this.erasedTime = erasedTime;
        }
    }
}
